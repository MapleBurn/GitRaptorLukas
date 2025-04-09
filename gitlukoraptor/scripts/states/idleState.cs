using Godot;
using System;

public partial class idleState : State
{
    [Export] private Pleb _pleb;
    private Random rdm = new Random();
    private double wanderTime;

    public override void Enter()
    {
        RandomizeWander();
    }

    public override void Update(double delta)
    {
        if (_pleb != null)
            _pleb.Velocity = _pleb.direction * _pleb.speed;
        if (wanderTime > 0)
            wanderTime -= delta;
        else
            RandomizeWander();
        if (_pleb.hunger <= 50)
        {
            EmitSignal(SignalName.StateChanged, this, "gatherState");
        }
    }

    public override void PhysicsUpdate(double delta)
    {
        _pleb.MoveAndSlide();
    }

    private void RandomizeWander()
    {
        _pleb.direction = new Vector2(rdm.Next(-100, 100) / 100f, rdm.Next(-100, 100) / 100f).Normalized();
        wanderTime = rdm.Next(100, 400) / 100f;
    }
}
