using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouletteServer.Protocol.Messages
{
    class MessageDisconnect : Message
    {
        MessageType MessageType { get; set; }

        public MessageDisconnect(byte[] bytes)
        {
            MessageType = (MessageType)bytes[0];
        }

        public MessageDisconnect()
        {
            MessageType = MessageType.DISCONNECT;
        }
        public override byte[] GetBytes()
        {
            return new byte[] { (byte)MessageType };
        }
    }
}
