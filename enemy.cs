using Godot;
using System;

public class enemy : KinematicBody2D
{
    // Member variables here, example:
    private int speed = 100;
    // private string b = "textvar";

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here
    }

    public override void _Process(float delta)
    {
		KinematicBody2D player = (KinematicBody2D)GetParent().GetNode("player");
		Vector2 directionToPlayer = (player.Position - this.Position).Normalized();
		MoveAndSlide(speed*directionToPlayer);
    }
}
