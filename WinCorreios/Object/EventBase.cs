using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WinCorreios.Object
{
    public abstract class EventBase
    {
        public abstract string Status { get; set; }
        public abstract string DateAbbreviation { get; }
        public abstract string FullDate { get; }
        public abstract DateTime Date { get; set; }
        public abstract string Description { get; set; } //Não implementado
        public abstract bool DescriptionExists { get; } //Não implementado
        public abstract string Place { get; set; }        
        public abstract string Destiny { get; set; }
        public abstract bool DestinyExists { get; }
        public abstract Brush Color { get; }
        public abstract TransformedBitmap Image { get; }
    }
}
