using System.Collections.Generic;
using System.Threading;
using Godot;
using GodotSteam;

namespace SteamMultiplayer.features.networking;

public partial class SteamNetworking : Node
{
	[Export] private Gui _gui;

	private SteamMultiplayerPeer peer = new();

	public SteamLobby SteamLobby { get; private set; }

	public Dictionary<int, string> Players { get; } = new();

	public string PlayerSteamName { get; set; }
	public ulong PlayerSteamId { get; set; }
	public static SteamNetworking Instance { get; private set; }
	
	public ulong LobbyId { get; set; }

	[Signal] public delegate void HostCreatedEventHandler();
	[Signal] public delegate void PlayerListChangedEventHandler();
	
	public override void _Ready()
	{
		Instance = this;
		
		_gui.LobbyHostRequested += () => CreateSteamLobby(Steam.LobbyType.Public, 5);
		_gui.LobbyPlayRequested += () =>
		{
			Main.Instance.StartGame();
			
		};

		Steam.LobbyJoined += (lobbyId, permissions, locked, response) => OnLobbyJoined(lobbyId, response);
		
		Multiplayer.PeerConnected += id =>
		{
			RpcId(id, MethodName.RegisterPlayer, PlayerSteamName);
			GD.Print("Peer connected");
		};
		Multiplayer.ConnectedToServer += () => GD.Print("I connected to server");
	}

	private void OnLobbyJoined(ulong lobbyId, long response)
	{
		var lobbyOwnerId = Steam.GetLobbyOwner(lobbyId);
		GD.Print("Attempting to join Lobby, response: ", response);
		
		if ((int)response != (int)ChatRoomResponse.ChatRoomEnterResponseSuccess)
		{
			GD.PrintErr(((ChatRoomResponse)response).ToString());
			return;
		}

		LobbyId = lobbyId;
		
		// Since this is the host we've already created the host peer and connected
		if (lobbyOwnerId == PlayerSteamId)
		{
			EmitSignal(SignalName.PlayerListChanged);
			return;
		}
		
		ConnectSteamSocket(lobbyOwnerId);
		Players.Add(Multiplayer.GetUniqueId(), PlayerSteamName);
		EmitSignal(SignalName.PlayerListChanged);
	}

	private void CreateSteamSocketHost()
	{
		peer = new SteamMultiplayerPeer();
		var err = peer.CreateHost(0);
		GD.Print("Creating Host.. status: ", err.ToString());
		
		Multiplayer.SetMultiplayerPeer(peer);
		Players.Add(Multiplayer.GetUniqueId(), PlayerSteamName);
		GD.Print("Steam socket host created");
	}

	private void ConnectSteamSocket(ulong steamId)
	{
		peer = new SteamMultiplayerPeer();
		var err = peer.CreateClient(steamId, 0);
		
		GD.Print(err.ToString());
		
		Multiplayer.SetMultiplayerPeer(peer);
		GD.Print("Steam socket connected");
	}

	public void CreateSteamLobby(Steam.LobbyType lobbyType, long maxPlayers)
	{
		Steam.CreateLobby(lobbyType, maxPlayers);
		SteamLobby = new SteamLobby();
		SteamLobby.SteamLobbyInitialized += CreateSteamSocketHost;
	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void RegisterPlayer(string playerName)
	{
		var remoteSenderId = Multiplayer.GetRemoteSenderId();
		Players.Add(remoteSenderId, playerName);
		EmitSignal(SignalName.PlayerListChanged);
	}
}

public enum ChatRoomResponse
{
	ChatRoomEnterResponseSuccess =	1,
	ChatRoomEnterResponseDoesntExist =	2,
	ChatRoomEnterResponseNotAllowed = 3,
	ChatRoomEnterResponseFull =	4,
	ChatRoomEnterResponseError = 5,
	ChatRoomEnterResponseBanned = 6,
	ChatRoomEnterResponseLimited = 7,
	ChatRoomEnterResponseClanDisabled = 8,
	ChatRoomEnterResponseCommunityBan = 9,
	ChatRoomEnterResponseMemberBlockedYou = 10,
	ChatRoomEnterResponseYouBlockedMember = 11,
}
