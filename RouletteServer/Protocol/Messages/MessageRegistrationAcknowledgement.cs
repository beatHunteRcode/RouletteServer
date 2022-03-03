using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouletteServer.Protocol.Messages
{
    class MessageAcknowledgement : Message
    {
        public MessageType MessageType { get; set; }

        public MessageAcknowledgement()
        {
            MessageType = MessageType.ACK;
        }

        public override byte[] GetBytes()
        {
            return new byte[] { (byte)MessageType };
        }
    }
}
