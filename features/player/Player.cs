using Godot;

namespace SteamMultiplayer.features.player;

public partial class Player : CharacterBody3D
{
	[Export] private MultiplayerSynchronizer _synchronizer;
	[Export]private float _speed = 5.0f;

	private PlayerCamera _camera;
	private PlayerInputs _playerInputs;

	public override void _Ready()
	{
		var isMultiplayerAuthority = IsMultiplayerAuthority();
		
		SetProcess(isMultiplayerAuthority);
		SetPhysicsProcess(isMultiplayerAuthority);

		_synchronizer.SetMultiplayerAuthority(peerId);
		
		if (!isMultiplayerAuthority)
		{
			return;
		}

		var packedCamera = GD.Load<PackedScene>("res://features/player/player_camera.tscn");
		_camera = packedCamera.Instantiate<PlayerCamera>();
		GetParent().AddChild(_camera);

		var peerId = Multiplayer.GetUniqueId();
		
		_camera.SetCurrent(true);
		_camera.SetPlayer(this);
		
		// Set up multiplayer authority for children nodes
		_camera.SetMultiplayerAuthority(peerId);
		
		_playerInputs = new PlayerInputs(this);
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
}