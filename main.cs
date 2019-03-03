using Godot;
using System;

public class main : Node2D
{
	PackedScene enemy = (PackedScene)ResourceLoader.Load("res://enemy.tscn");

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here
        
    }

    public override void _Process(float delta)
    {
        // Called every frame. Delta is time since last frame.
        // Update game logic here.
		
    }
	
	private void _on_enemy_spawner_timeout()
	{
		KinematicBody2D newEnemy = (KinematicBody2D)enemy.Instance();
		Vector2 spawnPosition = new Vector2(100, 100);
		newEnemy.Position = spawnPosition;
		AddChild(newEnemy);
	}
}
