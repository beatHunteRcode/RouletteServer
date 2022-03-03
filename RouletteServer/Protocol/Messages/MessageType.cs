using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouletteServer.Protocol.Messages
{
    public enum MessageType: byte
    {
        REGISTRATION = 1,
        BETS_ANNOUNCEMENT = 2,
        MAKING_BET = 3,
        DRAWING = 4,
        INFORM_CLIENT = 5,
        DISCONNECT = 6,
        ACK = 7
    }
}
