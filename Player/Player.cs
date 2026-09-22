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
	[Export] private Node3D rigPivot;
	[Export] private Rig rig;
	[Export] private float animationDecay = 20.0f;
	[Export] private float ATTACK_MOVE_SPEED = 3.0f;

	private Vector3 attackDirection = Vector3.Zero;
	

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
		rig.UpdateAnimationTree(direction);
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
			LookTowardDirection(direction,(float)delta);
		}
		else // Decelerate
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		HandleSlashingPhysicsFrame((float)delta);
		MoveAndSlide();

		
	}
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_cancel"))	// For releasing the mouse
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
		if(Input.MouseMode == Input.MouseModeEnum.Captured) // For looking around (mouse rotate cam)
		{
			if( @event is InputEventMouseMotion)
			{
				_look += -(@event as InputEventMouseMotion).Relative * mouseSensitivity;

			}
		}
		if (rig.isIdle())
		{
			if (@event.IsActionPressed("click"))
			{
				SlashAttack();
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

		_look = Vector2.Zero;
	}
	private void LookTowardDirection(Vector3 direction, float delta)
	{
		var targetTransform = rigPivot.GlobalTransform.LookingAt(
			rigPivot.GlobalPosition + direction,
			Vector3.Up,
			true
		);
		//rigPivot.GlobalTransform = new Transform3D(targetTransform.Basis, rigPivot.GlobalTransform.Origin); //rigPivot.GlobalTransform.Basis = targetTransform.Basis;
		rigPivot.GlobalTransform  = rigPivot.GlobalTransform.InterpolateWith (
			targetTransform,
			1.0f - Mathf.Exp(-animationDecay * (float)delta)
		);
	}
	public void SlashAttack()
    {
        rig.Travel("Slash");
		attackDirection = GetMovementDirection();
		if (attackDirection.IsZeroApprox())
		{
			attackDirection = rig.GlobalBasis * new Vector3(0,0,1);
		}
    }
	private void HandleSlashingPhysicsFrame(float delta)
	{
		if (!rig.isSlashing())
			return;

		Vector3 velocity = this.Velocity;

		velocity.X = attackDirection.X * ATTACK_MOVE_SPEED;
		velocity.Z = attackDirection.Z * ATTACK_MOVE_SPEED;

		this.Velocity = velocity;

		LookTowardDirection(attackDirection,delta);
	}
}
