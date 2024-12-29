using Godot;
using System;
using GodotSteam;

public partial class LobbyList : ItemList
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Steam.LobbyMatchList += (items) =>
		{
			foreach (var item in items)
			{
				GD.Print("Lobby Item:", item);
				AddItem(Steam.GetLobbyData((ulong)item, "name"));
			}
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}

public record LobbyListItem(string name, string mode);
