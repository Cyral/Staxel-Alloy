using System.Collections.Generic;
using Alloy.API;
using Alloy.API.Entities;
using Plukit.Base;
using Staxel.Data;
using Staxel.Server;

namespace Alloy.Loader
{
    /// <summary>
    /// Handle messaging between Staxel and the API.
    /// </summary>
    public class Core
    {
        internal ModHost Host { get; }

        internal Communicator Communicator { get; private set; }

        internal Dictionary<Player, ClientServerConnection> Connections { get; }

        public Core(ModHost host)
        {
            Host = host;
            Communicator = Host.Communicator;

            Connections = new Dictionary<Player, ClientServerConnection>();

            Communicator.Chat += (player, message) =>
            {
                var blob = BlobAllocator.Blob(true);
                blob.SetString("response", message);
                SendPacket(player, DataPacketKind.ConsoleResponse, blob);
            };
        }

        public static bool OnServerPacket(ClientServerConnection connection, DataPacket packet)
        {
            //Console.WriteLine($"Server Packet: {packet?.Kind} from {connection?.Credentials?.Username}");

            if (packet != null)
            {
                var blob = packet.Blob;
                var user = connection.Credentials;
            }
            return true;
        }

        private void SendPacket(Player player, DataPacketKind type, Blob blob)
        {
            Connections[player].SendPacket(new DataPacket(ServerClockNow(), type, blob));
            blob.Deallocate();
        }

        private static Timestep ServerClockNow()
        {
            return ServerMainLoop._clock.Now();
        }
    }
}