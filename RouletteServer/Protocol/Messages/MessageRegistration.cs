using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouletteServer.Protocol.Messages
{
    class MessageRegistration : Message
    {
        public MessageType MessageType { get; set; }
        public UserType UserType { get; set; }
        public byte UsernameLength { get; set; }
        public String Username { get; set; }
        public byte PasswordLength { get; set; }
        public String Password { get; set; }

        private byte[] UsernameArr;
        public byte[] PasswordArr;

        public MessageRegistration(byte[] bytes)
        {
            MessageType = (MessageType)bytes[0];
            UserType = (UserType)bytes[1];
            UsernameLength = (byte)(bytes[2] * Utilities.ENCODING_SIZE);
            UsernameArr = new byte[UsernameLength];
            Array.Copy(bytes, 3, UsernameArr, 0, UsernameLength);
            Username = Encoding.Unicode.GetString(UsernameArr);
            PasswordLength = (byte)(bytes[3 + UsernameLength] * Utilities.ENCODING_SIZE);
            PasswordArr = new byte[PasswordLength];
            Array.Copy(bytes, 3 + UsernameLength + 1, PasswordArr, 0, PasswordLength);
            Password = Encoding.Unicode.GetString(PasswordArr);
        }

        public override byte[] GetBytes()
        {
            throw new NotImplementedException();
        }
    }
}
