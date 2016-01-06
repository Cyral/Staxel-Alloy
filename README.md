# Alloy
###[Staxel](http://playstaxel.com/) Modding API

Alloy is an API for developing code-based Staxel mods and plugins.

Currently nothing more than an experiement.

===

#### How does it work?
Alloy doesn't need to use or distribute any code from Staxel. It instead patches `Staxel.dll` (the bulk of the game's code) on the user's machine to include the mod loader. When Staxel is initialized, the mod loader will load mods which can interact with the game via the injected code in the patched version of Staxel.

The idea currently is to use the minimum amount of IL code injection possible. Everything should be handled by the mod loader/host and not directly through the Staxel assembly (as that is tricky).

===

**Patcher:**
Injects a few IL intructions as well as a public static field (`ModLoader`) into `Staxel.dll`, and initializes the mod loader when the game starts.


**Mod Loader:**
When the game starts, an instance of the mod loader is created, so all code related to mod loading is done through this separate assembly.

The mod loader also has a "mod host", which will serve as the communicator between the mods and Staxel.

For events (such as OnBlockPlaced), IL will need to be injected (for example, into the block place method), which will invoke an event in the mod host, which mods can subscribe to. 

For instructions (such as a mod wanting to add a chat message), we will need to do the opposite. Either by referencing Staxel (easier to code but adds a dependency), or using some reflection stuff to call Staxel methods from the mod host.

#### Roadmap:

When and if I have more time to work on this:

1. Identify functions in Staxel that would make sense for method calls to be injected to. (Add IL code to these methods to call the mod loader/host, which will in turn trigger events for the mods to respond to)
2. Design a proper API and simplified types found in staxel. (For example, design an block class that has a position, size, etc. This will keep Alloy's implementation separate from Staxel, so the only thing that can break is the IL code being injected.)
3. If this all works out and I really want to develop this, eventually make an installer that will help users patch their DLL, add some update system, etc.
