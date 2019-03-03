using Godot;
using System;

public class GUI : MarginContainer
{
    private Tween _tween;
    private Label _healthBarLabel;
    private TextureProgress _healthBar;
    private int _animatedHealth;

    public override void _Ready()
    {
        // get tween
        _tween = (Tween) GetNode("Tween");

        // Get Health Bar Stuff
        _healthBar = (TextureProgress) GetNode("Bars/HealthBar/LifeBar/Gauge");
        _healthBarLabel = (Label) GetNode("Bars/HealthBar/LifeBar/MarginContainer/Background/Number");

        // need to get all the flow bars as well and initialize them to 0 here

        // Set Bar to Player's Max Health
        var player = (player) GetNode("../chars/player");
        _healthBar.MaxValue = player.MaxHealth;
        UpdateHealth(player.MaxHealth);

        // connect to health changed on player
        player.Connect("HealthChanged", this, nameof(onPlayerHealthChanged));
    }

    public override void _Process(float delta)
    {
        _healthBarLabel.Text = _animatedHealth.ToString();
        _healthBar.Value = _animatedHealth;
    }

    public void UpdateHealth(int health)
    {
        _tween.InterpolateProperty(this, "_animatedHealth", _animatedHealth, health, 0.6f, Tween.TransitionType.Linear, Tween.EaseType.In);

        // activate the tween if it is not active
        if (!_tween.IsActive())
        {
            _tween.Start();
        }
    }

    private void onPlayerHealthChanged(int playerHealth)
    {
        UpdateHealth(playerHealth);
    }
}
