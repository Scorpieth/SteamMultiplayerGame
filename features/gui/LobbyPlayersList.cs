using Godot;
using System.Collections.Generic;
using System.Linq;
using GodotSteam;
using SteamMultiplayer.features.networking;

public partial class LobbyPlayersList : ItemList
{
	private List<LobbyPlayer> _lobbyPlayers = new();
	
	private ulong? _steamLobbyId;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SteamNetworking.Instance.PlayerListChanged += RefreshPlayers;
	}

	private void RefreshPlayers()
	{
		_steamLobbyId ??= SteamNetworking.Instance.SteamLobby.LobbyId;

		if (_steamLobbyId is null)
		{
			return;
		}
		
		_lobbyPlayers.Clear();
		var playerCount = Steam.GetNumLobbyMembers(_steamLobbyId.Value);

		_lobbyPlayers = Enumerable.Range(0, playerCount).Select(x =>
		{
			var steamId = Steam.GetLobbyMemberByIndex(_steamLobbyId.Value, x);
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
