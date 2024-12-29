using Godot;
using System.Collections.Generic;
using System.Linq;
using GodotSteam;
using SteamMultiplayer.features.networking;

public partial class LobbyPlayersList : ItemList
{
	private List<LobbyPlayer> _lobbyPlayers = new();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SteamNetworking.Instance.PlayerListChangedInLobby += RefreshPlayers;
	}

	private void RefreshPlayers(ulong lobbyId)
	{
		_lobbyPlayers.Clear();
		var playerCount = Steam.GetNumLobbyMembers(lobbyId);

		_lobbyPlayers = Enumerable.Range(0, playerCount).Select(x =>
		{
			var steamId = Steam.GetLobbyMemberByIndex(lobbyId, x);
			var steamName = Steam.GetFriendPersonaName(steamId);

			AddItem(steamName);
			
			return new LobbyPlayer(steamName, steamId);
		}).ToList();
	}
}

public class LobbyPlayer
{
	private readonly string _name;
	private readonly ulong _id;

	public LobbyPlayer(string name, ulong id)
	{
		_name = name;
		_id = id;
	}
};
