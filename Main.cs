using Godot;
using GodotSteam;
using SteamMultiplayer.features.networking;

namespace SteamMultiplayer;

public partial class Main : Node
{
	[Export] private SteamNetworking _steamNetworking;
	[Export] private Node3D _world;
	
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

		var packedPlayer = GD.Load<PackedScene>("res://features/player/player.tscn");
		var playerScene = packedPlayer.Instantiate<Player>();

		
		
		EmitSignal(SignalName.GameStarted);
	}
	
	
	[Rpc(CallLocal = true)]
	private void LoadWorld()
	{
		var packed = GD.Load<PackedScene>("res://features/world/testMap/testWorld.tscn");
		var testMap = packed.Instantiate<Node3D>();
		_world.AddChild(testMap);
	}
}