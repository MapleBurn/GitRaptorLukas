using Godot;
using System;

public partial class idleState : State
{
    [Export] private Pleb _pleb;
    [Export] private AnimatedSprite2D _animation;
    private double wanderTime;
    private Random rdm = Pleb.rdm;
    private Rid map = Spawner.navMap;

    public override void Enter()
    {
        CallDeferred(nameof(RandomizeWander));
        _animation.Play("idle");
    }

    public override void Update(double delta)
    {
        if (_pleb.isDead && _pleb == null)
            return;
        Vector2 currentAgentPos = _pleb.GlobalPosition;
        Vector2 nextPos = _pleb.navAgent.GetNextPathPosition();
        _pleb.direction = currentAgentPos.DirectionTo(nextPos);
        _pleb.Velocity = currentAgentPos.DirectionTo(nextPos) * _pleb.speed;
        
        if (_pleb.navAgent.IsNavigationFinished() && wanderTime <= 0)
            RandomizeWander();
        else
            wanderTime -= delta;
        
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
        
        if (rdm.Next(20) < 8 && !_pleb.isOnWater)
        {
            _pleb.direction = Vector2.Zero;
            _pleb.navAgent.TargetPosition = _pleb.GlobalPosition;
            wanderTime = rdm.Next(100, 400) / 100f;
            return;
        }
        var origin = _pleb.GlobalPosition;
        int MaxAttempts = 10;

        for (int i = 0; i < MaxAttempts; i++)
        {
            // Generate a random point in a circle
            float angle = (float)(GD.Randf() * Mathf.Tau);
            float distance = (float)(GD.Randf() * _pleb.moveRadius);
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
            Vector2 candidate = origin + offset;

            // Project to nearest point on navigation mesh
            Vector2 projected = NavigationServer2D.MapGetClosestPoint(map, candidate);
            Vector2 counterVector = -offset * (float)rdm.NextDouble();
            if (projected != candidate)
                projected = NavigationServer2D.MapGetClosestPoint(map, projected + counterVector);

            // Check if path is valid
            var path = NavigationServer2D.MapGetPath(map, origin, projected, false);
            if (path.Length > 0)
            {
                _pleb.navAgent.TargetPosition = projected;
                return;
            }
        }
    }
}
