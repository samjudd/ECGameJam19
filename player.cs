using Godot;
using System;

public class player : KinematicBody2D
{
    public float _speed = 75f;
    Vector2 _velocity = new Vector2();
    enum state {NORMAL, SWORD_ATTACK};
    state _state = state.NORMAL;
    float _attack_time = 0;

    public override void _PhysicsProcess(float delta)
    {
        GetInput();
        DoAttacks(delta);
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
        _velocity = _velocity.Normalized() * _speed;

        // check for attacks, do attack if there is one
        if (Input.IsActionJustPressed("sword_attack"))
        {
            _state = state.SWORD_ATTACK;
            ((AnimationPlayer)this.GetNode("AnimationPlayer")).Play("sword_attack");
        }
    }

    private void DoAttacks(float delta)
    {
        switch (_state)
        {
            case state.SWORD_ATTACK:
                // dash at triple speed for 0.5 seconds with sword out
                if (_attack_time <= 0.3)
                {
                    _velocity = this.Transform.y * _speed * 3.0f;
                    _attack_time += delta;
                }
                else
                {
                    _state = state.NORMAL;
                    _attack_time = 0;
                    _velocity = new Vector2();
                }
                break;
        }

    }

    private void MoveAndRotate()
    {        
        MoveAndSlide(_velocity);
        if(_velocity.Length() != 0)
        {
            this.Rotation += this.Transform.y.AngleTo(_velocity);
        }
    }
}