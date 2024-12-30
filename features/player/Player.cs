using Godot;
using System;
using SteamMultiplayer.features.player;

public partial class Player : CharacterBody3D
{
	[Export] private Camera3D _camera;

	private PlayerInputs _playerInputs;
	
	[Export]private float _speed = 5.0f;

	public override void _Ready()
	{
		var isMultiplayerAuthority = IsMultiplayerAuthority();
		
		SetProcess(isMultiplayerAuthority);
		SetPhysicsProcess(isMultiplayerAuthority);
		
		_camera.SetCurrent(isMultiplayerAuthority);

		if (!isMultiplayerAuthority)
		{
			return;
		}

		_playerInputs = new PlayerInputs(this);
	}

	public override void _Process(double delta)
	{
		_playerInputs.Handler();
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

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
}
