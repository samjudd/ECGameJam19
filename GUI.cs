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

        // Connect to player Flow Changed Signals
        player.Connect("SwordFlowChanged", this, nameof(onSwordFlowChanged));
        player.Connect("ShieldFlowChanged", this, nameof(onShieldFlowChanged));
        player.Connect("BootsFlowChanged", this, nameof(onBootsFlowChanged));

    }

    public override void _Process(float delta)
    {
        _healthBarLabel.Text = _animatedHealth.ToString();
        _healthBar.Value = _animatedHealth;

        _swordBarLabel.Text = _animatedSword.ToString();
        _swordBar.Value = _animatedSword;

        _shieldBarLabel.Text = _animatedShield.ToString();
        _shieldBar.Value = _animatedShield;

        _bootsBarLabel.Text = _animatedBoots.ToString();
        _bootsBar.Value = _animatedBoots;
    }

    public void UpdateHealth(int health)    
    {
        _tween.InterpolateProperty(this, "_animatedHealth", _animatedHealth, health, 0.6f, Tween.TransitionType.Linear, Tween.EaseType.In);
    }

    public void UpdateSwordFlow(int swordFlow)    
    {
        _tween.InterpolateProperty(this, "_animatedSword", _animatedSword, swordFlow, 0.6f, Tween.TransitionType.Linear, Tween.EaseType.In);
    }

    public void UpdateShieldFlow(int shieldFlow)    
    {
        _tween.InterpolateProperty(this, "_animatedShield", _animatedShield, shieldFlow, 0.6f, Tween.TransitionType.Linear, Tween.EaseType.In);
    }

    public void UpdateBootsFlow(int bootsFlow)    
    {
        _tween.InterpolateProperty(this, "_animatedBoots", _animatedBoots, bootsFlow, 0.6f, Tween.TransitionType.Linear, Tween.EaseType.In);
    }

    private void onPlayerHealthChanged(int playerHealth)
    {
        UpdateHealth(playerHealth);
    }

    private void onSwordFlowChanged(int swordFlow)
    {
        UpdateSwordFlow(swordFlow);
    }

    private void onShieldFlowChanged(int shieldFlow)
    {
        UpdateShieldFlow(shieldFlow);
    }

    private void onBootsFlowChanged(int bootsFlow)
    {
        UpdateBootsFlow(bootsFlow);
    }
}
