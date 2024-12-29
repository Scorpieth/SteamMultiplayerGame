using Godot;
using System;
using GodotSteam;

public partial class Main : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var steamInit = Steam.SteamInit(480);
		GD.Print($"Steam Initialized: {steamInit.Status}");
		
		if (!Steam.IsSteamRunning())
		{
			GD.PrintErr("Steam is not running");
		}
		
		var steamId = Steam.GetSteamID();
		var name = Steam.GetFriendPersonaName(steamId);	
        
		GD.Print("Your Steam Name: " + name);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
