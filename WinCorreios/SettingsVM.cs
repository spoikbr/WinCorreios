using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WinCorreios
{
    public class SettingsVM : ViewModelBase
    {
        public SettingsWindow settingsWindow;
        public List<TimeSpan> _updateSpansList = new List<TimeSpan>()
        {
            TimeSpan.FromMinutes(3),
            TimeSpan.FromMinutes(5),
            TimeSpan.FromMinutes(15),
            TimeSpan.FromMinutes(30),
            TimeSpan.FromHours(1),
            TimeSpan.FromHours(3),
            TimeSpan.FromHours(6),
            TimeSpan.FromHours(9),
        };
        public string ProgramVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }
        public List<TimeSpan> UpdateSpansList
        {
            get
            {
                return _updateSpansList;
            }
        }
        
        public bool AutomaticUpdates
        {
            get
            {
                return Properties.Settings.Default.AutomaticUpdates;
            }
            set
            {
                if (value == true)
                {
                    settingsWindow.mainWindow.updateTimer.Start();
                }
                else
                {
                    settingsWindow.mainWindow.updateTimer.Stop();
                }               
                Properties.Settings.Default.AutomaticUpdates = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged("AutomaticUpdates");
            }
        }
        public bool Notifications
        {
            get
            {
                return Properties.Settings.Default.Notifications;
            }
            set
            {
                Properties.Settings.Default.Notifications = value;
                Properties.Settings.Default.Save();
            }
        }
        public bool CheckForUpdates
        {
            get
            {
                return Properties.Settings.Default.CheckForUpdates;
            }
            set
            {
                Properties.Settings.Default.CheckForUpdates = value;
                Properties.Settings.Default.Save();
            }
        }
        public bool OpenAtStartup
        {
            get
            {
                return Properties.Settings.Default.OpenAtStartup;
            }
            set
            {
                RegistryKey rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                if (value == true)
                {
                    rk.SetValue("WinCorreios", System.Reflection.Assembly.GetEntryAssembly().Location + " -hide");
                }
                else
                {
                    rk.DeleteValue("WinCorreios", false);
                }
                Properties.Settings.Default.OpenAtStartup = value;
                Properties.Settings.Default.Save();
            }
        }
        public TimeSpan UpdateSpan
        {
            get
            {
                return Properties.Settings.Default.UpdateSpan;
            }
            set
            {
                settingsWindow.mainWindow.updateTimer.Stop();
                settingsWindow.mainWindow.updateTimer.Interval = value.TotalMilliseconds;
                settingsWindow.mainWindow.updateTimer.Start();
                Properties.Settings.Default.UpdateSpan = value;
                Properties.Settings.Default.Save();

            }
        }
    }
}
