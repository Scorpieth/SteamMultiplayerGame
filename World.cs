using Godot;

public partial class World : Node3D
{
	[Export] private Node _playersContainer;
	public void AddPlayer(Player player)
	{
		_playersContainer.AddChild(player);
	}
}
