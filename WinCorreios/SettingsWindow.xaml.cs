using WinCorreios.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Navigation;

namespace WinCorreios
{
    /// <summary>
    /// Lógica interna para SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private SettingsVM vm;
        public MainWindow mainWindow;
        public SettingsWindow()
        {
            InitializeComponent();
            vm = (SettingsVM)DataContext;
            vm.settingsWindow = this;
        }
        private void Github_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }
    }
}
