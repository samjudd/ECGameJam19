using Godot;
using System;
using System.Linq;

public class player : KinematicBody2D
{
    [Signal]
    public delegate void HealthChanged(int playerHealth);
    [Signal]
    public delegate void SwordFlowChanged(int swordFlow);
    [Signal]
    public delegate void ShieldFlowChanged(int shieldFlow);
    [Signal]
    public delegate void BootsFlowChanged(int bootsFlow);
    [Signal]
    public delegate void PlayerDied();
    public float _baseSpeed = 75f;
    Vector2 _velocity = new Vector2();
    enum state {IDLE, COMBO, TRIP, SWORD, SWORDSHIELD, SHIELD, SHIELDBOOTS, BOOTS, SWORDBOOTS};
    float _attackTime = 0;
    float _flowTime = 0;
    Vector2 _target = new Vector2();
    public const int MaxHealth = 100;
    private int _playerHealth = MaxHealth;
    private int _swordFlow = 0;
    private int _shieldFlow = 0;
    private int _bootsFlow = 0;
    private state[] _stateHistory = new state[5] {state.IDLE, state.IDLE, state.IDLE, state.IDLE, state.IDLE};
    private state[] _attacks = new state[6] {state.SWORD, state.SWORDSHIELD, state.SHIELD, state.SHIELDBOOTS, state.BOOTS, state.SWORDBOOTS};
    private Dictionary<state, state[]> _validTransitions = new Dictionary<state, state[]>()
    {
        {state.SWORD, new state[]{state.SWORDSHIELD, state.IDLE}},
        {state.SWORDSHIELD, new state[]{state.SHIELD, state.SWORDBOOTS, state.IDLE}},
        {state.SHIELD, new state[]{state.SHIELDBOOTS, state.IDLE, state.SHIELD}},
        {state.SHIELDBOOTS, new state[]{state.BOOTS, state.SWORDSHIELD, state.IDLE}},
        {state.BOOTS, new state[]{state.SWORDBOOTS, state.IDLE}},
        {state.SWORDBOOTS, new state[]{state.SWORD, state.SHIELDBOOTS, state.IDLE}}
    };

    private state[][] _swordCombos = new state[][]
    {
        new state[] {state.SWORD, state.COMBO, state.SWORDSHIELD, state.COMBO, state.SWORDBOOTS},
        new state[] {state.SWORDBOOTS, state.COMBO, state.SWORD, state.COMBO, state.SWORDSHIELD},
        new state[] {state.SWORDSHIELD, state.COMBO, state.SWORDBOOTS, state.COMBO, state.SWORD}
    }; 

    private state[][] _shieldCombos = new state[][]
    {
        new state[] {state.SHIELD, state.COMBO, state.SHIELD, state.COMBO, state.SHIELD},
        new state[] {state.SHIELD, state.COMBO, state.SHIELDBOOTS, state.COMBO, state.SWORDSHIELD},
        new state[] {state.SHIELDBOOTS, state.COMBO, state.SWORDSHIELD, state.COMBO, state.SHIELD},
        new state[] {state.SWORDSHIELD, state.COMBO, state.SHIELD, state.COMBO, state.SHIELDBOOTS}
    }; 

    private state[][] _bootsCombos = new state[][]
    {
        new state[] {state.BOOTS, state.COMBO, state.SWORDBOOTS, state.COMBO, state.SHIELDBOOTS},
        new state[] {state.SWORDBOOTS, state.COMBO, state.SHIELDBOOTS, state.COMBO, state.BOOTS},
        new state[] {state.SHIELDBOOTS, state.COMBO, state.BOOTS, state.COMBO, state.SWORDBOOTS}
    };  

    public override void _Ready()
    {
        this.GetNode("Area2D").Connect("body_entered", this, nameof(BodyEntered));
    }

    public override void _PhysicsProcess(float delta)
    {
        GetInput();
        DoAttacks(delta);
        UpdateFlow(delta);
        MoveAndRotate();
    }

