using Godot;

namespace SteamMultiplayer.features.gui;

public partial class LobbyRow : HBoxContainer
{
    public LobbyDetails LobbyDetails { get; private set; }
    private Label _lobbyNameLabel;
    
    private StyleBoxFlat _style = new StyleBoxFlat();
    
    [Signal] public delegate void LobbySelectedEventHandler(ulong lobbyId);

    public override void _Ready()
    {
        MouseFilter = MouseFilterEnum.Stop;
        _style.SetBorderColor(Colors.Black);
    }

    public void SetLobbyDetails(ulong lobbyId, string lobbyName)
    {
        LobbyDetails = new LobbyDetails(lobbyId, lobbyName);
        _lobbyNameLabel = new Label();
        AddChild(_lobbyNameLabel);
        _lobbyNameLabel.Text = lobbyName;
    }

    public void SetSelected(bool selected)
    {
        var width = selected ? 3 : 0;
        
        _style.SetBorderWidthAll(width);
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mb && mb.ButtonIndex == MouseButton.Left)
        {
            EmitSignal(SignalName.LobbySelected);
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