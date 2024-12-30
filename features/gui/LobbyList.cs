using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotSteam;

namespace SteamMultiplayer.features.gui;

public partial class LobbyList : ScrollContainer
{
	[Export] private Button _refreshButton;

	private Dictionary<ulong, string> _lobbyList = new();

	private VBoxContainer _vBoxContainer;

	private List<LobbyRow> _lobbyRows = new();

	public LobbyRow SelectedRow { get; private set; }

	public override void _Ready()
	{
		_refreshButton.Pressed += Steam.RequestLobbyList;
		
		Steam.LobbyMatchList += lobbyIds =>
		{
			_lobbyList.Clear();
			_lobbyRows.Clear();
			InitVboxContainer();
			
			foreach (var lobbyId in lobbyIds)
			{
				var lobbyName = Steam.GetLobbyData((ulong)lobbyId, "name");
				GD.Print("Lobby Item:", lobbyId, lobbyName);
				if (string.IsNullOrEmpty(lobbyName))
				{
					continue;
				}
				AddItem((ulong)lobbyId, lobbyName);
			}
		};
	}

	private void AddItem(ulong lobbyId, string lobbyName)
	{
		_lobbyList.Add(lobbyId, lobbyName);
		var lobbyRow = new LobbyRow();
		lobbyRow.SetLobbyDetails(lobbyId, lobbyName);
		lobbyRow.LobbySelected += id =>
		{
			if (SelectedRow is not null)
			{
				SelectedRow.SetSelected(false);
			}
			SelectedRow = _lobbyRows.First(x => x.LobbyDetails.LobbyId == id);
			SelectedRow.SetSelected(true);
		};
		_lobbyRows.Add(lobbyRow);
		_vBoxContainer.AddChild(lobbyRow);
	}

	private void InitVboxContainer()
	{
		_vBoxContainer?.QueueFree();
		_vBoxContainer = null;
		_vBoxContainer = new VBoxContainer();
		AddChild(_vBoxContainer);
	}
}