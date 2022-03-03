using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouletteServer.Protocol.Messages
{
    class MessageHeader : Message
    {
        public int MessageLength { get; set; }
        public MessageType MessageType { get; set; }

        private byte[] MessageLengthArr = new byte[sizeof(Int32)];

        public MessageHeader(byte[] bytes)
        {
            Array.Copy(bytes, 0, MessageLengthArr, 0, 4);
            MessageLength = Utilities.convertBytesArrayToInt32(MessageLengthArr);
            MessageType = (MessageType)bytes[4];
        }

        public override byte[] GetBytes()
        {
            throw new NotImplementedException();
        }
    }
}
