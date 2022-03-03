using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouletteServer.Protocol
{
    public class Utilities
    {
        public const int TEMP_BUFFER_SIZE = 512;
        public const byte HEADER_SIZE = 3;
        public const int SLEEP_TIME_MS = 100;

        public const string SERVER_NAME = "Server";

		public const byte SIZE_FIELD_LENGTH = 3;
		public const int MAX_LENGTH = 16777215; // 256^3 - 1


		public static Int32 convertBytesArrayToInt32(byte[] bytes)
        {
            return BitConverter.ToInt32(bytes, 0);
        }

        public static short convertBytesArrayToShort(byte[] bytes)
        {
            return (short)BitConverter.ToUInt16(new byte[2] { (byte)bytes[1], (byte)bytes[0] }, 0);
        }
		public static byte[] convertShortToByteArray(short value)
		{
			byte[] arr = BitConverter.GetBytes(value);
			byte tmp = 0;
			tmp = arr[0];
			arr[0] = arr[1];
			arr[1] = tmp;

			return arr;
		}

		public static string GetCurrentTime()
        {
            return DateTime.Now.ToString("HH:mm");
        }

		private static byte[] GetMessageLength(int length)
		{
			int _base = byte.MaxValue + 1;
			byte[] result = new byte[SIZE_FIELD_LENGTH];

			if (length >= MAX_LENGTH)
			{
				throw new Exception("Message was too large");
			}

			for (int i = 0; i < SIZE_FIELD_LENGTH; i++)
			{
				int p = SIZE_FIELD_LENGTH - i - 1;
				int quotient = length / (int)Math.Pow(_base, p);
				result[i] = (byte)quotient;
				length = length % (int)Math.Pow(_base, p);
			}

			return result;
		}

		//Summarizes message length (except length field itself)
		public static int GetMessageLength(byte[] length)
		{
			int _base = byte.MaxValue + 1;
			int m = 1;
			int result = 0;
			for (int i = 1; i <= SIZE_FIELD_LENGTH; i++)
			{
				result += m * length[SIZE_FIELD_LENGTH - i];
				m *= _base;
			}
			return result;
		}
	}


}
