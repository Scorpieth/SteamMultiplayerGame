using System.Collections.Generic;
using Godot;
using GodotSteam;

namespace SteamMultiplayer.features.networking;

public partial class SteamNetworking : Node
{
	[Export] private Gui _gui;
	
	private SteamLobby _steamLobby;

	public Dictionary<int, string> Players { get; } = new();

	public string PlayerSteamName { get; set; }
	public ulong PlayerSteamId { get; set; }
	public static SteamNetworking Instance { get; private set; }

	[Signal] public delegate void HostCreatedEventHandler();
	[Signal] public delegate void PlayerListChangedInLobbyEventHandler(ulong lobbyId);
	[Signal] public delegate void PlayerListChangedEventHandler();
	
	public override void _Ready()
	{
		Instance = this;
		
		_gui.LobbyJoinRequested += () => { };
		_gui.LobbyHostRequested += () => CreateSteamLobby(Steam.LobbyType.Public, 5);

		Steam.LobbyJoined += (lobbyId, permissions, locked, response) => OnLobbyJoined(lobbyId);
		Multiplayer.PeerConnected += (id) =>
		{
			RpcId(id, MethodName.RegisterPlayer, PlayerSteamName);
		};
	}

	private void OnLobbyJoined(ulong lobbyId)
	{
		var lobbyOwnerId = Steam.GetLobbyOwner(lobbyId);

		// Since this is the host we've already created the host peer and connected
		if (lobbyOwnerId == PlayerSteamId)
		{
			EmitSignal(SignalName.PlayerListChangedInLobby, lobbyId);
			return;
		}
			
		ConnectSteamSocket(lobbyOwnerId);
		Rpc(MethodName.RegisterPlayer, PlayerSteamName);
		Players.Add(Multiplayer.GetUniqueId(), PlayerSteamName);
			
		EmitSignal(SignalName.PlayerListChangedInLobby, lobbyId);
	}

	private void CreateSteamSocket()
	{
		var peer = new SteamMultiplayerPeer();
		peer.CreateHost(0);
		Multiplayer.SetMultiplayerPeer(peer);
		GD.Print("Steam socket created");
	}

	private void ConnectSteamSocket(ulong steamId)
	{
		var peer = new SteamMultiplayerPeer();
		peer.CreateClient(steamId, 0);
		Multiplayer.SetMultiplayerPeer(peer);
		GD.Print("Steam socket connected");
	}

	public void CreateSteamLobby(Steam.LobbyType lobbyType, long maxPlayers)
	{
		Steam.CreateLobby(lobbyType, maxPlayers);
		_steamLobby = new SteamLobby();
		_steamLobby.SteamLobbyInitialized += CreateSteamSocket;
	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void RegisterPlayer(string playerName)
	{
		var remoteSenderId = Multiplayer.GetRemoteSenderId();
		Players.Add(remoteSenderId, playerName);
		EmitSignal(SignalName.PlayerListChanged);
	}
}