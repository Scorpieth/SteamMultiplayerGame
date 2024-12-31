extends HBoxContainer
class_name LobbyRow

var id: int
var lobbyName: String

var lobbyNameLabel: Label

signal lobby_selected(lobbyId: int)

func _ready():
	mouse_filter = MouseFilter.MOUSE_FILTER_STOP
	pass

func setLobbyDetails(lobbyId: int, lobby_Name: String):
	id = lobbyId
	lobbyName = lobby_Name
	lobbyNameLabel = Label.new()
	lobbyNameLabel.name = lobbyName
	lobbyNameLabel.text = lobbyName
	add_child(lobbyNameLabel)
	pass

func setSelected(selected: bool):
	lobbyNameLabel.add_theme_color_override("font_color", Color.ORANGE_RED if selected else Color.WHITE)
	pass

func _gui_input(event: InputEvent) -> void:
	if event is InputEventMouseButton && event.button_index == MOUSE_BUTTON_LEFT && event.is_released():
		lobby_selected.emit(id)
	pass
