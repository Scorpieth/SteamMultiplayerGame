extends PanelContainer
class_name LobbyMenu

@export var hostButton: Button
@export var joinButton: Button
@export var playButton: Button
@export var leaveButton: Button

@export var lobbies: Control
@export var lobbyList: LobbyList
@export var waitingRoom: Control

@export var lobbyPlayersList: LobbyPlayersList

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	joinButton.pressed.connect(
		func():
			Steam.joinLobby(lobbyList.selectedRow.id)
			lobbies.hide()
			waitingRoom.show()
	)
	hostButton.pressed.connect(
		func():
			lobbies.hide()
			waitingRoom.show()
	)
	
	lobbies.show()
	waitingRoom.hide()
	pass # Replace with function body.
