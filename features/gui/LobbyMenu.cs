using Godot;
using GodotSteam;

namespace SteamMultiplayer.features.gui;

public partial class LobbyMenu : PanelContainer
{
	[Export] public Button HostButton;
	[Export] public Button JoinButton;
	[Export] public Button PlayButton;
	[Export] public Button LeaveButton;

	[Export] private Control _lobbies;
	[Export] private LobbyList _lobbiesList; 
	[Export] private Control _waitingRoom;

	public void ShowLobbies(bool show = true) => _lobbies.Visible = show;
	public void ShowWaitingRoom(bool show = true) => _waitingRoom.Visible = show;

	public override void _Ready()
	{
		Main.Instance.GameStarted += () =>
		{
			ShowLobbies(false);
			ShowWaitingRoom(false);
		};
		JoinButton.Pressed += () =>
		{
			Steam.JoinLobby(_lobbiesList.SelectedRow.LobbyDetails.LobbyId);
			ShowLobbies(false);
			ShowWaitingRoom();
		};
		HostButton.Pressed += () =>
		{
			ShowLobbies(false);
			ShowWaitingRoom();
		};
		
		ShowWaitingRoom(false);
		ShowLobbies();
	}
}