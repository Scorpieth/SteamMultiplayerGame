using Godot;
using SteamMultiplayer.features.networking;

namespace SteamMultiplayer.features.debugging;

public partial class DebugWindow : PanelContainer
{
	[Export] private ItemList _itemList;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SteamNetworking.Instance.PlayerListChanged += () =>
		{
			_itemList.Clear();
			foreach (var player in SteamNetworking.Instance.Players)
			{
				_itemList.AddItem(player.Value);
			}
		};
	}
}