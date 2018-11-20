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

namespace WinCorreios
{
    /// <summary>
    /// Lógica interna para AddObjectWindow.xaml
    /// </summary>
    public partial class AddObjectWindow : Window
    {
        public MainWindow mainWindow;
        public AddObjectVM vm;

        public AddObjectWindow()
        {
            
            InitializeComponent();
            vm = (AddObjectVM)DataContext;
            vm.addObjectWindow = this;
        }
    }
}
