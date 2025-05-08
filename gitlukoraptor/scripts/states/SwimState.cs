using Godot;
using System;

public partial class SwimState : State
{
    //nodes
    [Export] private Pleb _pleb;
    private AnimatedSprite2D animatedSprite;
    private NavigationAgent2D navAgent;
    
    //variables
    private Random rdm = Pleb.rdm;

    public override void Enter()
    {
        animatedSprite = _pleb.sprite;
        animatedSprite.Play("swim");
        
        //swimming is slower than running
        _pleb.speed = _pleb.baseSpeed / 2;
        
        navAgent = _pleb.navAgent;
        if (_pleb.memory.TryGetValue(Pleb.MemoryKey.travelPoint, out Vector2 travelPoint))
        {
            navAgent.TargetPosition = travelPoint;
        }
        else
        {
            //pleb swims back to the closest shore
            FindShore();
        }
    }

    public override void Update(double delta)
    {
        //this handles the animation
        if (_pleb.direction > Vector2.Zero)
        {
            animatedSprite.FlipH = false;
            animatedSprite.Play("swim");
        }
        else if (_pleb.direction < Vector2.Zero)
        {
            animatedSprite.FlipH = true;
            animatedSprite.Play("swim");
        }
        
        if (!_pleb.isOnWater)
        {
            Exit();
            EmitSignal(State.SignalName.StateChanged, this, "idleState");
        }
    }

    public override void PhysicsUpdate(double delta)
    {
        if (!_pleb.navAgent.IsNavigationFinished())
        {
            Vector2 currentAgentPos = _pleb.GlobalPosition;
            Vector2 nextPos = _pleb.navAgent.GetNextPathPosition();
            _pleb.direction = currentAgentPos.DirectionTo(nextPos);
            _pleb.Velocity = currentAgentPos.DirectionTo(nextPos) * _pleb.speed;
        }
        else
            FindShore();
        
        _pleb.MoveAndSlide();
    }

    public override void Exit()
    {
        animatedSprite.Stop();
        
        _pleb.speed = _pleb.baseSpeed;
    }

    private void FindShore()
    {
        Rid map = Pleb.navMap;
        Vector2 target = NavigationServer2D.MapGetClosestPoint(map, _pleb.GlobalPosition);
        var path = NavigationServer2D.MapGetPath(map, _pleb.GlobalPosition, target, false);
        if (path.Length > 0)
            navAgent.TargetPosition = target;
    }
}
