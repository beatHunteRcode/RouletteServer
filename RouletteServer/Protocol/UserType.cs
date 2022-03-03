using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouletteServer.Protocol
{
    public enum UserType: byte
    {
        PLAYER = 1,
        CROUPIER = 2
    }
}
