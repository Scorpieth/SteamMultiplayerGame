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

func start_game():
	if not multiplayer.is_server():
		return
	print("Starting game..")
	var spawnLocation := 0;

	var packedMap: PackedScene = load("res://features/world/testMap/testWorld.tscn")
	var mapScene: TestWorld = packedMap.instantiate()
	world.add_child(mapScene)

	var spawnPositions: Array[Marker3D] = mapScene.spawnLocations;	

	for player in networking.players:
		if player != multiplayer.get_unique_id():
			load_world.rpc_id(player)
			
		var startPos := spawnPositions[spawnLocation].global_position;
		
		load_player.rpc_id(player, player, startPos)
		teleport_player.rpc_id(player, startPos)
		spawnLocation += 1
	pass

@rpc("call_local")
func load_player(peerId: int, startPos: Vector3):
	print("Loading player..")
	var packedPlayer: PackedScene = load("res://features/player/player.tscn")
	var playerScene: Node3D = packedPlayer.instantiate()
	playerScene.name = str(peerId)
	playerScene.MultiplayerAuthority = peerId
	playerScene.StartPosition = startPos
	world.addPlayer(playerScene)
	
	spawn_player.rpc(networking.playerSteamName, startPos)
	
	game_started.emit()
	print("Player loaded..")
	pass

@rpc("call_local")
func load_world():
	print("Loading world..")
	var packedMap: PackedScene = load("res://features/world/testMap/testWorld.tscn")
	var mapScene: Node3D = packedMap.instantiate()
	world.add_child(mapScene)
	print("World loaded..")
	pass

@rpc("any_peer")
func spawn_player(steamName: String, startPos: Vector3):
	print("Spawning remote Player: ", steamName)
	var senderId := multiplayer.get_remote_sender_id()
	var packedPlayer: PackedScene = load("res://features/player/player.tscn")
	var playerScene: Node3D = packedPlayer.instantiate()
	playerScene.name = str(senderId)
	playerScene.MultiplayerAuthority = senderId
	playerScene.StartPosition = startPos
	world.addPlayer(playerScene)
	pass
	
@rpc("call_local")
func teleport_player(position: Vector3):
	print("Teleporting player..")
	player_teleport.emit(position)
	print("Player teleported..")
	pass
