using Godot;
using System;

public partial class idleState : State
{
    //nodes
    [Export] private Pleb _pleb;
    AnimatedSprite2D animatedSprite;
    
    //variables
    private Random rdm = Pleb.rdm;
    private double idleTime;

    public override void Enter()
    {
        if (!_pleb.initialized)
            _pleb.Initialize();
        animatedSprite = _pleb.sprite;
        animatedSprite.Play("idle");
        
        _pleb.direction = Vector2.Zero;
        _pleb.Velocity = Vector2.Zero;
        idleTime = (float)rdm.Next(50, 300) / 100f;
    }

    public override void Update(double delta)
    {
        idleTime -= delta;
        
        if (_pleb.isOnWater)
        {
            Exit();
            EmitSignal(State.SignalName.StateChanged, this, "swimState");
        }
        if (_pleb.hunger <= (_pleb.maxHunger / 2) && idleTime <= 0)
        {
            Exit();
            EmitSignal(State.SignalName.StateChanged, this, "gatherState");
        }
        else if (idleTime <= 0)
        {
            Exit();
            EmitSignal(State.SignalName.StateChanged, this, "wanderState");
        }
    }

    public override void PhysicsUpdate(double delta)
    {
        _pleb.MoveAndSlide();
    }

    public override void Exit()
    {
        animatedSprite.Stop();
    }
}
