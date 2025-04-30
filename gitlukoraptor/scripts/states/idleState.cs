using Godot;
using System;

public partial class idleState : State
{
    [Export] private Pleb _pleb;
    [Export] private AnimatedSprite2D _animation;
    private Random rdm = Pleb.rdm;

    public override void Enter()
    {
        CallDeferred(nameof(call));
        _animation.Play("idle");
    }

    private void call() { _pleb.RandomizeWander(false); }   //ugly but solves the problem with calldeferred9

    public override void Update(double delta)
    {
        if (_pleb.isDead && _pleb == null)
            return;
        Vector2 currentAgentPos = _pleb.GlobalPosition;
        Vector2 nextPos = _pleb.navAgent.GetNextPathPosition();
        _pleb.direction = currentAgentPos.DirectionTo(nextPos);
        _pleb.Velocity = currentAgentPos.DirectionTo(nextPos) * _pleb.speed;
        
        if (_pleb.navAgent.IsNavigationFinished() && _pleb.wanderTime <= 0)
            _pleb.RandomizeWander(false);
        else
            _pleb.wanderTime -= delta;
        
        _pleb.Animate();
        
        if (_pleb.hunger <= 50)
        {
            Exit();
            EmitSignal(State.SignalName.StateChanged, this, "gatherState");
        }
    }

    public override void PhysicsUpdate(double delta)
    {
        if (_pleb.isDead)
            return;
        _pleb.MoveAndSlide();
    }

    public override void Exit()
    {
        _animation.Stop();
    }
}
