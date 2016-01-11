namespace Alloy.API.Entities
{
    public class Player : IEntity
    {
        public void SendMessage(string message)
        {
            Communicator.OnChat(this, message);
        }
    }
}