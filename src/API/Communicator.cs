using Alloy.API.Entities;

namespace Alloy.API
{
    /// <summary>
    /// Reverse API to keep Staxel.dll dependency out of API.
    /// </summary>
    internal class Communicator
    {
        internal static event ChatEventHandler Chat;
        internal static void OnChat(Player player, string message) => Chat?.Invoke(player, message);

        internal delegate void ChatEventHandler(Player player, string message);
    }
}