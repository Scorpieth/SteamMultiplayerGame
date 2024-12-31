extends Node
class_name Main

@export var networking : SteamNetworking
@export var world: WorldState

signal game_started;
signal game_ended;
signal player_teleport(position: Vector3);

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
	var spawnLocation := 0;

	var packedMap: PackedScene = load("res://features/world/testMap/testWorld.tscn")
	var mapScene: TestWorld = packedMap.instantiate()
	world.add_child(mapScene)

	var teleportPosition: Marker3D = mapScene.spawnLocations[spawnLocation];	

	for player in networking.players:
		if player != multiplayer.get_unique_id():
			load_world.rpc_id(player)
			
		load_player.rpc_id(player, player)
		teleport_player.rpc_id(player, teleportPosition)
	pass
	
	
@rpc("call_local")
func load_player(peerId: int):
	print("Loading player..")
	var packedPlayer: PackedScene = load("res://features/player/player.tscn")
	var playerScene: Node3D = packedPlayer.instantiate()
	playerScene.name = str(peerId)
	world.addPlayer(playerScene)
	spawn_player.rpc(networking.playerSteamName)
	game_started.emit()
	print("Player loaded..")
	pass

@rpc("call_local")
func load_world():
	print("Loading world..")
	var packedMap = load("res://features/world/testMap/testWorld.tscn")
	var mapScene = packedMap.instantiate()
	world.add_child(mapScene)
	print("World loaded..")
	pass

@rpc("any_peer")
func spawn_player(steamName: String):
	print("Spawning remote Player: ", steamName)
	var senderId = multiplayer.get_remote_sender_id()
	var packedPlayer = load("res://features/player/player.tscn")
	var playerScene = packedPlayer.instantiate()
	playerScene.name = str(senderId)
	world.addPlayer(playerScene)
	pass
	
@rpc("call_local")
func teleport_player(position: Vector3):
	print("Teleporting player..")
	player_teleport.emit(position)
	print("Player teleported..")
	pass
