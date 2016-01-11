namespace Alloy.API.Entities
{
    public class Player : IEntity
    {
        public string Name { get; set; }
        public string UID { get; set; }

        public Player(string name, string uid)
        {
            Name = name;
            UID = uid;
        }

        public void SendMessage(string message)
        {
            Communicator.OnChat(this, message);
        }
    }
}