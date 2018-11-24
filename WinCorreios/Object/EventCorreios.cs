using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WinCorreios.Object
{

    public class EventCorreios : EventBase
    {
        private string _status;
        private DateTime _date;
        private string _description;
        private string _place;
        private string _destiny;
        public override string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }
        public override string DateAbbreviation 
        {
            get
            {
                return Date.ToString("d 'de' MMM HH:mm",CultureInfo.CurrentCulture);
                
            }
        }
        public override string FullDate
        {
            get
            {
                return Date.ToString("dd/MM/yyyy HH:mm", CultureInfo.CurrentCulture);
            }
        }
        public override DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
            }
        }

        public override string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }
        public override bool DescriptionExists
        {
            get
            {
                if (Description == String.Empty)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        public override string Place
        {
            get
            {
                return _place;
            }
            set
            {
                _place = value;
            }
        }

        public override string Destiny
        {
            get
            {
                return _destiny;
            }
            set
            {
                _destiny = value;
            }
        }
        public override bool DestinyExists
        {
            get
            {
                if (Destiny == string.Empty)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        //Retorna a cor que será mostrada para o usuário, caso não haja cor específica, retorna a cor padrão
        public override Brush Color
        {
            get
            {
                if (Status.Contains("postado"))
                {
                    return Brushes.Purple;
                }
                if (Status.Contains("encaminhado"))
                {
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00a86d"));
                }
                if (Status.Contains("saiu para entrega"))
                {
                    return Brushes.DodgerBlue;
                }
                if (Status.Contains("aguardando retirada"))
                {
                    return Brushes.DarkOrange;
                }
                if (Status.Contains("entregue ao destinatário"))
                {
                    return Brushes.Green;
                }
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#153450"));

            }
        }
        //Retorna a imagem que será mostrada para o usuário, caso não haja cor específica, retorna a cor padrão
        public override TransformedBitmap Image
        {
            get
            {
                if (Status.Contains("postado"))
                {
                    BitmapImage image = new BitmapImage(new Uri("pack://application:,,,/Images/Arrow.png"));
                    TransformedBitmap transformedBitmap = new TransformedBitmap(image, new RotateTransform(-90));
                    return transformedBitmap;
                }
                if (Status.Contains("encaminhado"))
                {
                    BitmapImage image = new BitmapImage(new Uri("pack://application:,,,/Images/Arrow.png"));
                    TransformedBitmap transformedBitmap = new TransformedBitmap(image, new RotateTransform(0));
                    return transformedBitmap;
                }
                if (Status.Contains("saiu para entrega"))
                {
                    BitmapImage image = new BitmapImage(new Uri("pack://application:,,,/Images/Truck.png"));
                    TransformedBitmap transformedBitmap = new TransformedBitmap(image, new RotateTransform(0));
                    return transformedBitmap;
                }
                if (Status.Contains("entregue ao destinatário"))
                {
                    BitmapImage image = new BitmapImage(new Uri("pack://application:,,,/Images/Check.png"));
                    TransformedBitmap transformedBitmap = new TransformedBitmap(image, new RotateTransform(0));
                    return transformedBitmap;
                }
                BitmapImage defaultImage = new BitmapImage(new Uri("pack://application:,,,/Images/PackageWhite.png"));
                TransformedBitmap transformedDefault = new TransformedBitmap(defaultImage, new RotateTransform(0));
                return transformedDefault;
            }
        }

        public EventCorreios(string status, DateTime date, string description,
            string place, string destiny)
        {
            Status = status;
            Date = date;
            Description = description;
            Place = place;
            Destiny = destiny;
        }
        //Construtor vazio para serialização
        public EventCorreios()
        {

        }
    }
}

