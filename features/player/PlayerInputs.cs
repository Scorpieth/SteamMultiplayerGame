using Godot;

namespace SteamMultiplayer.features.player;

public class PlayerInputs
{
    private Player _player;
    public Vector2 Direction { get; private set; }
    public bool IsMoving => !Direction.IsZeroApprox();
    
    public bool Interact => Input.IsActionJustPressed("Interact");
    
    /// <summary>
    /// Needs player as input to calculate moveDirection relative to player
    /// </summary>
    public PlayerInputs(Player player)
    {
        _player = player;
    }
    
    /// <summary>
    /// Call this in _process to handle active input processes
    /// </summary>
    public void Handler()
    {
        Direction = Input.GetVector("Left", "Right", "Up", "Down");
    }
    
    public Vector3 CalculatedDirection => (new Vector3(Direction.X, 0, Direction.Y)).Normalized();
}