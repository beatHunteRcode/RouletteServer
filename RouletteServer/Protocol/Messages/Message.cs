using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouletteServer.Protocol.Messages
{
    abstract class Message
    {
        abstract public byte[] GetBytes();
    }
}
