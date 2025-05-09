using Godot;
using System;

public partial class WanderState : State
{
    //nodes
    [Export] private Pleb _pleb;
    [Export] private Area2D detectionArea;
    private NavigationAgent2D navAgent;
    private AnimatedSprite2D animatedSprite;
    
    //variables
    private Random rdm = Pleb.rdm;
    private float moveRadius = 300f;

    public override void Enter()
    {
        animatedSprite = _pleb.sprite;
        animatedSprite.Play("walk");
        
        navAgent = _pleb.navAgent;
        
        CallDeferred(nameof(RandomizeWander));
    }

    public override void Update(double delta)
    {
        //this handles the animation
        if (_pleb.direction > Vector2.Zero)
        {
            animatedSprite.FlipH = false;
            animatedSprite.Play("walk");
        }
        else if (_pleb.direction < Vector2.Zero)
        {
            animatedSprite.FlipH = true;
            animatedSprite.Play("walk");
        }
        
        if (_pleb.isOnWater)
        {
            _pleb.memory[Pleb.MemoryKey.travelPoint] = navAgent.TargetPosition;
            Exit();
            EmitSignal(State.SignalName.StateChanged, this, "swimState");
        }
        if (_pleb.hunger <= (_pleb.maxHunger / 2) && navAgent.IsNavigationFinished())
        {
            Exit();
            EmitSignal(State.SignalName.StateChanged, this, "gatherState");
        }
    }

    public override void PhysicsUpdate(double delta)
    {
        MemoryUpdate();
        //calculate where to move to go to the randomly chosen point
        if (!_pleb.navAgent.IsNavigationFinished())
        {
            Vector2 currentAgentPos = _pleb.GlobalPosition;
            Vector2 nextPos = _pleb.navAgent.GetNextPathPosition();
            _pleb.direction = currentAgentPos.DirectionTo(nextPos);
            _pleb.Velocity = currentAgentPos.DirectionTo(nextPos) * _pleb.speed;
        }
        else
        {
            RandomizeWander();
        }
        
        _pleb.MoveAndSlide();
    }

    public override void Exit()
    {
        animatedSprite.Stop();
    }
    
    private void RandomizeWander()
    {
        //there a chance pleb will stop so it looks more natural
        if (rdm.Next(20) < 8)
        {
            Exit();
            EmitSignal(State.SignalName.StateChanged, this, "idleState");
        }
		
        Vector2 origin = _pleb.GlobalPosition;
        float minPathLength = 50f;
        Rid map = Pleb.navMap;
        int maxAttempts = 10;

        for (int i = 0; i < maxAttempts; i++)
        {
            // Generate a random point in a circle
            float angle = GD.Randf() * Mathf.Tau;
            float distance = GD.Randf() * moveRadius;
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
            Vector2 candidate = origin + offset;
			
            // Project to nearest point on navigation mesh
            Vector2 projected = NavigationServer2D.MapGetClosestPoint(map, candidate);
			
            // Check if path is valid
            var path = NavigationServer2D.MapGetPath(map, origin, projected, false);
            if (path.Length > 0 && (projected - origin).Length() > minPathLength)
            {
                navAgent.TargetPosition = projected;
                return;
            }
        }
        //if all attempts would fail he stands still
        Exit();
        EmitSignal(State.SignalName.StateChanged, this, "idleState");
    }

    private void MemoryUpdate()
    {
        var bodies = detectionArea.GetOverlappingBodies();
        if (bodies != null && bodies.Count > 0)
        {
            var body = bodies[0];
            if (body is NatureObject)
            {
                NatureObject foodSource = body as NatureObject;
                if (foodSource.type == "Food")
                    _pleb.memory[Pleb.MemoryKey.lastSeenBush] = body.GlobalPosition;
            }
        }
    }
}
