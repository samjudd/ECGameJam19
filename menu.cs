    using Godot;
using System;

public class menu : MarginContainer
{
    public override void _Ready()
    {
        ((Button)this.GetNode("VBoxContainer/HBoxContainer/MarginContainer/TutorialButton")).Connect("pressed", this, nameof(OnTutorialButtonPressed));
        ((Button)this.GetNode("VBoxContainer/HBoxContainer/MarginContainer2/CreditsButton")).Connect("pressed", this, nameof(OnCreditsButtonPressed));
    }

    public override void _Process(float delta)
    {
        // Called every frame. Delta is time since last frame.
        // Update game logic here.
		if (Input.IsActionPressed("start_game"))
        {
			GetTree().ChangeScene("res://game.tscn");
        }
    }

    private void OnTutorialButtonPressed()
    {
        GetTree().ChangeScene("res://tutorial.tscn");
    }

    private void OnCreditsButtonPressed()
    {
        GetTree().ChangeScene("res://credits.tscn");
    }
}
