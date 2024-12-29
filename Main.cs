using Godot;
using GodotSteam;
using SteamMultiplayer.features.networking;

namespace SteamMultiplayer;

public partial class Main : Node
{
	[Export] private SteamNetworking _steamNetworking;
	[Export] private World _world;
	
	public static Main Instance { get; private set; }

	[Signal] public delegate void GameStartedEventHandler();
	[Signal] public delegate void GameEndedEventHandler();
	
	public override void _Ready()
	{
		Instance = this;
		
		var steamInit = Steam.SteamInit(480);
		GD.Print($"Steam Initialized: {steamInit.Status}");
		
		if (!Steam.IsSteamRunning())
		{
			GD.PrintErr("Steam is not running");
		}
		
		_steamNetworking.PlayerSteamId = Steam.GetSteamID();
		_steamNetworking.PlayerSteamName = Steam.GetFriendPersonaName(_steamNetworking.PlayerSteamId);
	}
	
	public override void _Process(double delta)
	{
		Steam.RunCallbacks();
	}

	public void StartGame()
	{
		if (!Multiplayer.IsServer())
		{
			return;
		}
		GD.Print("Starting game..");



		foreach (var playerData in _steamNetworking.Players)
		{
			RpcId(playerData.Key, MethodName.LoadWorld);
			RpcId(playerData.Key, MethodName.LoadPlayer, playerData.Key);
		}
		
		EmitSignal(SignalName.GameStarted);
	}
	
	[Rpc(CallLocal = true)]
	private void LoadWorld()
	{
		var packed = GD.Load<PackedScene>("res://features/world/testMap/testWorld.tscn");
		var testMap = packed.Instantiate<Node3D>();
		_world.AddChild(testMap);
	}

	[Rpc(CallLocal = true)]
	private void LoadPlayer(int peerId)
	{
		SetMultiplayerAuthority(peerId);
		var packedPlayer = GD.Load<PackedScene>("res://features/player/player.tscn");
		var playerScene = packedPlayer.Instantiate<Player>();
		playerScene.SetName($"{_steamNetworking.PlayerSteamName}-{peerId.ToString()}");
		Rpc(MethodName.SpawnPlayer, _steamNetworking.PlayerSteamName);
		_world.AddPlayer(playerScene);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void SetAuthority(int peerId)
	{
		SetMultiplayerAuthority(peerId);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void SpawnPlayer(string steamName)
	{
		var senderId = Multiplayer.GetRemoteSenderId();
		var packedPlayer = GD.Load<PackedScene>("res://features/player/player.tscn");
		var playerScene = packedPlayer.Instantiate<Player>();
		playerScene.SetName($"{steamName}-{senderId}");
		_world.AddPlayer(playerScene);
	}
}