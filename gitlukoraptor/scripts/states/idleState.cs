using Godot;
using System;

public partial class idleState : State
{
    [Export] private Pleb _pleb;
    [Export] private AnimatedSprite2D _animation;
    private Random rdm = new Random();
    private double wanderTime;

    public override void Enter()
    {
        RandomizeWander();
        _animation.Play("idle");
    }

    public override void Update(double delta)
    {
        if (_pleb.Velocity == Vector2.Zero)
            _animation.Play("idle");
        else if (_pleb.direction > Vector2.Zero)
        {
            _animation.FlipH = false;
            _animation.Play("walk");
        }
        else if (_pleb.direction < Vector2.Zero)
        {
            _animation.FlipH = true;
            _animation.Play("walk");
        }
        if (_pleb != null)
            _pleb.Velocity = _pleb.direction * _pleb.speed;
        if (wanderTime > 0)
            wanderTime -= delta;
        else
            RandomizeWander();
        if (_pleb.hunger <= 50)
        {
            Exit();
            EmitSignal(State.SignalName.StateChanged, this, "gatherState");
        }
    }

    public override void PhysicsUpdate(double delta)
    {
        _pleb.MoveAndSlide();
    }

    public override void Exit()
    {
        _animation.Stop();
    }

    private void RandomizeWander()
    {
        _pleb.direction = new Vector2(rdm.Next(-100, 100) / 100f, rdm.Next(-100, 100) / 100f).Normalized();
        wanderTime = rdm.Next(100, 400) / 100f;
    }
}
