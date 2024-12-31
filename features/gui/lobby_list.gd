extends ScrollContainer
class_name LobbyList

@export var refreshButton: Button

var lobbies: Dictionary
var vBoxContainer: VBoxContainer
var lobbyRows: Array

var selectedRow: LobbyRow

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	refreshButton.pressed.connect(Steam.requestLobbyList)
	
	Steam.lobby_match_list.connect(
		func(lobbyIds):
			lobbies.clear()
			lobbyRows.clear()
			initLobbiesContainer()
			
			for id in lobbyIds:
				var lobbyName = Steam.getLobbyData(id, "name")
				if lobbyName:
					addItem(id, lobbyName)
				pass
			pass
	)
	
	pass # Replace with function body.

func initLobbiesContainer():
	if vBoxContainer != null:
		vBoxContainer.queue_free()
	vBoxContainer = VBoxContainer.new()
	add_child(vBoxContainer)
	pass

func addItem(id: int, lobbyName: String):
	lobbies[id] = lobbyName
	var row = LobbyRow.new()
	row.setLobbyDetails(id, lobbyName)
	lobbyRows.append(row)
	vBoxContainer.add_child(row)
	
	row.lobby_selected.connect(
		func(lobbyId: int):
			if selectedRow:
				selectedRow.setSelected(false)
			selectedRow = lobbyRows.filter(func(x: LobbyRow): return x.id == lobbyId)[0]
			selectedRow.setSelected(true)
			pass
	)
	pass