    public void GetInput()
    {
        // get and set velocity based on key inputs
        _velocity = new Vector2();
        if (Input.IsActionPressed("move_right"))
        {
            _velocity.x += 1;
        }
        if (Input.IsActionPressed("move_left"))
        {
            _velocity.x -= 1;
        }
        if (Input.IsActionPressed("move_down"))
        {
            _velocity.y += 1;
        }
        if (Input.IsActionPressed("move_up"))
        {
            _velocity.y -= 1;
        }
        _velocity = _velocity.Normalized() * GetSpeed();

        // Checks for attack input, then if state is norma
        if (Input.IsActionJustPressed("sword_attack") && this.ChangeState(state.SWORD))
        {
            ((AnimationPlayer)this.GetNode("AnimationPlayer")).Play("sword_attack");
        }
        else if (Input.IsActionJustPressed("swordshield_attack") && this.ChangeState(state.SWORDSHIELD))
        {
            ((AnimationPlayer)this.GetNode("AnimationPlayer")).Play("swordshield_attack");
        }
        else if (Input.IsActionJustPressed("shield_attack") && this.ChangeState(state.SHIELD))
        {
            ((AnimationPlayer)this.GetNode("pulse/AnimationPlayer")).Play("pulse");
        }
        else if (Input.IsActionJustPressed("boots_attack") && this.ChangeState(state.BOOTS))
        {
            _target = this.Position  + _velocity.Normalized() * 250;
			SetCollisionLayerBit(0, false);
			SetCollisionMaskBit(0, false);
            ((Area2D)this.GetNode("Area2D")).Monitoring = false;
        }
		else if (Input.IsActionJustPressed("bootsshield_attack") && GetCurrentState() == state.IDLE && this.ChangeState(state.SHIELDBOOTS))
		{
            this.Position = this.Position += this.GetRotateChild().Transform.y * GetSpeed() * -5.0f;
		}

    }

    private void DoAttacks(float delta)
    {
        switch (GetCurrentState())
        {
            case state.COMBO:
                if (_attackTime <= 1.5)
                {
                    _attackTime += delta;
                }
                else
                {
                    this.ChangeState(state.IDLE);
                }
                break;
            case state.TRIP:
                if (_attackTime <= 1.0)
                {
                    _attackTime += delta;
                }
                else
                {
                    this.ChangeState(state.IDLE);
                }
                break;
            case state.SWORD:
                // dash at 4x speed for 0.5 seconds with sword out
                if (_attackTime <= 0.3)
                {
                    _velocity = this.GetRotateChild().Transform.y * GetSpeed() * 4.0f;
                    _attackTime += delta;
                }
                else
                {
                    this.ChangeState(state.COMBO);
                    _velocity = new Vector2();
                }
                break;
            case state.SHIELD:
                // go back to normal after pulse
                if(_attackTime <= 0.3)
                {
                    _attackTime += delta;
                }
                else
                {
                    this.ChangeState(state.COMBO);
                }
                break;
            case state.SWORDSHIELD:
                // go back to normal after spin attack
                if(_attackTime <= 0.3)
                {
                    _attackTime += delta;
                }
                else
                {
                    this.ChangeState(state.COMBO);
                }
                break;
            case state.BOOTS:
                if (_attackTime <= 0.5)
                {
                    _velocity = this.GetRotateChild().Transform.y * GetSpeed() * 10.0f;
                    _attackTime += delta;
                }
                else
                {
                    this.ChangeState(state.COMBO);
                    _target = new Vector2();
                    ((Area2D)this.GetNode("Area2D")).Monitoring = true;
					SetCollisionLayerBit(0, true);
					SetCollisionMaskBit(0, true);
                }
                break;
			case state.SHIELDBOOTS:
				if (_attackTime > 0.3 && !((AnimationPlayer)this.GetNode("AnimationPlayer")).IsPlaying()) {
					((AnimationPlayer)this.GetNode("AnimationPlayer")).Play("spray_pulse_attack");
				}
				if (_attackTime <= 0.3) {
                	_attackTime += delta;
                } else if (_attackTime <= 0.6) {
					_velocity = this.GetRotateChild().Transform.y * GetSpeed() * 2.0f;
            		_attackTime += delta;
				} else {
					this.ChangeState(state.COMBO);
                    _attackTime = 0;
				}
				break;
        }
    }

