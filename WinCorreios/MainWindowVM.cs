using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace WinCorreios
{
    public class MainWindowVM : ViewModelBase
    {

        public MainWindow mainWindow;
        //0 = Objetos em andamento, 1 = Objetos Finalizados, 2 = Resultado da Pesquisa
        private int _objectsListTab; 
        public int ObjectsListTab
        {
            get => _objectsListTab;
            set => SetProperty(ref _objectsListTab, value);
        }
        private bool _isUpdating;
        public bool IsUpdating
        {
            get => _isUpdating;
            set => SetProperty(ref _isUpdating, value);
        }
        

        private string _searchText = String.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                if (value == String.Empty)
                {
                    if (oldListTab == 0)
                    {                       
                        OnGoindObjectsBtn(null);
                    }
                    else if (oldListTab == 1)
                    {
                        FinalizedObjectsBtn(null);
                    }
                }
                else
                {
                    Search();
                }                
            }
        }

        //O objeto selecionado no momento
        private Object.Object _selectedObject;
        public Object.Object SelectedObject
        {
            get => _selectedObject;
            set
            {
                mainWindow.FadeAnimation();
                SetProperty(ref _selectedObject, value);
                if (value != null)
                {
                    if (value.HasUnseenUpdates == true)
                    {
                        value.HasUnseenUpdates = false;
                        value.Save();
                    }
                    if (value.Events.Count > 0)
                    {
                        //Reseta a scrollbar da lista de eventos
                        mainWindow.EventsList.ScrollIntoView(mainWindow.EventsList.Items[0]);
                    }
                }
            }
        }        

        private ObservableCollection<Object.Object> _inProgressObjects = new ObservableCollection<Object.Object>();
        public ObservableCollection<Object.Object> InProgressObjects
        {
            get => _inProgressObjects;
            set => _inProgressObjects = value;
        }

        private ObservableCollection<Object.Object> _finalizedObjects = new ObservableCollection<Object.Object>();
        public ObservableCollection<Object.Object> FinalizedObjects
        {
            get => _finalizedObjects;
            set => _finalizedObjects = value;
        }
        private ICollectionView _sortedObjects;
        public ICollectionView SortedObjects
        {
            get => _sortedObjects;
            set => SetProperty(ref _sortedObjects,value);
        }

        private readonly DelegateCommand _addObjectCommand;
        public ICommand AddObjectCommand => _addObjectCommand;

        private readonly DelegateCommand _updateObjectsCommand;
        public ICommand UpdateObjectsCommand => _updateObjectsCommand;

        private readonly DelegateCommand _onGoingObjectsCommand;
        public ICommand OnGoingObjectsCommand => _onGoingObjectsCommand;

        private readonly DelegateCommand _finalizedObjectsCommand;
        public ICommand FinalizedObjectsCommand => _finalizedObjectsCommand;

        private readonly DelegateCommand _settingsCommand;
        public ICommand SettingsCommand => _settingsCommand;

        private readonly DelegateCommand _exitCommand;
        public ICommand ExitCommand => _exitCommand;

        public MainWindowVM()
        {
            ObjectsListTab = 0;
            _addObjectCommand = new DelegateCommand(AddObject, CanAddObject);
            _updateObjectsCommand = new DelegateCommand(UpdateObjects, CanUpdateObjects);
            _onGoingObjectsCommand = new DelegateCommand(OnGoindObjectsBtn, CanOnGoindObjectsBtn);
            _finalizedObjectsCommand = new DelegateCommand(FinalizedObjectsBtn, CanFinalizedObjectsBtn);
            _settingsCommand = new DelegateCommand(SettingsBtn, CanOpenSettingsBtn);
            _exitCommand = new DelegateCommand(ExitBtn, CanExitBtn);

            //Por enquanto, organizamos os objetos apenas pela data do último evento, do mais recente para o mais antigo.
            SortedObjects = CollectionViewSource.GetDefaultView(InProgressObjects);
            SortedObjects.SortDescriptions.Add(new SortDescription("LatestEventDateTime", ListSortDirection.Descending));

            

        }
        private void AddObject(object commandParameter)
        {
            AddObjectWindow addObject = new AddObjectWindow();
            addObject.mainWindow = mainWindow;
            addObject.Show();
        }
        public async void UpdateObjects(object commandParameter)
        {
            await mainWindow.UpdateObjects();
        }

        private void OnGoindObjectsBtn(object commandParameter)
        {
            SortedObjects = CollectionViewSource.GetDefaultView(InProgressObjects);
            SortedObjects.SortDescriptions.Add(new SortDescription("LatestEventDateTime", ListSortDirection.Descending));
            ObjectsListTab = 0;
        }
        private void FinalizedObjectsBtn(object commandParameter)
        {
            SortedObjects = CollectionViewSource.GetDefaultView(FinalizedObjects);
            SortedObjects.SortDescriptions.Add(new SortDescription("LatestEventDateTime", ListSortDirection.Descending));
            ObjectsListTab = 1;
        }
        private void SettingsBtn(object commandParameter)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.mainWindow = mainWindow;
            settingsWindow.Show();
        }
        private int oldListTab;
        private void Search()
        {
            List<Object.Object> results = new List<Object.Object>();
            results.AddRange(InProgressObjects.Where(x => x.Name.Contains(SearchText)).ToList());
            results.AddRange(FinalizedObjects.Where(x => x.Name.Contains(SearchText)).ToList());

            SortedObjects = CollectionViewSource.GetDefaultView(results);
            SortedObjects.SortDescriptions.Add(new SortDescription("LatestEventDateTime", ListSortDirection.Descending));
            if (ObjectsListTab != 2)
            {
                oldListTab = ObjectsListTab;
                ObjectsListTab = 2;
            }
        }
        
        private void ErasingSearch(object commandParameter)
        {
            string text = commandParameter.ToString();
            if (text == String.Empty)
            {
                if (oldListTab == 0)
                {
                    OnGoindObjectsBtn(null);
                }
                else if (oldListTab == 1)
                {
                    FinalizedObjectsBtn(null);
                }
            }

        }
        private void ExitBtn(object commandParameter)
        {
            if (MessageBox.Show("Você tem certeza que deseja fechar o programa?", "Fechar"
                , MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                mainWindow.notifyIcon.Visible = false;
                Properties.Settings.Default.Save();
                Application.Current.Shutdown();
            }
        }
        private bool CanOpenSettingsBtn(object commandParameter)
        {
            return true;
        }
        private bool CanAddObject(object commandParameter)
        {
            return true;
        }
        private bool CanUpdateObjects(object commandParameter)
        {
            return true;
        }
        private bool CanOnGoindObjectsBtn(object commandParameter)
        {
            return true;
        }
        private bool CanFinalizedObjectsBtn(object commandParameter)
        {
            return true;
        }
        private bool CanSearch(object commandParameter)
        {
            return true;
        }
        private bool CanEraseSearch(object commandParameter)
        {
            return true;
        }
        private bool CanExitBtn(object commandParameter)
        {
            return true;
        }
    }
}
