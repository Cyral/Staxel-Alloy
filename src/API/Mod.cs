namespace Alloy.API
{
    public abstract class Mod : ModData
    {
        /// <summary>
        /// Gets or sets if the mod is enabled and should be ran.
        /// </summary>
        public bool IsEnabled { get; internal set; }

        public ModHost Host { get; set; }

        public EventManager Events => Host.Events;

        public Mod(ModHost host)
        {
            Host = host;
            IsEnabled = true;
        }

        /// <summary>
        /// Performed when the mod is loaded.
        /// </summary>
        /// <remarks>
        /// Initialization logic should be put here and NOT in the constructor, as this method will be called if the mod is
        /// reloaded.
        /// </remarks>
        public abstract void Load();

        /// <summary>
        /// Performed when the mod is unloaded.
        /// </summary>
        /// <remarks>
        /// It is up to the mod author to properly unload/reset everything. If the mod is not reset completely, undesired
        /// side effects may occur.
        /// For this reason, mod reloading is not completely supported. The client or server must be completely restarted.
        /// </remarks>
        public abstract void Unload();
    }
}