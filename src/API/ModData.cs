using System;
using System.Collections.Generic;

namespace Alloy.API
{
    /// <summary>
    /// Mod metadata.
    /// </summary>
    public class ModData
    {
        /// <summary>
        /// The (preferably short) display name of the mod.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The unique identification name of this mod.
        /// This name must not be used by any other mod.
        /// </summary>
        /// <example>
        /// Examples:
        /// com.pyratron.mods.testmod
        /// mymod-123
        /// MyName.Mymod
        /// </example>
        public string Identifier { get; internal set; }

        /// <summary>
        /// The description of the mod.
        /// </summary>
        public string Description { get; internal set; }

        /// <summary>
        /// The name(s) of the author(s) of the mod.
        /// </summary>
        public List<string> Authors { get; internal set; }

        /// <summary>
        /// The mod identifiers that this mod requires to be installed to run.
        /// </summary>
        public List<string> Dependencies { get; internal set; }

        /// <summary>
        /// The version of this mod.
        /// </summary>
        /// <remarks>
        /// This value is used for mod updating and must be increased with each release.
        /// </remarks>
        public Version Version { get; internal set; }

        /// <summary>
        /// Full name of the main type loaded.
        /// </summary>
        internal string MainTypeName { get; set; }

        /// <summary>
        /// Path to this mod's root folder.
        /// </summary>
        public string Path { get; internal set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((ModData) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Version.GetHashCode() * 397) ^ Name.GetHashCode();
            }
        }

        /// <summary>
        /// Returns a readable string containing the mod's name, unique identifier, and version.
        /// </summary>
        public string GetInfoString()
        {
            return $"{Name} ({Identifier}) v{Version}";
        }

        private bool Equals(ModData other)
        {
            return Version == other.Version && string.Equals(Identifier, other.Identifier);
        }
    }
}