        private void UpdateFlow(float delta)
    {
        if (_flowTime < 1.0)
        {
            _flowTime += delta;
        }
        else
        {
            ChangeSwordFlow(-1);
            ChangeShieldFlow(-1);
            ChangeBootsFlow(-1);
            _flowTime = 0;
        }
    }

    private void MoveAndRotate()
    {        
        MoveAndSlide(_velocity);
        if(_velocity.Length() != 0)
        {
			Node2D rotatedPlayer = this.GetRotateChild();
			rotatedPlayer.Rotation += rotatedPlayer.Transform.y.AngleTo(_velocity);
        }
    }

    private bool ChangeState(state newState)
    {
        if (GetCurrentState() == state.COMBO)
        {
            // if it's a valid combo, transition, check for cycle
            if (_validTransitions[GetPreviousState()].Contains(newState))
            {
                AddState(newState);
                if(CheckCombo(_swordCombos))
                {
                    ChangeSwordFlow(3);
                }
                else if (CheckCombo(_shieldCombos))
                {
                    ChangeShieldFlow(3);
                }
                else if (CheckCombo(_bootsCombos))
                {
                    ChangeBootsFlow(3);
                }
                return true;
            }
            // if not a valid combo trip
            else
            {
                AddState(state.TRIP);
                ((Sprite)this.GetNode("tripMask")).Visible = true;
                return false;
            }

        }
        // attacks must all go to combo
        if (_attacks.Contains(GetCurrentState()) && newState == state.COMBO)
        {
            AddState(newState);
            return true;
        }
        // trips must go back to idle
        if (GetCurrentState() == state.TRIP && newState == state.IDLE)
        {
            AddState(newState);
            ((Sprite)this.GetNode("tripMask")).Visible = false;
            return true;
        }
        // from idle must go to an attack
        if (GetCurrentState() == state.IDLE && _attacks.Contains(newState))
        {
            AddState(newState);
            return true;
        }
        return false;
    }

	private Node2D GetRotateChild() {
		return (Node2D)GetNode("RotateChild");
	}

    private void BodyEntered(KinematicBody2D body)
    {
        if (body.GetGroups().Contains("enemies"))
        {
			if (body is enemy) {
				((enemy)body).Knockback(this);
			}
            ChangeHealth(-1);
        }
    }

    private void ChangeHealth(int delta)
    {
        if(delta < 0)
        {
            delta = (int)Math.Round((float)delta * (20.0f / (float)_shieldFlow));
        }
        _playerHealth += delta;
        this.EmitSignal(nameof(HealthChanged), _playerHealth);
    }

    private void ChangeSwordFlow(int delta)
    {
        
        _swordFlow = Mathf.Clamp(_swordFlow + delta, 0, 100);
        this.EmitSignal(nameof(SwordFlowChanged), _swordFlow);
    }

    private void ChangeShieldFlow(int delta)
    {
        _shieldFlow = Mathf.Clamp(_shieldFlow + delta, 0, 100);
        this.EmitSignal(nameof(ShieldFlowChanged), _shieldFlow);
    }

    private void ChangeBootsFlow(int delta)
    {
        _bootsFlow = Mathf.Clamp(_bootsFlow + delta, 0, 100);
        this.EmitSignal(nameof(BootsFlowChanged), _bootsFlow);
    }

    private void AddState(state state)
    {
        // Rightmost element is most recent, so shift elements left one
        _stateHistory[0] = _stateHistory[1];
        _stateHistory[1] = _stateHistory[2];
        _stateHistory[2] = _stateHistory[3];
        _stateHistory[3] = _stateHistory[4];
        // Add New State on Right
        _stateHistory[4] = state;
        // zero attack time anytime the state changes
        _attackTime = 0;
    }

    private state GetCurrentState()
    {
        return _stateHistory[4];
    }

    private state GetPreviousState()
    {
        return _stateHistory[3];
    }

    private bool CheckCombo(state[][] validCombos)
    {
        foreach (state[] combo in validCombos)
        {
            if (combo.SequenceEqual(_stateHistory))
                return true;
        }
        return false;
    }

    private float GetSpeed()
    {
        return _baseSpeed + _bootsFlow / 2.0f;
    }

    public int GetSwordFlow()
    {
        return _swordFlow;
    }
}