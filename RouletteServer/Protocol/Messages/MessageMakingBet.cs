using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouletteServer.Protocol.Messages
{
    class MessageMakingBet : Message
    {
        public MessageType MessageType { get; set; }
        public Bet Bet { get; set; }

        public MessageMakingBet(byte[] bytes)
        {
            MessageType = (MessageType)bytes[0];
            byte[] betBytes = new byte[bytes.Length - 1];
            Array.Copy(bytes, 1, betBytes, 0, betBytes.Length);
            Bet = new Bet(betBytes);
        }

        
        public override byte[] GetBytes()
        {
            throw new NotImplementedException();
        }
    }
}
