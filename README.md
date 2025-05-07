### Summary
Basic Template for Steam Multiplayer.

This setup currently has a demo scene for third person (top-view) 3d. You can switch out all the World implementations to your own, this is primarily meant for 3D and has not been tested for 2D.

This comes packed with a very basic lobby implementation.

There is no strict architectural decisions made, but it does utilise signals since it helps with keeping things decoupled. I might update this with more opinionated choices in the future.

### But it uses the Godot .Net??
Yeah, this is built ontop of the Mono godot build, so it has support for C#, and it also currently has a Player built in C#.
You can use this to make a pure GdScript project as well.

Recommended way is to simply pull all the gdscript content out of this project and just paste it in your own project. Make sure to install the required addons (GodotSteam and SteamMultiplayerPeer).

Might also work to convert / remove all the .cs files and remove the csproj and sln files, remove the .godot and then start up the project in the pure gdscript game engine.
I've personally used this template in pure Gdscript proejcts but I run it in the mono game engine.

### OBS
Currently the leave button is not implemented, I should probably get to that at some point.. Soon™
