using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WinCorreios
{
    public class AddObjectVM : ViewModelBase
    {
        public AddObjectWindow addObjectWindow;

        private string _objectName = String.Empty;
        public string ObjectName
        {
            get => _objectName;
            set => SetProperty(ref _objectName, value);
        }
        private string _trackingCode = String.Empty;
        public string TrackingCode
        {
            get => _trackingCode;
            set => SetProperty(ref _trackingCode, value);
        }

        private readonly DelegateCommand _addObjectCommand;
        public ICommand AddObjectCommand => _addObjectCommand;

        private readonly DelegateCommand _editObjectCommand;
        public ICommand EditObjectCommand => _editObjectCommand;
        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set
            {

                SetProperty(ref _isEditing, value);
                ObjectName = Object.Name;
                TrackingCode = Object.TrackingCode;
            }

        }
        public Object.Object Object;

        public AddObjectVM()
        {
            _addObjectCommand = new DelegateCommand(AddObject, CanAddObject);
            _editObjectCommand = new DelegateCommand(EditObject, CanEditObject);
        }


        private async void AddObject(object commandParameter)
        {

            if (ObjectName == String.Empty)
            {
                MessageBox.Show("O nome do objeto não pode estar em branco.","WinCorreios",
                    MessageBoxButton.OK,MessageBoxImage.Information);
                return;
            }
            if (TrackingCode.Length < 13)
            {
                MessageBox.Show("O código de rastreio deve ter 13 caracteres.", "WinCorreios",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            Regex r = new Regex("^[A-Z]{2}[0-9]{9}[A-Z]{2}$");
            if (!r.IsMatch(TrackingCode))
            {
                MessageBox.Show("Formato de código invalido.", "WinCorreios",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            List < Object.Object > verificationList = new List<Object.Object>();
            verificationList.AddRange(addObjectWindow.mainWindow.vm.InProgressObjects);
            verificationList.AddRange(addObjectWindow.mainWindow.vm.FinalizedObjects);
            Object.Object verificationOb = verificationList.FirstOrDefault(x => x.TrackingCode == TrackingCode);
            if (verificationOb != null)
            {
                MessageBox.Show(String.Format("O código de rastreio já foi adicionado no objeto {0}.",verificationOb.Name),"WinCorreios",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            addObjectWindow.AddButton.IsEnabled = false;
            Object.ObjectCorreios ob = new Object.ObjectCorreios(ObjectName, TrackingCode);
            try
            {
                await ob.UpdateAsync();
            }
            catch (Object.Exceptions.ObjectNotFoundException e)
            {
                MessageBox.Show(e.Message,"WinCorreios", MessageBoxButton.OK, MessageBoxImage.Information);
                addObjectWindow.AddButton.IsEnabled = true;
                return;
            }
            catch (Object.Exceptions.ConnectionProblemException e)
            {
                MessageBox.Show(e.Message,"Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                addObjectWindow.AddButton.IsEnabled = true;
                return;
            }
            ob.Save();
            addObjectWindow.mainWindow.AddObject(ob);
            addObjectWindow.Close();
        }
        private bool CanAddObject(object commandParameter)
        {
            return true;
        }
        private void EditObject(object commandParameter)
        {
            if (ObjectName == String.Empty)
            {
                MessageBox.Show("O nome do objeto não pode estar em branco.", "WinCorreios",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            Object.Delete();
            if (Object.IsDelivered == true)
            {
                addObjectWindow.mainWindow.vm.FinalizedObjects.Remove(Object);
            }
            else
            {
                addObjectWindow.mainWindow.vm.InProgressObjects.Remove(Object);
            }
            Object.Name = ObjectName;
            Object.Save();
            addObjectWindow.mainWindow.AddObject(Object);
            addObjectWindow.Close();
        }

        private bool CanEditObject(object commandParameter)
        {
            return true;
        }
    }
}
