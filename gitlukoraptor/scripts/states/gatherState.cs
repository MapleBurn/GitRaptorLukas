using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using GDC = Godot.Collections;

public partial class gatherState : State
{
    //nodes
    [Export] private Pleb _pleb;
    [Export] private Area2D detectionArea;
    private NatureObject closestFood;
    private NavigationAgent2D navAgent;
    private AnimatedSprite2D animatedSprite;
    private AStarGrid2D astarGrid;

    //variables
    private Random rdm = Pleb.rdm;
    private double wanderTime;
    private bool foundFood = false;
    private GDC.Array<Vector2I> currentPath;
    
    public override void Enter()
    {
        astarGrid = Pleb.astarGrid;
        currentPath = _pleb.currentPath;
        animatedSprite = _pleb.sprite;
        animatedSprite.Play("walk");
        
        //CallDeferred(nameof(SearchForFood));
        //navAgent = _pleb.navAgent;
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
            //we need to make the target position the bush, so change the navLayers and make path cost
            _pleb.memory[Pleb.MemoryKey.travelPoint] = currentPath.Last();
            Exit();
            EmitSignal(State.SignalName.StateChanged, this, "swimState");
        }
        if (_pleb.hunger >= (float)_pleb.maxHunger / 100f * 80f)
        {
            EmitSignal(SignalName.StateChanged, this, "idleState");
        }
    }

    public override void PhysicsUpdate(double delta)
    {
        if (!foundFood)
        {
            SearchForFood();
        }
        
        if (foundFood) 
        {
            //Pleb found bush and takes food from it
            //massive issue - closestFood can get deleted, and then we're trying to access a disposed object
            if (closestFood != null  && navAgent.IsNavigationFinished())
            {
                int takeAmount = 100 - _pleb.hunger;
                if (takeAmount <= closestFood.resourceCount)
                {
                    _pleb.hunger += takeAmount;
                    closestFood.resourceCount -= takeAmount;
                }
                else
                {
                    _pleb.hunger += closestFood.resourceCount;
                    closestFood.resourceCount = 0;
                }
                closestFood.health -= rdm.Next(5);
                if(_pleb.hunger < (_pleb.maxHunger / 100) * 80)
                {
                    foundFood = false;
                }
            }
        }
        
        if (!navAgent.IsNavigationFinished())
        {   
            Vector2 currentAgentPos = _pleb.GlobalPosition;
            Vector2 nextPos = navAgent.GetNextPathPosition();
            _pleb.Velocity = currentAgentPos.DirectionTo(nextPos) * _pleb.speed;
        }
        
        _pleb.MoveAndSlide();
    }
    
    public override void Exit()
    {
    }
    
    private void SearchForFood()    //pleb searches in the detection area and chooses the closest bush with food as target if there is any
    {
        int foodSourceCount = 0;
        var bodies = detectionArea.GetOverlappingBodies();
        if (bodies != null && bodies.Count > 0)
        {
            Vector2 shortest = new Vector2(10000, 10000);
            foreach (Node2D body in bodies)
            {
                if (body.GetGroups().Contains("nature"))
                {
                    if (body is NatureObject)
                    {
                        NatureObject foodSource = body as NatureObject;
                        if(foodSource.type == "Food")
                        {
                            foodSourceCount++;

                            if (foodSource.resourceCount > 0)
                            {
                                Vector2 position = body.GetGlobalPosition();
                                Vector2 distance = position - _pleb.GlobalPosition;
                                if (distance.Length() < shortest.Length() && foodSource.resourceCount > 0)
                                {
                                    shortest = distance;
                                    closestFood = foodSource;
                                }

                                navAgent.TargetPosition = closestFood.GlobalPosition;
                            }
                        }
                    }
                }
            }
        }
        if (foodSourceCount > 0)
            foundFood = true;
        else
        {
            if (_pleb.memory.TryGetValue(Pleb.MemoryKey.lastSeenBush, out Vector2 lastSeenBush))
            {
                navAgent.TargetPosition = lastSeenBush;
                if (navAgent.IsNavigationFinished())
                {
                        _pleb.memory.Remove(Pleb.MemoryKey.lastSeenBush);
                }
            }
            else
            {
                RandomizeWander();
            }
        }
    }
    
    private void RandomizeWander()
    {
        Vector2 origin = _pleb.GlobalPosition;
        float minPathLength = 50f;
        Rid map = Pleb.navMap;
        int maxAttempts = 10;
        float moveRadius = 200f;

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
}
