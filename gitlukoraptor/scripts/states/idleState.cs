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
        CallDeferred(nameof(RandomizeWander));
        _animation.Play("idle");
    }

    public override void Update(double delta)
    {
        if (_pleb.isDead)
            return;
        if (_pleb != null)
        {
            _pleb.Velocity = _pleb.direction * _pleb.speed;

            _pleb.Animate();
        }
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
        if (_pleb.isDead)
            return;
        _pleb.MoveAndSlide();
    }

    public override void Exit()
    {
        _animation.Stop();
    }

    //private void RandomizeWander()
    //{
    //    if (rdm.Next(20) < 8 && !_pleb.isOnWater)
    //    {
    //        _pleb.direction = Vector2.Zero;
    //    }
    //    else
    //        _pleb.direction = new Vector2(rdm.Next(-100, 100) / 100f, rdm.Next(-100, 100) / 100f).Normalized();
    //    wanderTime = rdm.Next(100, 400) / 100f;
    //}

    private void RandomizeWander()
    {
        Vector2 randomOffset;
        Vector2 randomPoint;
        Vector2 navigablePoint;
        int attempts = 10;
        var navMap = _pleb.navAgent.GetNavigationMap();
        
        for (int i = 0; i < attempts; i++)
        {
            randomOffset = Vector2.Right.Rotated((float)(GD.Randf() * MathF.Tau)) * (float)(GD.Randf() * _pleb.moveRadius);
            randomPoint = _pleb.GlobalPosition + randomOffset;
            navigablePoint = NavigationServer2D.MapGetClosestPoint(navMap, randomPoint);

            //kontroluje jestli point není moc daleko
            if (navigablePoint.DistanceTo(_pleb.GlobalPosition) <= _pleb.moveRadius * 1.5f)
            {
                _pleb.navAgent.TargetPosition = navigablePoint;
                return;
            }
        }
        GD.Print("Couldn't find navigable point after several tries.");
    }
}
