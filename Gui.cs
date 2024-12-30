using Godot;
using SteamMultiplayer.features.gui;

namespace SteamMultiplayer;

public partial class Gui : Control
{
	[Export] private Main _mainInstance;
	
	[Export] private bool _debugMode;
	[Export] private LobbyMenu _lobbyMenu;
	
	[Signal] public delegate void LobbyHostRequestedEventHandler();
	[Signal] public delegate void LobbyPlayRequestedEventHandler();
	[Signal] public delegate void LobbyLeaveRequestedEventHandler();
	
	public override void _Ready()
	{
		_lobbyMenu.HostButton.Pressed += () => EmitSignal(SignalName.LobbyHostRequested);
		_lobbyMenu.PlayButton.Pressed += () => EmitSignal(SignalName.LobbyPlayRequested);
		_lobbyMenu.LeaveButton.Pressed += () => EmitSignal(SignalName.LobbyLeaveRequested);
		_mainInstance.GameStarted += () => _lobbyMenu.Visible = false;
		_mainInstance.GameEnded += () => _lobbyMenu.Visible = true;
		
		GetNode<Control>("DebugWindow").Visible = _debugMode;
		
		_mainInstance.GameStarted += () =>
		{
			_lobbyMenu.ShowLobbies(false);
			_lobbyMenu.ShowWaitingRoom(false);
		};
	}
}