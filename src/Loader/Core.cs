using System;
using System.Collections.Generic;
using System.Linq;
using Alloy.API;
using Alloy.API.Entities;
using Plukit.Base;
using Staxel.Data;
using Staxel.Logic;
using Staxel.Server;
using Staxel.Tiles;

// ReSharper disable UnusedMember.Global

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

        public static Core Instance { get; private set; }

        public Core(ModHost host)
        {
            Host = host;
            Communicator = Host.Communicator;
            Instance = this;

            Connections = new Dictionary<Player, ClientServerConnection>();

            Communicator.Chat += (player, message) =>
            {
                var blob = BlobAllocator.Blob(true);
                blob.SetString("response", message);
                SendPacket(player, DataPacketKind.ConsoleResponse, blob);
            };

            Communicator.Disconnect += (player, reason) =>
            {
                // TODO: Make this better...
                Connections[player]._connection.Close();
            };
        }

        public static bool OnPlaceTile(Entity entity, Vector3I location, Tile tile)
        {
            if (Instance == null)
                return true;
            Console.WriteLine($"Tile placed at {location.X},{location.Y},{location.Z}.");
            return false;
        }

        public static bool OnServerPacket(ClientServerConnection connection, DataPacket packet)
        {
            if (Instance == null)
                return true;
            //Console.WriteLine($"Server Packet: {packet?.Kind} from {connection?.Credentials?.Username}");

            if (packet != null)
            {
                var blob = packet.Blob;
                var user = connection.Credentials;
                var sender = Instance.GetPlayer(connection);

                switch (packet.Kind)
                {
                    case DataPacketKind.HelloServer:
                        var player = new Player(user.Username, user.Uid);
                        Instance.Connections.Add(player, connection);
                        if (Instance.Host.Events.PlayerJoined.Invoke(new EventManager.PlayerJoinEventArgs(player)))
                            return false;
                        break;
                    case DataPacketKind.Disconnect:
                        Instance.Host.Events.PlayerQuit.Invoke(new EventManager.PlayerQuitEventArgs(sender));
                        Instance.Connections.Remove(sender);
                        break;
                    case DataPacketKind.ConsoleMessage:
                        if (Instance.Host.Events.Chat.Invoke(new EventManager.ChatEventArgs(sender,
                                blob.GetString("message"))))
                            return false;
                        break;
                }
            }
            return true;
        }

        private Player GetPlayer(ClientServerConnection connection)
        {
            return Connections.FirstOrDefault(c => c.Value == connection).Key;
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