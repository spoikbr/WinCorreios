using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Serialization;
using Octokit;

namespace WinCorreios
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowVM vm;
        public System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();
        public System.Timers.Timer updateTimer =
            new System.Timers.Timer(Properties.Settings.Default.UpdateSpan.TotalMilliseconds);

        public MainWindow()
        {
            
            InitializeComponent();
            InitNotifyIcon();
            vm = (MainWindowVM)DataContext;
            vm.mainWindow = this;
            LoadObjects();

            if (Properties.Settings.Default.AutomaticUpdates == true)
            {
                updateTimer.AutoReset = true;
                updateTimer.Elapsed += new ElapsedEventHandler(UpdateTimerElapsed);
                updateTimer.Start();
            }
        }
        private void InitNotifyIcon()
        {
            notifyIcon.Icon =
                new System.Drawing.Icon(System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/Images/Icon.ico")).Stream);
            notifyIcon.Text
                = String.Format("WinCorreios{0}Última atualização: {1}", Environment.NewLine, Properties.Settings.Default.LastUpdate);
            notifyIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add("Mostrar/Esconder janela", null, HideShow_Click);
            notifyIcon.MouseUp += new System.Windows.Forms.MouseEventHandler(NotifyIcon_Click);
            notifyIcon.ContextMenuStrip.Items.Add("Atualizar Objetos", null, UpdateObjects_Click);
            notifyIcon.ContextMenuStrip.Items.Add("Sair", null, Exit_Click);
            notifyIcon.Visible = true;
        }
        public void AddObject(Object.Object ob)
        {
            if (ob.IsDelivered == true || ob.IsUserFinalized == true)
            {
                vm.FinalizedObjects.Add(ob);
            }
            else
            {
                vm.InProgressObjects.Add(ob);
            }
        }
        private void LoadObjects()
        {
            
            List<string> files = new List<string>();
            string path =
                    System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WinCorreios", "Objects");
            try
            {
                
                files = Directory.GetFiles(path, "*.object").ToList();
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                Directory.CreateDirectory(path);
            }
            foreach (string file in files)
            {
                XmlDocument xml;
                using (StreamReader sr = new StreamReader(file, true))
                {
                    xml = new XmlDocument();
                    xml.Load(sr);
                }

                XmlSerializer xml_serializer = new XmlSerializer(typeof(Object.SaveObject));
                using (StringReader string_reader = new StringReader(xml.OuterXml))
                {
                    Object.SaveObject saveObject = (Object.SaveObject)(xml_serializer.Deserialize(string_reader));
                    AddObject(saveObject.Object);
                    
                }
                
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.AutomaticUpdates == true)
            {               
                await UpdateObjects();
            }
            if (Properties.Settings.Default.CheckForUpdates == true)
            {
                await CheckForUpdates();
            }                
        }
        private async Task CheckForUpdates()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var client = new GitHubClient(new ProductHeaderValue("WinCorreios"));
            try
            {
                var releases = await client.Repository.Release.GetAll("spoikbr", "WinCorreios");
                var latest = releases[0];
                Version LatestGithub = new Version(latest.TagName);
                Version Current = new Version(assembly.GetName().Version.ToString());
                if (LatestGithub > Current)
                {
                   MessageBoxResult result = MessageBox.Show("Uma nova atualização está disponível, deseja atualizar?\nVersão Atual: " + Current + "\nVocê pode desabilitar as verificações nas configurações",
                        "Nova atualização: " + LatestGithub,MessageBoxButton.YesNo,MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        System.Diagnostics.Process.Start("https://www.github.com/spoikbr/WinCorreios/releases/latest");
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        private async void UpdateTimerElapsed(object sender, ElapsedEventArgs e)
        {
            await UpdateObjects();
        }
        public async Task UpdateObjects()
        {
            vm.IsUpdating = true;
            List<Object.Object> objectsWithUpdates = new List<Object.Object>();
            for (int i = vm.InProgressObjects.Count - 1; i >= 0; i--)
            {
                Object.Object ob = vm.InProgressObjects[i];
                bool IsBeingViewed = false;
                if (vm.SelectedObject == ob)
                {
                    IsBeingViewed = true;
                }
                bool HasUpdates = false;
                try
                {
                    HasUpdates = await ob.UpdateAsync();
                }
                catch (Object.Exceptions.ConnectionProblemException ex)
                {
                    if (visible == true)
                    {
                        MessageBox.Show(ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                        continue;
                    }

                }
                catch (Object.Exceptions.ObjectNotFoundException ex)
                {
                    if (visible == true)
                    {
                        MessageBox.Show(ex.Message,"WinCorreios", MessageBoxButton.OK, MessageBoxImage.Information);
                        continue;
                    }
                }

                if (HasUpdates == true)
                {
                    objectsWithUpdates.Add(ob);
                }
                if (ob.IsDelivered)
                {
                    vm.InProgressObjects.RemoveAt(i);
                    vm.FinalizedObjects.Add(ob);
                }
                else
                {
                    if (IsBeingViewed == true)
                    {
                        ob.HasUnseenUpdates = false;
                        ob.Save();
                    }
                }

            }
            if (Properties.Settings.Default.Notifications == true)
            {
                if (objectsWithUpdates.Count == 1)
                {
                    Object.Object ob = objectsWithUpdates[0];
                    SendNotification(ob.Name, ob.Events[0].Status, System.Windows.Forms.ToolTipIcon.Info, 3000);
                }
                if (objectsWithUpdates.Count > 1)
                {
                    StringBuilder text = new StringBuilder();
                    text.Append("Atualizações nos objetos");
                    for (int i = 0; i < objectsWithUpdates.Count; i++)
                    {
                        if (i != objectsWithUpdates.Count - 1)
                        {
                            text.Append(String.Format(" {0},", objectsWithUpdates[i].Name));
                        }
                        else
                        {
                            text.Append(String.Format(" {0}.", objectsWithUpdates[i].Name));
                        }
                    }
                    SendNotification("WinCorreios", text.ToString(), System.Windows.Forms.ToolTipIcon.Info, 3000);
                }
            }
            vm.IsUpdating = false;
            Properties.Settings.Default.LastUpdate = DateTime.Now;
            Properties.Settings.Default.Save();
            notifyIcon.Text =
                String.Format("WinCorreios{0}Última atualização: {1}", Environment.NewLine, Properties.Settings.Default.LastUpdate);
        }
        private void SendNotification(string title, string text, System.Windows.Forms.ToolTipIcon icon, int timeout)
        {
            notifyIcon.BalloonTipTitle = title;
            notifyIcon.BalloonTipText = text;
            notifyIcon.BalloonTipIcon = icon;
            notifyIcon.ShowBalloonTip(timeout);
        }

        private void CopyObjectInfo_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(vm.SelectedObject.TrackingCode);
        }
        private void DeleteObject_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Você tem certeza que deseja excluir esse objeto?", "Excluir Objeto"
                , MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                vm.SelectedObject.Delete();
                if (vm.SelectedObject.IsDelivered == true || vm.SelectedObject.IsUserFinalized == true)
                {
                    vm.FinalizedObjects.Remove(vm.SelectedObject);
                }
                else
                {
                    vm.InProgressObjects.Remove(vm.SelectedObject);
                }
            }
        }
        private void EditObject_Click(object sender, RoutedEventArgs e)
        {
            AddObjectWindow addObject = new AddObjectWindow();
            addObject.mainWindow = this;
            addObject.vm.Object = vm.SelectedObject;
            addObject.vm.IsEditing = true;
            addObject.Show();
        }
        private void FinalizeObject_Click(object sender, RoutedEventArgs e)
        {
            Object.Object ob = vm.SelectedObject;
            if (ob.IsUserFinalized == true)
            {
                ob.IsUserFinalized = false;
                ob.Save();
                vm.InProgressObjects.Add(ob);
                vm.FinalizedObjects.Remove(ob);                
            }
            else
            {
                MessageBoxResult result = MessageBox.Show
                    ("Você tem certeza que deseja arquivar esse objeto?\nEle passará para a aba de objetos finalizados",
                    "Arquivar Objeto", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    ob.IsUserFinalized = true;
                    ob.Save();
                    vm.FinalizedObjects.Add(ob);
                    vm.InProgressObjects.Remove(ob);
                }                              
            }
        }
        public bool visible = true;
        private void HideShow_Click(object sender, EventArgs e)
        {

            if (visible == true)
            {
                Hide();
                visible = false;
            }
            else
            {
                Show();
                visible = true;
            }

        }
        private void NotifyIcon_Click(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (visible == true)
                {
                    Hide();
                    visible = false;
                }
                else
                {
                    Show();
                    visible = true;
                }
            }
        }
        private async void UpdateObjects_Click(object sender, EventArgs e)
        {
            await UpdateObjects();
        }
        private void Exit_Click(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            Properties.Settings.Default.Save();
            System.Windows.Application.Current.Shutdown();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
            visible = false;
        }

        public void FadeAnimation()
        {
            Storyboard sb = (Storyboard)FindResource("FadeIn");
            sb.Begin(EventsList, true);
            sb.Begin(SROCodeTxt, true);
            sb.Begin(NameTxt, true);
            sb.Begin(Details, true);
        }
    }
}

