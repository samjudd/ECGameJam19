using Godot;
using System;

public class GUI : MarginContainer
{
    private Tween _tween;
    private Label _health_bar_label;
    private TextureProgress _health_bar;

    public override void _Ready()
    {
        // get tween
        _tween = (Tween) GetNode("Tween");

        // Get Health Bar Stuff
        _health_bar = (TextureProgress) GetNode("Bars/HealthBar/LifeBar/Gauge");
        _health_bar_label = (Label) GetNode("Bars/HealthBar/LifeBar/MarginContainer/Background/Number");

        // need to get all the flow bars as well and initialize them to 0 here

        // Set Bar to Player's Max Health
        var player = (player) GetNode("../chars/player");
        _health_bar.MaxValue = player.MaxHealth;
        UpdateHealth(player.MaxHealth);

        // connect to health changed on player
        player.Connect("HealthChanged", this, nameof(onPlayerHealthChanged));
    }

    public void UpdateHealth(float health)
    {
    _health_bar_label.Text = health.ToString();
    _health_bar.Value = health;
    }

    private void onPlayerHealthChanged(float playerHealth)
    {
        UpdateHealth(playerHealth);
    }
}
