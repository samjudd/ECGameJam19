using Godot;
using System;

public class game : Node
{
	PackedScene enemy = (PackedScene)ResourceLoader.Load("res://enemy.tscn");

	int _nEnemies = 0;

    public override void _Ready()
    {
        this.GetNode("chars/enemy_spawner").Connect("timeout", this, nameof(_on_enemy_spawner_timeout));
    }

    public override void _Process(float delta)
    {	
    }
	
	private void _on_enemy_spawner_timeout()
	{
		if (_nEnemies < 6)
		{		
			KinematicBody2D newEnemy = (KinematicBody2D)enemy.Instance();
			Vector2 spawnPosition = new Vector2(100, 100);
			newEnemy.Position = spawnPosition;
			this.GetNode("chars").AddChild(newEnemy);
			_nEnemies += 1;
		}
	}

	public void EnemyDead()
	{
		_nEnemies -= 1;
	}
}

