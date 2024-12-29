using Godot;
using System;
using GodotSteam;

public partial class LobbyMenu : PanelContainer
{
	[Export] private Button _hostButton;
	[Export] private Button _joinButton;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_hostButton.Pressed += () =>
		{
			Steam.CreateLobby(Steam.LobbyType.FriendsOnly, 5);
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
