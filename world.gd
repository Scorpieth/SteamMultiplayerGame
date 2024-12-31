extends Node3D
class_name WorldState

@export var playersContainer: Node

# Called when the node enters the scene tree for the first time.
func addPlayer(player: Node3D):
	playersContainer.add_child(player)
	pass
