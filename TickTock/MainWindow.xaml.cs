using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TickTock
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int m_timerStepMin = 10; // 

        public TimeSpan StartTime { get; set; }

        public TimeSpan CurrentTime { get; set; }

        public string CurrentTaskName { get; set; }

        public bool IsRunning { get; set; }

        private DispatcherTimer m_countdownTimer = new DispatcherTimer();
        public MainWindow()
        {
            this.CurrentTaskName = "Task";
            this.IsRunning = false;
            this.StartTime = TimeSpan.FromMinutes(0);
            this.CurrentTime = TimeSpan.FromMinutes(0);
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Buffer for scroll bar
            const double positionBuffer = 50;
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width - positionBuffer;
            this.Top = desktopWorkingArea.Bottom - this.Height;
    }

        private void TaskTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.TaskTextBlock.Visibility = System.Windows.Visibility.Collapsed;
            this.TaskTextBox.Visibility = System.Windows.Visibility.Visible;
            this.TaskTextBox.Focusable = true;
        }

        private void TaskTextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            this.TaskTextBlock.Visibility = System.Windows.Visibility.Visible;
            this.TaskTextBox.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void TaskTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Keyboard.Focus(this.MainPanel);
                this.TaskTextBox.Focusable = false;
            }
        }

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((e.Source != this.TaskTextBlock) &&
                    (e.Source != this.TaskTextBox))
            {
                Keyboard.Focus(this.MainPanel);
                this.TaskTextBox.Focusable = false;
            }
            
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        void UpdateTimerDisplay()
        {
            this.CountdownTextBox.Text = string.Format("{0:000}:{1:ss}", (int)CurrentTime.TotalMinutes, this.CurrentTime); //this.CurrentTime.ToString(@"%m\:ss");
        }

        void Pause()
        {
            this.IsRunning = false;
            this.PlayPauseButton.Content = ">";
            m_countdownTimer.Stop();
            UpdateTimerDisplay();
        }

        void Stop()
        {
            Pause();
            this.CurrentTime = TimeSpan.FromSeconds(0);
            UpdateTimerDisplay();
        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.IsRunning)
            {
                Pause();
            }
            else
            {
                if (this.CurrentTime == TimeSpan.Zero)
                {
                    return;
                }

                this.IsRunning = true;
                this.PlayPauseButton.Content = "||";
                UpdateTimerDisplay();

                m_countdownTimer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
                {
                    this.CurrentTime = this.CurrentTime.Subtract(TimeSpan.FromSeconds(1));
                    UpdateTimerDisplay();

                    if (this.CurrentTime == TimeSpan.Zero)
                    {
                        Pause();
                    }
                }, Application.Current.Dispatcher);

                m_countdownTimer.Start();
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        private void AddTimeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.IsRunning)
            {
                Pause();
            }
            this.CurrentTime = this.CurrentTime.Add(TimeSpan.FromMinutes(m_timerStepMin));
            UpdateTimerDisplay();
        }

        private void SubtractTimeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.IsRunning)
            {
                Pause();
            }
            if (this.CurrentTime == TimeSpan.Zero)
            {
                this.CurrentTime = this.CurrentTime.Subtract(TimeSpan.FromMinutes(m_timerStepMin));
            }
            UpdateTimerDisplay();
        }
    }
}
