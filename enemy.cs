using Godot;
using System;

public class enemy : KinematicBody2D
{
    // Member variables here, example:
    private int speed = 100;
    // private string b = "textvar";

    public override void _Ready()
    {
      // connect to area entered signal
      this.GetNode("Area2D").Connect("area_entered", this, nameof(OnAreaEntered));
    }

    public override void _Process(float delta)
    {
		KinematicBody2D player = (KinematicBody2D)GetParent().GetNode("player");
		Vector2 directionToPlayer = (player.Position - this.Position).Normalized();
		MoveAndSlide(speed*directionToPlayer);
    }

    private void OnAreaEntered(Area2D area)
    {
      area.GetGroups();
      if ("attacks" )
      GD.Print("Attack Registered");
    }
}
