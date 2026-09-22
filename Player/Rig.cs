using Godot;
using System;


// parameters/MoveSpace/blend_position
public partial class Rig : Node3D
{
    private string RUN_PATH = "parameters/MoveSpace/blend_position";
    [Export] private AnimationTree animationTree;

    private float runWeightTarget = -1.0f;

    [Export] private float ANIMATION_SPEED = 10.0f;

    private AnimationNodeStateMachinePlayback playback; // @onready
  

    public override void _Ready()
    {
        playback = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");
    }
    public override void _PhysicsProcess(double delta)
    {
        
        animationTree.Set(RUN_PATH, Mathf.MoveToward(
            (float)animationTree.Get(RUN_PATH),
            runWeightTarget,
            (float)delta * ANIMATION_SPEED
        ));
    }
    public void UpdateAnimationTree(Vector3 direction)
    {
        if(direction.IsZeroApprox())
        {
            //animationTree[RUN_PATH] = -1.0; // idle
            
            runWeightTarget = -1.0f;
        }
        else
        {
        
             runWeightTarget = 1.0f;
        }
            
    }
    public void Travel(string animationName)
    {
        playback.Travel(animationName);
    }
    public bool isIdle()
    {
        return playback.GetCurrentNode().ToString() == "MoveSpace"; // CHECK THIS STRINGNAME and STRING COMPARISON
    }
    public bool isSlashing()
    {
        return playback.GetCurrentNode().ToString() == "Slash"; // CHECK THIS STRINGNAME and STRING COMPARISON
    }

}
