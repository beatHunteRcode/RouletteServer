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
        private bool hasCroupier = false;
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
                IPEndPoint TCPEndPoint = new IPEndPoint(IPAddress.Any, _port);

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
                    do
                    {
                        byte[] buffer = new byte[Utilities.TEMP_BUFFER_SIZE];
                        if (buffer.Length < Utilities.TEMP_BUFFER_SIZE)
                        {
                            buffer = new byte[Utilities.TEMP_BUFFER_SIZE];
                        }
                        byte[] messageBytes = ReceiveBytes(listener, buffer);
                        if (messageBytes.Length > 0) HandleMessage(listener, messageBytes);
                    } while (listener.Connected);
                }
                catch (Exception e)
                {
                    //Console.WriteLine(e.ToString());
                    DisconnectUser(listener);
                    //Console.ReadKey();
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
                    User enteringUser = new User(
                        messageRegistration.UserType,
                        messageRegistration.Username,
                        messageRegistration.Password,
                        Thread.CurrentThread,
                        listener);
                    bool isConnected = false;
                    if (enteringUser.UserType == UserType.CROUPIER && hasCroupier) break;
                    foreach (User u in UsersList)
                    {
                            if (u.Username == enteringUser.Username)
                            {
                                isConnected = true;
                                if (u.Password == enteringUser.Password)
                                {
                                    if (u.UserType == UserType.CROUPIER) hasCroupier = true;
                                    u.Socket = listener;
                                    SendMessage(listener, new MessageAcknowledgement());
                                    Console.WriteLine("[" + Utilities.GetCurrentTime() + "] " + enteringUser.Username + " has been connected (as " + enteringUser.UserType.ToString() + ")");
                                    break;
                                }
                                break;
                            }
                    }
                    if (!isConnected)
                    {
                        UsersList.Add(enteringUser);
                        if (enteringUser.UserType == UserType.CROUPIER) hasCroupier = true;
                        SendMessage(listener, new MessageAcknowledgement());
                        Console.WriteLine("[" + Utilities.GetCurrentTime() + "] " + enteringUser.Username + " has been connected (as " + enteringUser.UserType.ToString() + ")");
                    }
                    break;
                case MessageType.BETS_ANNOUNCEMENT:
                    break;
                case MessageType.MAKING_BET:
                    User bettingPlayer = new User();
                    MessageMakingBet messageMakingBet = new MessageMakingBet(messageBytes);
                    foreach (User u in UsersList)
                    {
                        if (u.Socket == listener)
                        {
                            u.isBetted = true;
                            u.Bet = messageMakingBet.Bet;
                            bettingPlayer = u;
                        }
                    }
                    SendMessage(listener, new MessageAcknowledgement());

                    if (bettingPlayer.Bet.BetType != BetType.NUMBER)
                    {
                        Console.WriteLine(
                            "[" + Utilities.GetCurrentTime() + "] " + bettingPlayer.Username + " made a bet: " +
                            bettingPlayer.Bet.Sum + "$ on " + bettingPlayer.Bet.BetType.ToString());
                    }
                    else
                    {
                        Console.WriteLine(
                            "[" + Utilities.GetCurrentTime() + "] " + bettingPlayer.Username + " made a bet: " +
                            bettingPlayer.Bet.Sum + "$ on number " + bettingPlayer.Bet.Number);
                    }

                    break;
                case MessageType.DRAWING:
                    PlayersList.Clear();
                    foreach (User u in UsersList)
                    {
                        if (u.UserType == UserType.PLAYER)
                        {
                            if (u.isBetted == true) PlayersList.Add(u);
                        }
                    }
                    if (PlayersList.Count == UsersList.Count - 1)
                    {
                        MessageBetsAnnouncement messageBetsAnnouncement = new MessageBetsAnnouncement(PlayersList);
                        BroadcastToAllPlayersMessage(messageBetsAnnouncement);
                        EndBets();
                        Console.WriteLine("AND THE NUMBER IS...");
                        Thread.Sleep(2000);
                        CurrentNumber = RollRoulette();
                        Console.WriteLine(CurrentNumber);
                        MessageInforming messageInforming = new MessageInforming();
                        messageInforming.Number = CurrentNumber;
                        BroadcastToAllPlayersMessage(messageInforming);
                    }
                    break;
                case MessageType.INFORM_CLIENT:
                    break;
                case MessageType.DISCONNECT:
                    MessageDisconnect messageDisconnect = new MessageDisconnect(messageBytes);
                    SendMessage(listener, messageDisconnect);
                    listener.Disconnect(false);
                    User disconnectedUser = new User();
                    foreach (User u in UsersList)
                    {
                        if (u.Socket == listener) disconnectedUser = u;
                    }
                    Console.WriteLine("[" + Utilities.GetCurrentTime() + "] " + disconnectedUser.Username + " (" + disconnectedUser.UserType.ToString().ToUpper() + ") has been disconnected");
                    break;
            }
        }

        public void DisconnectUser(Socket listener)
        {
            try
            {
                User userToDisconnect = null;
                foreach (User u in UsersList)
                {
                    if (u.Socket == listener) userToDisconnect = u;
                }
                if (userToDisconnect != null)
                {
                    //BroadcastDisconnectMessage(listener, new MessageDisconnect());
                    Console.WriteLine($"[{Utilities.GetCurrentTime()}] {userToDisconnect.Username} ({ userToDisconnect.UserType.ToString().ToUpper()}) has been disconnected");
                    UsersList.Remove(userToDisconnect);
                    if (userToDisconnect.UserType == UserType.CROUPIER) hasCroupier = false;
                    listenersList.Remove(listener);
                    listener.Shutdown(SocketShutdown.Both);
                }
                else
                {
                    Console.WriteLine($"[{Utilities.GetCurrentTime()}] Somebody is trying to connect, but he dropped out...");
                    listenersList.Remove(listener);
                    listener.Shutdown(SocketShutdown.Both);
                }
            }
            catch
            {
                User disconnectedUser = new User();
                foreach (User user in UsersList)
                {
                    if (user.Socket == listener) disconnectedUser = user;
                }
                Console.WriteLine($"[{Utilities.GetCurrentTime()}] {disconnectedUser.Username} ({disconnectedUser.UserType.ToString().ToUpper()}) has been disconnected due to error");
                if (disconnectedUser.UserType == UserType.CROUPIER) hasCroupier = false;
                listenersList.Remove(listener);
                listener.Shutdown(SocketShutdown.Both);
            }
        }

        private void BroadcastToAllPlayersMessage(Message message)
        {
            foreach (User player in PlayersList)
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
            MessageHeader header = new MessageHeader(sendingBytes.Length);
            sendingBytes = header.GetBytes().Concat(sendingBytes).ToArray();
            listener.Send(sendingBytes);
        }

        private byte[] ReceiveBytes(Socket socket, byte[] buffer)
        {
            int bufferSize = socket.Receive(buffer);
            byte[] headerBytes = new byte[Utilities.HEADER_SIZE];
            Array.Copy(buffer, 0, headerBytes, 0, headerBytes.Length);
            byte[] mainMessageBytes = new byte[Utilities.GetMessageLength(headerBytes)];
            Array.Copy(buffer, 3, mainMessageBytes, 0, mainMessageBytes.Length);
            return mainMessageBytes;
        }

        private byte RollRoulette()
        {
            Random rnd = new Random();
            return (byte)rnd.Next(0, 37);
        }

        public void EndBets()
        {
            Console.WriteLine("------------BETS END------------");
            foreach(User player in PlayersList)
            {
                if (player.Bet.BetType != BetType.NUMBER)
                {
                    Console.WriteLine($"{player.Username}: {player.Bet.Sum}$ on {player.Bet.BetType.ToString().ToUpper()}");
                }
                else
                {
                    Console.WriteLine($"{player.Username}: {player.Bet.Sum}$ on number {player.Bet.Number}");
                }
            }
            Console.WriteLine("--------------------------------");
        }
    }
}
