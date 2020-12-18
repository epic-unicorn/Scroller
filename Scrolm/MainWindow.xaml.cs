using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using MahApps.Metro.Controls;
using Scroller.ViewModels;
using Application = System.Windows.Application;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Scroller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly MainWindowViewModel m_MainWindowViewModel;
        private NotifyIcon m_NotifyIcon;

        public MainWindow()
        {
            InitializeComponent();           
            m_MainWindowViewModel = new MainWindowViewModel();
            DataContext = m_MainWindowViewModel;
            SetupWindow();
        }

        private void SetupWindow()
        {
            m_NotifyIcon = new NotifyIcon
            {
                BalloonTipText = Properties.Resources.TrayIconText,
                BalloonTipIcon = ToolTipIcon.Info,
                Text = Properties.Resources.TrayIconTitle
            };

            var open = new ToolStripMenuItem("Open", null, OnConfigClick);
            var close = new ToolStripMenuItem("Exit", Image.FromFile("Resources/close.png"), OnExitClick);

            var contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip.Items.Add(open);
            contextMenuStrip.Items.Add(close);

            m_NotifyIcon.ContextMenuStrip = contextMenuStrip;
            m_NotifyIcon.MouseDoubleClick += OnNotifyIconDoubleClicked;

            try { m_NotifyIcon.Icon = new Icon("Resources/icon.ico"); }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void OnConfigClick(object sender, EventArgs e)
        {
            MinimizeToTray(false);
        }

        private void OnExitClick(object sender, EventArgs e)
        {
            Close();
        }

        private void OnNotifyIconDoubleClicked(object sender, MouseEventArgs e)
        {
            MinimizeToTray(false);
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           var result = MessageBox.Show(Properties.Resources.WindowClosing_Are_you_sure_to_close_this_application_, Properties.Resources.WindowClosing_Close_Application, MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if(result == System.Windows.Forms.DialogResult.Yes)
            {
                m_NotifyIcon.MouseDoubleClick -= OnNotifyIconDoubleClicked;
                m_MainWindowViewModel.OnClosing();
            }
            else
            {
                e.Cancel = true;
                MinimizeToTray(true);
            }
        }

        private void WindowStateChanged(object sender, EventArgs e)
        {
            MinimizeToTray(WindowState == WindowState.Minimized);
        }

        private void MinimizeToTray(bool inTray)
        {
            if (inTray)
            {
                ShowInTaskbar = false;
                WindowState = WindowState.Minimized;

                if (m_NotifyIcon != null)
                {
                    m_NotifyIcon.Visible = true;
                }
            }
            else
            {
                ShowInTaskbar = true;
                WindowState = WindowState.Normal;

                Activate();
                if (m_NotifyIcon != null)
                {
                    m_NotifyIcon.Visible = false;
                }
            }
        }
    }
}
