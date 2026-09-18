using Godot;
using System;

public partial class SmoothCameraArm : SpringArm3D
{
	[Export] private Node3D target; // VerticalPivot
	[Export] private float decay = 10.0f;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		GlobalTransform = GlobalTransform.InterpolateWith(
			target.GlobalTransform,
			1.0f - Mathf.Exp(-decay * (float)delta)
			);
	}
}
