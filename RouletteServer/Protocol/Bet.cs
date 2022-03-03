using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouletteServer.Protocol
{
    class Bet
    {
        public BetType BetType { get; set; }
        public byte Number { get; set; }
        public short Sum { get; set; }

        public Bet(byte[] bytes)
        {
            BetType = (BetType)bytes[0];
            Number = bytes[1];
            Sum = Utilities.convertBytesArrayToShort(new byte[] { bytes[2], bytes[3] });
        }

        public Bet(BetType betType, short sum)
        {
            BetType = betType;
            Sum = sum;
        }

        public Bet(byte number, short sum)
        {
            Number = number;
            Sum = sum;
        }

        public byte[] GetBytes()
        {
            //byte[] betTypeBytes = new byte[] { (byte)BetType };
            //byte[] numberBytes = new byte[] { Number };
            byte[] sumBytes = Utilities.convertShortToByteArray(Sum);

            //return betTypeBytes.Concat(numberBytes).Concat(sumBytes).ToArray();
            return sumBytes;
        }
    }
}
