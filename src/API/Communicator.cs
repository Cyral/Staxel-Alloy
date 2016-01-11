using Alloy.API.Entities;

namespace Alloy.API
{
    /// <summary>
    /// Reverse API to keep Staxel.dll dependency out of API.
    /// </summary>
    internal class Communicator
    {
        internal static event ChatEventHandler Chat;

        internal static event DisconnectEventHandler Disconnect;

        internal static void OnChat(Player player, string message) => Chat?.Invoke(player, message);
        internal static void OnDisconnect(Player player, string reason) => Disconnect?.Invoke(player, reason);

        internal delegate void ChatEventHandler(Player player, string message);

        internal delegate void DisconnectEventHandler(Player player, string reason);
    }
}