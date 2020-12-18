using System;
using System.ComponentModel;
using System.Windows.Input;
using MouseKeyboardActivityMonitor;
using MouseKeyboardActivityMonitor.WinApi;
using Scroller.Extensions;
using Scroller.Interfaces;
using Scroller.Views;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using Scroller.Commands;

namespace Scroller.ViewModels
{
    public enum UnmuteOptions
    {
        DisableScroll,
        UnmuteScroll,
        UnmuteWithPreferredVolume
    };

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly KeyboardHookListener m_KeyboardHookListener;
        private readonly MouseHookListener m_MouseHookManager;
        private readonly IVolumeManager m_VolumeManager;
        private readonly VolumeOverlay m_VolumeOverlay = new VolumeOverlay();

        private readonly ICommand m_MuteCommand;

        private UnmuteOptions m_UnmuteOption = Properties.Settings.Default.UnmuteSetting;
        public UnmuteOptions UnmuteOption 
        {
            get { return m_UnmuteOption; }
            set
            {
                if(m_UnmuteOption != value)
                {
                    m_UnmuteOption = value;
                    OnPropertyChanged("UnmuteOption");

                    Properties.Settings.Default.UnmuteSetting = value;
                    Properties.Settings.Default.Save();
                }
            } 
        }

        private uint m_VolumeStepSize = Properties.Settings.Default.VolumeStepValue;
        public uint VolumeStepSize
        {
            get { return m_VolumeStepSize; }
            set
            {
                if(m_VolumeStepSize != value)
                {
                    m_VolumeStepSize = value;                    
                    OnPropertyChanged("VolumeStepSize");

                    Properties.Settings.Default.VolumeStepValue = value;
                    Properties.Settings.Default.Save();
                }
            }
        }

        public string VolumePercentage
        {
            get { return string.Format("{0}%", Math.Round(m_VolumeManager.CurrentVolume() * 100, 0)); }
        }

        public int Volume
        {
            get { return (int)Math.Round(m_VolumeManager.CurrentVolume() * 100, 0); }
        }

        private bool m_Mute;
        public bool Mute
        {
            get { return m_Mute; }
            set
            {
                if (value != m_Mute)
                {
                    m_Mute = value;
                    OnPropertyChanged("Mute");
                }
            }
        }

        public ICommand MuteCommand
        {
            get { return m_MuteCommand; }
        }

        private uint m_UnmuteVolume = Properties.Settings.Default.PreferredUnmuteVolume;
        public uint UnmuteVolume
        {
            get { return m_UnmuteVolume; }
            set
            {
                if(m_UnmuteVolume != value)
                {
                    m_UnmuteVolume = value;
                    OnPropertyChanged("UnmuteVolume");

                    Properties.Settings.Default.PreferredUnmuteVolume = value;
                    Properties.Settings.Default.Save();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            m_VolumeManager = new VolumeManager();
            m_MuteCommand = new MuteCommand(this);

            m_MouseHookManager = new MouseHookListener(new GlobalHooker()) { Enabled = true };
            m_MouseHookManager.MouseWheel += OnMouseWheelActivity;

            m_KeyboardHookListener = new KeyboardHookListener(new GlobalHooker()) { Enabled = true };
            m_KeyboardHookListener.KeyUp += OnKeyReleased;

            Mute = m_VolumeManager.IsMuted();
            m_VolumeManager.OnAudioNotification += OnAudioNotification;

            m_VolumeOverlay.DataContext = this;
            m_VolumeOverlay.Hide();
        }

        void OnAudioNotification(object sender, AudioVolumeNotificationEventArgs e)
        {
            Mute = e.Data.Muted;
        }

        private void OnKeyReleased(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Alt)
            {
                m_VolumeOverlay.Hide();
            }
        }

        private void OnMouseWheelActivity(object sender, MouseEventArgs mouseEventArgs)
        {
            if (Keyboard.Modifiers == ModifierKeys.Alt)
            {
                var mouseEventExtArgs = mouseEventArgs as MouseEventExtArgs;
                if (mouseEventExtArgs != null)
                {
                    mouseEventExtArgs.Handled = true;
                }

                if (mouseEventArgs.Delta.IsPositive())
                {
                    m_VolumeManager.VolumeUp(UnmuteOption, UnmuteVolume, VolumeStepSize);
                }
                else
                {
                    m_VolumeManager.VolumeDown(UnmuteOption, UnmuteVolume, VolumeStepSize);
                }

                OnPropertyChanged("Mute");
                OnPropertyChanged("VolumePercentage");
                OnPropertyChanged("Volume");

                m_VolumeOverlay.Show();
            }
        }

        /// <summary>
        /// Called when <see cref="MainWindow"/> will be closed
        /// </summary>
        public void OnClosing()
        {
            m_VolumeOverlay.Close();
            m_MouseHookManager.MouseWheel -= OnMouseWheelActivity;
            m_VolumeManager.OnAudioNotification -= OnAudioNotification;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void HandleMuteCommand(bool val)
        {
            Mute = val;
            if (Mute)
            {
                m_VolumeManager.Mute();
            }
            else
            {
                m_VolumeManager.UnMute();
            }
        }

        /// <summary>
        /// Opens the <param name="navigationUri">Uri</param> in the default browser.
        /// </summary>
        /// <param name="navigationUri">The navigation URI.</param>
        public void OpenUrl(object navigationUri)
        {
            try { System.Diagnostics.Process.Start((string)navigationUri); }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}