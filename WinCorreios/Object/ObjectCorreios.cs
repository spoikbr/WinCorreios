using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace WinCorreios.Object
{
    [System.Xml.Serialization.XmlInclude(typeof(EventCorreios))]
    [Serializable]
    public class ObjectCorreios : Object
    {
        private string _name;
        private string _trackingCode;
        private bool _isUserFinalized;
        private ObservableCollection<EventBase> _events = new ObservableCollection<EventBase>();
        private bool _hasUnseenUpdates;
        private string _packageType;
        public override string Name
        {
            get
            {
                return _name;
            }
            set
            {
                SetProperty(ref _name, value);
            }
        }
        public override string TrackingCode
        {
            get
            {
                return _trackingCode;
            }
            set
            {
                _trackingCode = value;
            }
        }
        public override string PackageType
        {
            get
            {
                return _packageType;
            }
            set
            {
                _packageType = value;
            }
        }
        public override EventBase LatestEvent
        {
            get
            {
                EventBase evt = Events[0];
                return evt;
            }
        }
        public override DateTime LatestEventDateTime
        {
            get
            {
                EventBase evt = Events[0];
                return evt.Date;
            }
        }

        public override bool IsDelivered
        {
            get
            {
                EventBase evt = Events[0];
                if (evt.Status == "Objeto entregue ao destinatário")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public override bool IsUserFinalized
        {
            get
            {
                return _isUserFinalized;
            }
            set
            {
                _isUserFinalized = value;
            }
        }
        public override int DaysInTransport 
        {
            get
            {
                if (IsDelivered == true)
                {
                    EventBase LatestEvent = Events[0];
                    EventBase FirstEvent = Events[Events.Count - 1];

                    return (int)(LatestEvent.Date - FirstEvent.Date).TotalDays;
                }
                else
                {
                    
                    EventBase FirstEvent = Events[Events.Count - 1];

                    return (int)(DateTime.Now - FirstEvent.Date).TotalDays;
                }
            }

        }

        //País de origem, no caso dos Correios isso é mostrado pelos dois últimos caracteres do código de rastreio
        public override string CountryOrigin
        {
            get
            {
                string countryCode = TrackingCode.Substring(TrackingCode.Length - 2);
                RegionInfo regionInfo = new RegionInfo(countryCode);
                return regionInfo.DisplayName;
            }
        }
        public override ObservableCollection<EventBase> Events
        {
            get
            {
                return _events;
            }
            set
            {
                SetProperty(ref _events, value);
                OnPropertyChanged("LatestEvent");
                //OnPropertyChanged("LatestEventDate");
            }
        }

        public override bool HasUnseenUpdates
        {
            get
            {
                return _hasUnseenUpdates;
            }
            set
            {
                SetProperty(ref _hasUnseenUpdates, value);
            }
        }
        private int _newEventsCount;
        public override int NewEventsCount
        {
            get
            {
                return _newEventsCount;
            }
            set
            {
                _newEventsCount = value;
            }
        }

        public ObjectCorreios(string name, string trackingCode)
        {
            Name = name;
            _trackingCode = trackingCode;
        }

        public override async Task<bool> UpdateAsync()
        {
            XmlDocument response = await Providers.CorreiosWebPost.GetEvents(TrackingCode);
            if (response == null)
            {
                throw new Exceptions.ConnectionProblemException();
            }
            PackageType = response.SelectSingleNode("rastro/objeto/categoria").InnerText;
            List<EventBase> events = new List<EventBase>();

            foreach (XmlNode node in response.GetElementsByTagName("evento"))
            {

                string status = node.SelectSingleNode("descricao").InnerText;
                DateTime date = DateTime.ParseExact(node.SelectSingleNode("data").InnerText,
                    "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                DateTime date_hour = DateTime.ParseExact(node.SelectSingleNode("hora").InnerText, "HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                date = date.AddHours(date_hour.Hour);
                date = date.AddMinutes(date_hour.Minute);
                string place;
                if (node.SelectSingleNode("unidade/cidade") == null)
                {
                    place = node.SelectSingleNode("unidade/local").InnerText;
                }
                else
                {
                    string place_city = String.Format("{0} / {1}", node.SelectSingleNode("unidade/cidade").InnerText,
                    node.SelectSingleNode("unidade/uf").InnerText);
                    place = String.Format("{0} em {1}", node.SelectSingleNode("unidade/local").InnerText, place_city);
                }
                string destiny = String.Empty;
                
                if (node.SelectSingleNode("destino") != null)
                {
                    XmlNode destinyNode = node.SelectSingleNode("destino");                    
                    string destiny_city = String.Format("{0} / {1}", destinyNode.SelectSingleNode("cidade").InnerText,
                        destinyNode.SelectSingleNode("uf").InnerText);
                    destiny = String.Format("{0} em {1}", destinyNode.SelectSingleNode("local").InnerText,destiny_city);
                }
                
                events.Add(new EventCorreios(status, date, "", place,destiny));
            }
            //Caso o número de eventos retornados pelos Correios for maior que os do objeto, existem atualizações
            int difference = events.Count - Events.Count;
            if (difference > 0)
            {
                HasUnseenUpdates = true;
                NewEventsCount = difference;                
                Events = new ObservableCollection<EventBase>(events);
                Save();
                return true;
            }
            else
            {
                return false;
            }

        }
        
        public override void Delete()
        {
            string path = 
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                "WinCorreios", "Objects", String.Format("{0}-{1}.object", Name, TrackingCode));
            File.Delete(path);
        }

        public override void Save()
        {
            SaveObject saveObject = new SaveObject(this);
            var serializer = new XmlSerializer(saveObject.GetType());
            string path =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "WinCorreios", "Objects", String.Format("{0}-{1}.object", Name, TrackingCode));

            using (var writer = XmlWriter.Create(path))
            {
                serializer.Serialize(writer, saveObject);
            }
        }
        //Construtor vazio para a serialização
        public ObjectCorreios()
        {

        }
    }
}

    

