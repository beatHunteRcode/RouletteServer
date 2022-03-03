using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouletteServer.Protocol.Messages
{
    class MessageDrawing : Message
    {
        public MessageType MessageType { get; set; }

        public MessageDrawing(byte[] bytes)
        {
            MessageType = (MessageType)bytes[0];
        }

        public MessageDrawing()
        {
            MessageType = MessageType.DRAWING;
        }

        public override byte[] GetBytes()
        {
            throw new NotImplementedException();
        }
    }
}
