

using Godot;
using System;

public partial class SmoothCameraArm : SpringArm3D
{
	[Export] private Node3D target; // VerticalPivot
	[Export] private float decay = 20.0f;

	public override void _PhysicsProcess(double delta)
	{
		GlobalTransform = GlobalTransform.InterpolateWith(
			target.GlobalTransform,
			1.0f - Mathf.Exp(-decay * (float)delta)
			);
	}
}


