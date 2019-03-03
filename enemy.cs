using Godot;
using System;

public class enemy : KinematicBody2D
{
    private int speed = 100;
	  private int _health = 70;

    public override void _Ready()
    {
      // connect to area entered signal
      this.GetNode("Area2D").Connect("area_entered", this, nameof(OnAreaEntered));
    }

    public override void _Process(float delta)
    {
		if (_health <= 0) {
			QueueFree();
			return;
		}
		KinematicBody2D player = (KinematicBody2D)GetParent().GetNode("player");
		Vector2 directionToPlayer = (player.Position - this.Position).Normalized();
		MoveAndSlide(speed*directionToPlayer);

		ColorRect healthRect = (ColorRect)GetNode("health");
		healthRect.SetSize(new Vector2(_health, healthRect.GetSize().y));
    }

    private void OnAreaEntered(Area2D area)
    {
		if (area.GetGroups().Contains("attacks")) {
			switch (area.Name) {
				case "sword":
					_health -= 10;
					break;
				case "spinsword":
					_health -= 5;
					break;
				case "pulse":
					_health -= 1;
					break;
			}
		}
    }
}
