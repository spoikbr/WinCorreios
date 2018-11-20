using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCorreios.Object.Exceptions
{
    public class ObjectNotFoundException : Exception
    {
        public string sroCode;
        
        public ObjectNotFoundException(string sroCode)
            : base(String.Format("O objeto {0} não foi encontrado.", sroCode))
        {
            this.sroCode = sroCode;
        }
    }
}
