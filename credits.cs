using Godot;
using System;

public class credits : MarginContainer
{

    public override void _Ready()
    {
        ((Button)this.GetNode("VBoxContainer/Button")).Connect("pressed", this, nameof(OnBackButtonPressed));
    }

    private void OnBackButtonPressed()
    {
        GetTree().ChangeScene("res://menu.tscn");
    }
}

