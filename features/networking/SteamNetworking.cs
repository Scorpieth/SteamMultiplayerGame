using Godot;
using GodotSteam;

namespace SteamMultiplayer.features.networking;

public partial class SteamNetworking : Node
{
	[Export] private Gui _gui;
	
	private SteamLobby _steamLobby;

	[Signal] public delegate void HostCreatedEventHandler();
	
	public override void _Ready()
	{
		_gui.LobbyJoinRequested += () => { };
		_gui.LobbyHostRequested += () => CreateSteamLobby(Steam.LobbyType.Public, 5);
	}

	private void CreateHost()
	{
		var peer = new SteamMultiplayerPeer();
		peer.CreateHost(0);
		Multiplayer.SetMultiplayerPeer(peer);
		GD.Print("Host peer created");
	}

	public void CreateSteamLobby(Steam.LobbyType lobbyType, long maxPlayers)
	{
		Steam.CreateLobby(lobbyType, maxPlayers);
		_steamLobby = new SteamLobby();
		_steamLobby.SteamLobbyInitialized += CreateHost;
	}
}