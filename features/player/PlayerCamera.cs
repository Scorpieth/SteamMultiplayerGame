using Godot;

namespace SteamMultiplayer.features.player;

public partial class PlayerCamera : Camera3D
{
	[Export] private Player _player;
	
	public override void _PhysicsProcess(double delta)
	{
		GlobalPosition = new(_player.GlobalPosition.X, GlobalPosition.Y, _player.GlobalPosition.Z + 4.5f);
	}
}