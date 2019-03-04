using Godot;
using System;

public class enemy : KinematicBody2D
{
    private int speed = 100;
		private int knockbackSpeed = 1000;
		private int _health = 70;
		private player _player;

    public override void _Ready()
    {
			// connect to area entered signal
			this.GetNode("Area2D").Connect("area_entered", this, nameof(OnAreaEntered));
			// get reference to player
			_player = (player)GetNode("../player");
    }

    public override void _Process(float delta)
    {
			if (_health <= 0)
			{
				((game)this.GetNode("../..")).EnemyDead();	
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
			if (area.GetGroups().Contains("attacks"))
			{
				float swordFlow = (float)_player.GetSwordFlow();
				switch (area.Name) {
					case "sword":
						_health -= (int)Math.Round(10.0f + 40.0f * swordFlow / 100.0f);
						break;
					case "spinsword":
						_health -= (int)Math.Round(5.0f + 20.0f * swordFlow / 100.0f);
						break;
					case "pulse":
						_health -= (int)Math.Round(1.0f + 9.0f * swordFlow / 100.0f);
						Knockback(_player);
						break;
					case "spray_pulse":
						_health -= (int)Math.Round(2.0f + 13.0f * swordFlow / 100.0f);
						Knockback(_player);
						break;
					case "dash_sword":
						_health -= (int)Math.Round(7.0f + 33.0f * swordFlow / 100.0f);
						break;
				}
			}
    }
	
	public void Knockback(player playerInstance)
	{
		Vector2 directionToPlayer = (playerInstance.Position - this.Position).Normalized();
		MoveAndSlide(-knockbackSpeed*directionToPlayer);
	}
}
