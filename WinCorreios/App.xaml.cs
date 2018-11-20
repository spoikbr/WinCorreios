using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WinCorreios
{
    /// <summary>
    /// Interação lógica para App.xaml
    /// </summary>
    public partial class App : Application
    {
        async void App_Startup(object sender, StartupEventArgs e)
        {
            //Caso haja um argument -hide, a interface não será mostrada.
            //Utilizado como argumento quando inicializar com o Windows.
            bool hide = false;
            for (int i = 0; i != e.Args.Length; ++i)
            {
                if (e.Args[i] == "-hide")
                {
                    hide = true;
                }
            }
            
            MainWindow mainWindow = new MainWindow();
            
            if (hide != true)
            {
                mainWindow.Show();
            }
            else
            {
                mainWindow.visible = false;
                await mainWindow.UpdateObjects();
            }
        }
    }
}
