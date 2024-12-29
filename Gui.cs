using Godot;
using SteamMultiplayer.features.gui;

namespace SteamMultiplayer;

public partial class Gui : Control
{
	[Export] LobbyMenu _lobbyMenu;
	
	[Signal] public delegate void LobbyJoinRequestedEventHandler();
	[Signal] public delegate void LobbyHostRequestedEventHandler();
	[Signal] public delegate void LobbyPlayRequestedEventHandler();
	[Signal] public delegate void LobbyLeaveRequestedEventHandler();
	
	public override void _Ready()
	{
		_lobbyMenu.HostButton.Pressed += () =>
		{
			EmitSignal(SignalName.LobbyHostRequested);
			_lobbyMenu.ShowLobbies(false);
			_lobbyMenu.ShowWaitingRoom();
		};

		_lobbyMenu.JoinButton.Pressed += () =>
		{
			EmitSignal(SignalName.LobbyJoinRequested);
			_lobbyMenu.ShowLobbies(false);
			_lobbyMenu.ShowWaitingRoom();
		};

		_lobbyMenu.PlayButton.Pressed += () =>
		{
			EmitSignal(SignalName.LobbyPlayRequested);
		};
		
		_lobbyMenu.LeaveButton.Pressed += () =>
		{
			EmitSignal(SignalName.LobbyLeaveRequested);
		};
	}
}