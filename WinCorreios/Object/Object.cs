using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WinCorreios.Object
{
    public abstract class Object : INotifyPropertyChanged
    {
        public abstract string Name { get; set; }
        public abstract string TrackingCode { get; set; }
        public abstract string PackageType { get; set; }
        public abstract EventBase LatestEvent { get; }
        public abstract DateTime LatestEventDateTime { get; }
        public abstract bool IsDelivered { get; }
        public abstract bool IsUserFinalized { get; set; } 
        public abstract bool HasUnseenUpdates { get; set; } 
        public abstract int DaysInTransport { get; }
        public abstract string CountryOrigin { get; }
        public abstract ObservableCollection<EventBase> Events { get; set; }
        public abstract int NewEventsCount { get; set; }
        
        public abstract Task<bool> UpdateAsync(); //Retorna true caso haja novas atualizações

        public abstract void Delete();
        public abstract void Save();

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName]string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                field = newValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }
            return false;
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
