extends Node
class_name Main

@export var networking : SteamNetworking
@export var world: WorldState

signal game_started;
signal game_ended;

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	Steam.steamInit(true, 480);
	
	networking.playerSteamId = Steam.getSteamID();
	networking.playerSteamName = Steam.getPersonaName();
	
	Steam.requestLobbyList();
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta: float) -> void:
	Steam.run_callbacks();
	pass

func startGame():
	if not multiplayer.is_server():
		return
	print("Starting game..")
	
	for player in networking.players:
		load_world.rpc_id(player)
		load_player.rpc_id(player, player)
	pass
	
	
@rpc
func load_player(peerId: int):
	print("loading player..")
	set_multiplayer_authority(peerId)
	var packedPlayer: PackedScene = load("res://features/player/player.tscn")
	var playerScene: Node3D = packedPlayer.instantiate()
	playerScene.name = networking.playerSteamName + str(peerId)
	world.addPlayer(playerScene)
	spawn_player.rpc(networking.playerSteamName)
	game_started.emit()
	pass

@rpc("call_local")
func load_world():
	print("loading world..")
	var packedMap = load("res://features/world/testMap/testWorld.tscn")
	var mapScene = packedMap.instantiate()
	world.add_child(mapScene)
	pass

@rpc("any_peer")
func spawn_player(steamName: String):
	var senderId = multiplayer.get_remote_sender_id()
	var packedPlayer = load("res://features/player/player.tscn")
	var playerScene = packedPlayer.instantiate()
	playerScene.name = steamName + str(senderId)
	world.addPlayer(playerScene)
	pass
