using Godot;
using System;

public class GUI : MarginContainer
{
    private Tween _tween;
    private Label _healthBarLabel;
    private TextureProgress _healthBar;
    private int _animatedHealth;
    private Label _swordBarLabel;
    private TextureProgress _swordBar;
    private int _animatedSword;
    private Label _shieldBarLabel;
    private TextureProgress _shieldBar;
    private int _animatedShield;
    private Label _bootsBarLabel;
    private TextureProgress _bootsBar;
    private int _animatedBoots;

    public override void _Ready()
    {
        // Get and Activate Tween
        _tween = (Tween) GetNode("Tween");
        if (!_tween.IsActive())
            _tween.Start();

        // Get Health Bar References
        _healthBar = (TextureProgress) GetNode("Bars/HealthBar/LifeBar/Gauge");
        _healthBarLabel = (Label) GetNode("Bars/HealthBar/LifeBar/MarginContainer/Background/Number");

        // Set Health Bar to Player's Max Health
        var player = (player) GetNode("../chars/player");
        _healthBar.MaxValue = player.MaxHealth;
        UpdateHealth(player.MaxHealth);

        // Connect to Player HealthChangedSignal
        player.Connect("HealthChanged", this, nameof(onPlayerHealthChanged));

        // Get Sword Bar References
        _swordBar = (TextureProgress) GetNode("Bars/CycleBars/AttackBar/Gauge");
        _swordBarLabel = (Label) GetNode("Bars/CycleBars/AttackBar/MarginContainer/Background/Number");
        // Get Shield Bar References
        _shieldBar = (TextureProgress) GetNode("Bars/CycleBars/ShieldBar/Gauge");
        _shieldBarLabel = (Label) GetNode("Bars/CycleBars/ShieldBar/MarginContainer/Background/Number");
        // Get Boots Bar References
        _bootsBar = (TextureProgress) GetNode("Bars/CycleBars/BootsBar/Gauge");
        _bootsBarLabel = (Label) GetNode("Bars/CycleBars/BootsBar/MarginContainer/Background/Number");
    }

    public override void _Process(float delta)
    {
        _healthBarLabel.Text = _animatedHealth.ToString();
        _healthBar.Value = _animatedHealth;
    }

    public void UpdateHealth(int health)    
    {
        _tween.InterpolateProperty(this, "_animatedHealth", _animatedHealth, health, 0.6f, Tween.TransitionType.Linear, Tween.EaseType.In);
    }

    private void onPlayerHealthChanged(int playerHealth)
    {
        UpdateHealth(playerHealth);
    }
}
