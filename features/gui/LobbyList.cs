using Godot;
using GodotSteam;

namespace SteamMultiplayer.features.gui;

public partial class LobbyList : ItemList
{
	[Export] private Button _refreshButton;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_refreshButton.Pressed += Steam.RequestLobbyList;
		
		Steam.LobbyMatchList += (items) =>
		{
			foreach (var item in items)
			{
				var lobbyName = Steam.GetLobbyData((ulong)item, "name");
				GD.Print("Lobby Item:", item, lobbyName);
				if (string.IsNullOrEmpty(lobbyName))
				{
					continue;
				}
				
				AddItem(lobbyName);
			}
		};
	}
}