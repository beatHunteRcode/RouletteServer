using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouletteServer.Protocol.Messages
{
    class MessageInforming : Message
    {
        public MessageType MessageType { get; set; }
        public byte Number { get; set; }

        public MessageInforming(byte[] bytes)
        {
            MessageType = (MessageType)bytes[0];
            Number = bytes[1];
        }

        public MessageInforming()
        {
            MessageType = MessageType.INFORM_CLIENT;
        }

        public override byte[] GetBytes()
        {
            throw new NotImplementedException();
        }
    }
}
