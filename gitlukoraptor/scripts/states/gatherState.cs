using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class gatherState : State
{
    [Export] private Pleb _pleb;
    [Export] private Area2D detectionArea;
    
    private Random rdm =  new Random();
    private double wanderTime = 0;
    private bool isSearchingForFood = true;
    private NatureObject closestFood;
    
    public override void Enter()
    {
        RandomizeWander();
    }

    public override void Update(double delta)
    {
        if (_pleb.isDead || _pleb == null)
            return;
        if (wanderTime > 0)
            wanderTime -= delta;
        
        if (isSearchingForFood)
        {
            _pleb.Velocity = _pleb.direction * _pleb.speed;
            if (wanderTime <= 0)
                RandomizeWander();
            SearchForFood();
        }
        
        _pleb.Animate();
        
        if (_pleb.hunger >= 80)
        {
            EmitSignal(SignalName.StateChanged, this, "idleState");
        }
    }

    public override void PhysicsUpdate(double delta)
    {
        if (_pleb.isDead)
            return;
        
        if (!isSearchingForFood) 
        {
            //Pleb found bush and takes food from it
            if (closestFood != null  && _pleb.navAgent.IsNavigationFinished())
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
                _pleb.Velocity = Vector2.Zero;
            }
            else
            {
                Vector2 currentAgentPos = _pleb.GlobalPosition;
                Vector2 nextPos = _pleb.navAgent.GetNextPathPosition();
                _pleb.Velocity = currentAgentPos.DirectionTo(nextPos) * _pleb.speed;
            }
            if(_pleb.hunger < 80)
            {
                isSearchingForFood = true;
            }
        }
        _pleb.MoveAndSlide();
    }
    
    private void SearchForFood()
    {
        var bodies = detectionArea.GetOverlappingBodies();
        if (bodies != null && bodies.Count > 0)
        {
            int foodSourceCount = 0;
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
                            }
                            //_pleb.direction = shortest.Normalized();
                            _pleb.navAgent.TargetPosition = closestFood.GlobalPosition;
                        }
                    }
                }
            }
            if (foodSourceCount > 0)
                isSearchingForFood = false;
        }
    }
    
    private void RandomizeWander()
    {
        _pleb.direction = new Vector2(rdm.Next(-100, 100) / 100f, rdm.Next(-100, 100) / 100f).Normalized();
        wanderTime = rdm.Next(100, 400) / 100f;
    }
}
