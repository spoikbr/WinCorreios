using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCorreios.Object.Exceptions
{
    public class ConnectionProblemException : Exception
    {
        public ConnectionProblemException()
            : base("Erro de comunicação, verifique sua conexão com a internet e tente novamente mais tarde.")
        {
        }
    }      
}
