using Alloy.API.Entities;

namespace Alloy.API
{
    /// <summary>
    /// Provides hooks/events for the API.
    /// </summary>
    public class EventManager
    {
        /// <summary>
        /// When a player joins the server.
        /// </summary>
        public Event<PlayerJoinEventArgs> PlayerJoined { get; } = new Event<PlayerJoinEventArgs>();

        /// <summary>
        /// When a player exits the server.
        /// </summary>
        public Event<PlayerQuitEventArgs> PlayerQuit { get; } = new Event<PlayerQuitEventArgs>();

        /// <summary>
        /// When a chat message is received.
        /// </summary>
        public Event<ChatEventArgs> Chat { get; } = new Event<ChatEventArgs>();

        public class PlayerJoinEventArgs : AlloyEventArgs
        {
            public Player Player { get; private set; }

            public PlayerJoinEventArgs(Player player)
            {
                Player = player;
            }
        }

        public class PlayerQuitEventArgs : AlloyEventArgs
        {
            public Player Player { get; private set; }

            public PlayerQuitEventArgs(Player player)
            {
                Player = player;
            }
        }

        public class ChatEventArgs : AlloyEventArgs
        {
            public Player Sender { get; set; }
            public string Message { get; set; }

            public ChatEventArgs(Player sender, string message)
            {
                Sender = sender;
                Message = message;
            }
        }
    }
}