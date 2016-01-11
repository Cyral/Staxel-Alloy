namespace Alloy.API
{
    public class ModHost
    {
        internal Communicator Communicator { get; private set; }

        public ModHost()
        {
            Communicator = new Communicator();
        }
    }
}