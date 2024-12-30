using Godot;

namespace SteamMultiplayer.features.player;

public partial class PlayerCamera : Camera3D
{
	public Player Player { get; private set; }
	
	public override void _PhysicsProcess(double delta)
	{
		GlobalPosition = new(Player.GlobalPosition.X, GlobalPosition.Y, Player.GlobalPosition.Z + 4.5f);
	}

	public void SetPlayer(Player player)
	{
		Player = player;
	}
}