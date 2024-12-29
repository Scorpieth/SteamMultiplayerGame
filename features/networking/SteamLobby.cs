using System;
using Godot;
using GodotSteam;

namespace SteamMultiplayer.features.networking;

public class SteamLobby
{
    private ulong _lobbyId;
    public ulong LobbyId => _lobbyId;

    public Action SteamLobbyInitialized;
    
    public SteamLobby()
    {
        Steam.LobbyCreated += (connect, id) =>
        {
            if (connect != 1)
            {
                GD.Print("Failed creating steam lobby");
            }
			
            _lobbyId = id;
            Steam.SetLobbyJoinable(_lobbyId, true);
            Steam.SetLobbyData(_lobbyId, "name", $"{Steam.GetPersonaName()}'s Lobby");
			
            // Allows for fallback p2p communication to be relayed through steam to bypass Nat and firewall issues
            Steam.AllowP2PPacketRelay(true);
			
            GD.Print($"Lobby Created: {id}");
            SteamLobbyInitialized?.Invoke();
        };
    }
}