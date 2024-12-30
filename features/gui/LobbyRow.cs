using Godot;

namespace SteamMultiplayer.features.gui;

public partial class LobbyRow : HBoxContainer
{
    public LobbyDetails LobbyDetails { get; private set; }
    private Label _lobbyNameLabel;
    
    [Signal] public delegate void LobbySelectedEventHandler(ulong lobbyId);

    public override void _Ready()
    {
        MouseFilter = MouseFilterEnum.Stop;
    }

    public void SetLobbyDetails(ulong lobbyId, string lobbyName)
    {
        LobbyDetails = new LobbyDetails(lobbyId, lobbyName);
        _lobbyNameLabel = new Label();
        _lobbyNameLabel.Name = lobbyName;
        AddChild(_lobbyNameLabel);
        _lobbyNameLabel.Text = lobbyName;
    }

    public void SetSelected(bool selected)
    {
        _lobbyNameLabel.AddThemeColorOverride("font_color", selected ? Colors.Red : Colors.White);
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mb && mb.ButtonIndex == MouseButton.Left && mb.IsReleased())
        {
            GD.Print("Mouse Button Pressed");
            EmitSignal(SignalName.LobbySelected, LobbyDetails.LobbyId);
        }
    }
}

public class LobbyDetails
{
    public ulong LobbyId { get; private set; }
    public string LobbyName { get; private set; }

    public LobbyDetails(ulong lobbyId, string lobbyName)
    {
        LobbyId = lobbyId;
        LobbyName = lobbyName;
    }
}