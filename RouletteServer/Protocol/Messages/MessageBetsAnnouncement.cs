using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouletteServer.Protocol.Messages
{
    class MessageBetsAnnouncement : Message
    {
        public MessageType MessageType { get; set; }
        public byte UsersInGame { get; set; }

        public List<User> PlayersList;

        public MessageBetsAnnouncement(List<User> playersList)
        {
            MessageType = MessageType.BETS_ANNOUNCEMENT;
            UsersInGame = (byte)playersList.Count;
            PlayersList = playersList;
        }

        public override byte[] GetBytes()
        {
            byte[] messageTypeBytes = new byte[] { (byte)MessageType };
            byte[] usersInGameBytes = new byte[] { UsersInGame };
            byte[] playersBytes = new byte[0];
            foreach (User player in PlayersList)
            {
                playersBytes = playersBytes.Concat(player.GetBytes()).ToArray();
            }

            return messageTypeBytes.Concat(usersInGameBytes).Concat(playersBytes).ToArray();
        }
    }
}
