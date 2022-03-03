using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouletteServer
{
    class Program
    {

        const string ip = "26.236.95.61";
        const int port = 451;

        static void Main(string[] args)
        {
            Server server = new Server(ip, port);
            server.start();
        }
    }
}
