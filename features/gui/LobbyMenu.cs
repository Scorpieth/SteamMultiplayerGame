using System;
using Godot;
using GodotSteam;

public partial class LobbyMenu : PanelContainer
{
	[Export] private Button _hostButton;
	[Export] private Button _joinButton;

	private ulong _lobbyId;
	
	public override void _Ready()
	{
		_hostButton.Pressed += () =>
		{
			Steam.CreateLobby(Steam.LobbyType.Public, 5);
		};

		_joinButton.Pressed += () =>
		{
			Steam.JoinLobby(_lobbyId);
		};

		Steam.LobbyCreated += (connect, id) =>
		{
			if (connect != 1)
			{
				GD.Print("Failed creating steam lobby");
			}
			
			_lobbyId = id;
			Steam.SetLobbyJoinable(_lobbyId, true);
			Steam.SetLobbyData(_lobbyId, "name", "TestLobby");
			Steam.SetLobbyData(_lobbyId, "mode", "TestLobbyMode");

			// Allows for fallback p2p communication to be relayed through steam to bypass Nat and firewall issues
			Steam.AllowP2PPacketRelay(true);
			
			GD.Print($"Lobby Created: {id}");
			GD.Print("Requesting lobbies for lobby list");
			Steam.RequestLobbyList();
		};
	}
}
