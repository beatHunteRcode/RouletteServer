using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RouletteServer.Protocol
{
    class User
    {
        public UserType UserType { get; set; }
        public Bet Bet { get; set; }
        public byte UsernameLength { get; set; }
        public String Username { get; set; }
        public byte PasswordLength { get; set; }
        public String Password { get; set; }
        public Thread Thread;
        public Socket Socket;

        public bool isBetted = false;

        public User(UserType userType, String username, String password, Thread t, Socket s)
        {
            UserType = userType;
            Username = username;
            Password = password;
            UsernameLength = (byte)username.Length;
            PasswordLength = (byte)password.Length;
            Thread = t;
            Socket = s;
        }

        public User()
        {

        }

        public byte[] GetBytes()
        {
            byte[] betBytes = Bet.GetBytes();
            byte[] usernameLengthBytes = new byte[] { UsernameLength };
            byte[] usernameBytes = Encoding.Unicode.GetBytes(Username);

            return betBytes.Concat(usernameLengthBytes).Concat(usernameBytes).ToArray();
        }
    }
}
