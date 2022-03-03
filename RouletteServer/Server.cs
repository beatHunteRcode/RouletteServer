using RouletteServer.Protocol.Messages;
using RouletteServer.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RouletteServer
{
    public class Server
    {

        readonly string _ip;
        readonly int _port;
        const int listenersNumb = 10;
        private static readonly List<Socket> listenersList = new List<Socket>();
        private static readonly List<User> UsersList = new List<User>();
        private static readonly List<User> PlayersList = new List<User>();
        private static byte CurrentNumber = 100;

        public Server(string ip, int port)
        {
            _ip = ip;
            _port = port;
        }

        public void start()
        {
            Run();
        }


        public void Run()
        {
            Console.WriteLine("Launching server...");
            try
            {
                IPEndPoint TCPEndPoint = new IPEndPoint(IPAddress.Parse(_ip), _port);

                Socket TCPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                TCPSocket.Bind(TCPEndPoint);
                Console.WriteLine("Server has successfully launched");
                TCPSocket.Listen(listenersNumb);
                Console.WriteLine("Server is running");
                while (true)
                {
                    var listener = TCPSocket.Accept();
                    listenersList.Add(listener);
                    CreateUserThread(listener);
                }
            }
            finally
            {
                Console.ReadKey();
            }
        }

        private void CreateUserThread(Socket listener)
        {
            Thread userThread = new Thread(() =>
            {
                try
                {
                    byte[] buffer = new byte[Utilities.TEMP_BUFFER_SIZE];
                    do
                    {
                        if (buffer.Length < Utilities.TEMP_BUFFER_SIZE)
                        {
                            buffer = new byte[Utilities.TEMP_BUFFER_SIZE];
                        }
                        byte[] messageBytes = ReceiveBytes(listener, buffer);
                        HandleMessage(listener, messageBytes);
                    } while (listener.Connected);
                }
                catch
                {
                    DisconnectUser(listener);
                }
            });
            userThread.Start();
        }

        private void HandleMessage(Socket listener, byte[] messageBytes)
        {
            MessageType messageType = (MessageType)messageBytes[0];
            switch (messageType)
            {
                case MessageType.REGISTRATION:
                    MessageRegistration messageRegistration = new MessageRegistration(messageBytes);
                    User user = new User(
                        messageRegistration.UserType,
                        messageRegistration.Username,
                        messageRegistration.Password,
                        Thread.CurrentThread,
                        listener);
                    UsersList.Add(user);
                    SendMessage(listener, new MessageAcknowledgement());
                    break;
                case MessageType.BETS_ANNOUNCEMENT:
                    PlayersList.Clear();
                    foreach (User u in UsersList)
                    {
                        if (u.UserType == UserType.PLAYER) PlayersList.Add(u);
                    }
                    MessageBetsAnnouncement messageBetsAnnouncement = new MessageBetsAnnouncement(PlayersList);
                    BroadcastBestAnnounceMessage(messageBetsAnnouncement);
                    break;
                case MessageType.MAKING_BET:
                    MessageMakingBet messageMakingBet = new MessageMakingBet(messageBytes);
                    foreach (User u in UsersList)
                    {
                        if (u.Socket == listener) u.Bet = messageMakingBet.Bet;
                    }
                    SendMessage(listener, new MessageAcknowledgement());
                    break;
                case MessageType.DRAWING:
                    CurrentNumber = RollRoulette();
                    break;
                case MessageType.INFORM_CLIENT:
                    MessageInforming messageInforming = new MessageInforming();
                    messageInforming.Number = CurrentNumber;
                    SendMessage(listener, messageInforming);
                    break;
                case MessageType.DISCONNECT:
                    MessageDisconnect messageDisconnect = new MessageDisconnect(messageBytes);
                    SendMessage(listener, messageDisconnect);
                    listener.Disconnect(false);
                    break;
            }
        }

        public void DisconnectUser(Socket listener)
        {
            User userToDisconnect = null;
            foreach (User u in UsersList)
            {
                if (u.Socket == listener) userToDisconnect = u;
            }
            BroadcastDisconnectMessage(listener, new MessageDisconnect());
            Console.WriteLine($"[{Utilities.GetCurrentTime()}] {userToDisconnect.Username} has been disconnected");
            UsersList.Remove(userToDisconnect);
            listenersList.Remove(listener);
            listener.Shutdown(SocketShutdown.Both);
        }

        private void BroadcastBestAnnounceMessage(MessageBetsAnnouncement message)
        {
            foreach (User player in message.PlayersList)
            {
                SendMessage(player.Socket, message);
            }
        }

        private void BroadcastDisconnectMessage(Socket currentListener, Message message)
        {
            foreach (Socket listener in listenersList)
            {
                if (!listener.Equals(currentListener)) SendMessage(listener, message);
            }
        }

        private void SendMessage(Socket listener, Message message)
        {
            byte[] sendingBytes = message.GetBytes();
            listener.Send(sendingBytes);
        }

        private byte[] ReceiveBytes(Socket socket, byte[] buffer)
        {
            int bufferSize = socket.Receive(buffer);
            byte[] headerBytes = new byte[Utilities.HEADER_SIZE];
            Array.Copy(buffer, 0, headerBytes, 0, headerBytes.Length);
            byte[] mainMessageBytes = new byte[Utilities.GetMessageLength(headerBytes)];
            return mainMessageBytes;
        }

        private byte RollRoulette()
        {
            Random rnd = new Random();
            return (byte)rnd.Next(0, 37);
        }
    }
}
