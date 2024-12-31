extends ItemList
class_name LobbyPlayersList

func refreshPlayers(lobbyId: int):
	if !lobbyId:
		return
	clear()
		
	var playerCount = Steam.getNumLobbyMembers(lobbyId)
	
	for num in playerCount:
		var steamId = Steam.getLobbyMemberByIndex(lobbyId, num);
		var steamName = Steam.getFriendPersonaName(steamId)
		add_item(steamName)
	pass
