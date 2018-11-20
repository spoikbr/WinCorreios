using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCorreios.Object
{
    [Serializable]
    [System.Xml.Serialization.XmlInclude(typeof(ObjectCorreios))]
    public class SaveObject
    {
        public Object Object;
        public SaveObject()
        {

        }
        public SaveObject(Object ob)
        {
            Object = ob;
        }
    }
}
