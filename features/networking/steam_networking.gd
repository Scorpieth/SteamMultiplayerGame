extends Node
class_name SteamNetworking

@export var main: Main
@export var gui: Gui

var peer := SteamMultiplayerPeer.new();

var players : Dictionary = {};

var playerSteamName: String;
var playerSteamId: int;

var lobbyId: int;

signal player_list_changed;

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	gui.lobby_host_requested.connect(createSteamLobby)
	gui.lobby_play_requested.connect(_on_lobby_play_requested);
	
	Steam.lobby_joined.connect(_on_lobby_joined);
	Steam.lobby_created.connect(_on_lobby_created);
	
	multiplayer.peer_connected.connect(_on_peer_connected);
	multiplayer.peer_disconnected.connect(_on_peer_disconnected);
	pass # Replace with function body.

func createSteamSocketHost():
	print("Creating host..")
	peer.create_host(0)
	multiplayer.multiplayer_peer = peer
	players[multiplayer.get_unique_id()] = playerSteamName
	print("Host created..")
	pass

func connectSteamSocket(remoteSteamId):
	print("Creating client..")
	peer.create_client(remoteSteamId, 0)
	multiplayer.multiplayer_peer = peer
	print("Client created..")
	pass

func createSteamLobby(lobbyType: Steam.LobbyType, maxPlayers: int):
	Steam.createLobby(lobbyType, maxPlayers)
	pass

func _on_lobby_created(connection: int, id: int):
	print("Creatin lobby..")
	if connection != 1:
		print("Failed creating steam lobby")
		return
	Steam.setLobbyJoinable(id, true);
	Steam.setLobbyData(id, "name", playerSteamName + "'s Cool Lobby")
	Steam.allowP2PPacketRelay(true)
	print("Lobby created..")
	createSteamSocketHost()
	pass

func _on_lobby_joined(targetLobbyId, _permissions, _locked, response):
	var lobbyOwnerId = Steam.getLobbyOwner(targetLobbyId);
	print("Attempting to join steam lobby")
	
	if response != 1:
		print("Error joining steam lobby")
		return
	
	lobbyId = targetLobbyId;
	
	if lobbyOwnerId == Steam.getSteamID(): #No need to run logic below for host
		player_list_changed.emit()
		return
	
	connectSteamSocket(lobbyOwnerId);
	players[multiplayer.get_unique_id()] = playerSteamName
	player_list_changed.emit()
	pass

func _on_lobby_play_requested():
	main.start_game()
	pass

func _on_peer_connected(peerId):
	register_player.rpc_id(peerId, playerSteamName)
	pass

func _on_peer_disconnected():
	pass

@rpc("any_peer", "call_local")
func register_player(playerName: String):
	print("Registering player:", playerName)
	var remoteSenderId:= multiplayer.get_remote_sender_id()
	players[remoteSenderId] = playerName
	player_list_changed.emit()
	pass
