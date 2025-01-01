using Godot;

namespace SteamMultiplayer.features.player;

public partial class Player : CharacterBody3D
{
	[Export] private MultiplayerSynchronizer _synchronizer;
	[Export]private float _speed = 5.0f;
	[Export]private PlayerCamera _camera;

	[Export] private int _multiplayerAuthority;

	public int MultiplayerAuthority
	{
		get => _multiplayerAuthority;
		set
		{
			_multiplayerAuthority = value;
			SetMultiplayerAuthority(value);
		}
	}
	
	public Vector3 StartPosition { get; set; }
	
	private PlayerInputs _playerInputs;

	public override void _Ready()
	{
		var isMultiplayerAuthority = IsMultiplayerAuthority();
		
		SetProcess(isMultiplayerAuthority);
		SetPhysicsProcess(isMultiplayerAuthority);

		GlobalPosition = StartPosition;
		
		if (!isMultiplayerAuthority)
		{
			return;
		}
		
		_playerInputs = new PlayerInputs(this);

		var main = GetTree().Root.GetNode<Node>("Main");
		main.Connect("player_teleport", new Callable(this, MethodName.OnPlayerTeleport));
	}

	public override void _Process(double delta)
	{
		_playerInputs.Handler();
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;
		
		LookAt(GetMousePosition3D());

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}
		
		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector3 direction = _playerInputs.CalculatedDirection;
		if (_playerInputs.IsMoving)
		{
			velocity.X = direction.X * _speed;
			velocity.Z = direction.Z * _speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, _speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, _speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
	
	private Vector3 GetMousePosition3D()
	{
		var targetPlane = new Plane(new(0, 1, 0), GlobalPosition.Y);
		var mousePosition = GetViewport().GetMousePosition();
		var camera = _camera;

		var rayStart = camera.ProjectRayOrigin(mousePosition);
		var rayEnd = rayStart + camera.ProjectRayNormal(mousePosition) * 2000;

		return targetPlane.IntersectsRay(rayStart, rayEnd) ?? Vector3.Zero;
	}

	private void OnPlayerTeleport(Vector3 newPosition)
	{
		GD.Print("Teleporting.. - ", Name);
		GD.Print("New pos: ", newPosition);
		GlobalPosition = newPosition;
	}
}