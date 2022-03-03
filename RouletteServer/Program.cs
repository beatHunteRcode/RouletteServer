using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouletteServer
{
    class Program
    {

        const string ip = "127.0.0.1";
        const int port = 53;

        static void Main(string[] args)
        {
            Server server = new Server(ip, port);
            server.start();
        }
    }
}
