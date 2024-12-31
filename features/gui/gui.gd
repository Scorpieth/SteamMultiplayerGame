extends Control
class_name Gui

@export var main: Main
@export var debugMode: bool

@export var networking: SteamNetworking

@export var lobbyMenu: LobbyMenu

signal lobby_host_requested
signal lobby_play_requested
signal lobby_leave_requested

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	lobbyMenu.hostButton.pressed.connect(
		func():
			lobby_host_requested.emit(Steam.LobbyType.LOBBY_TYPE_PUBLIC, 5)
	)
	lobbyMenu.playButton.pressed.connect(
		func():
			lobby_play_requested.emit()
	)
	lobbyMenu.leaveButton.pressed.connect(
		func():
			lobby_leave_requested.emit()
	)
	
	main.game_started.connect(
		func():
			lobbyMenu.hide()
	)
	main.game_ended.connect(
		func():
			lobbyMenu.show()
	)
	
	networking.player_list_changed.connect(
		func():
			lobbyMenu.lobbyPlayersList.refreshPlayers(networking.lobbyId)
	)
	
	pass
