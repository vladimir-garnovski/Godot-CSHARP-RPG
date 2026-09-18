using Godot;
using System;

public partial class Player : CharacterBody3D
{
	// Movement

	public const float Speed = 10.0f;
	public const float JumpVelocity = 9.0f;

	// Mouse Motion

	private Vector2 _look = Vector2.Zero;
	[Export] private float mouseSensitivity = 0.00075f;

	[Export] private float MIN_BOUNDARY = -60;
	[Export] private float MAX_BOUNDARY = 10;

	[Export] private SpringArm3D springArm3D;

	[Export] private Node3D horizontalPivot;
	[Export] private Node3D verticalPivot;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }
	public override void _PhysicsProcess(double delta)
	{
		FrameCameraRotation();
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

	
		
		var direction = GetMovementDirection();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else // Decelerate
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_cancel"))
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
		if(Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			if( @event is InputEventMouseMotion)
			{
				_look += -(@event as InputEventMouseMotion).Relative * mouseSensitivity;

			}
		}
    }
	private Vector3 GetMovementDirection()
	{

		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
		var inputVector = new Vector3(inputDir.X, 0, inputDir.Y).Normalized();
		Vector3 direction = horizontalPivot.GlobalTransform.Basis *inputVector;

		return direction;
	
	}
	private void FrameCameraRotation()
	{
		horizontalPivot.RotateY(_look.X);
		verticalPivot.RotateX(_look.Y);
	
		verticalPivot.Rotation = new Vector3 (
			Mathf.Clamp(verticalPivot.Rotation.X,Mathf.DegToRad(MIN_BOUNDARY),Mathf.DegToRad(MAX_BOUNDARY)),
			verticalPivot.Rotation.Y,
			verticalPivot.Rotation.Z
		);

		springArm3D.GlobalTransform = verticalPivot.GlobalTransform;
		_look = Vector2.Zero;
	}
}
