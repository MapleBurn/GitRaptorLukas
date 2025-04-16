using Godot;
using System;
using System.Collections.Generic;

public partial class gatherState : State
{
    [Export] private Pleb _pleb;
    [Export] private Area2D detectionArea;
    
    private Random rdm =  new Random();
    private double wanderTime;
    private bool isSearchingForBush = true;
    //private bool isGoingToBush = false;
    private Bush closestBush;
    private Vector2 shortest = new Vector2(10000, 10000);
    
    public override void Enter()
    {
        RandomizeWander();
        //detectionArea = GetNode<Area2D>("DetectionArea");
        //enter funkce
    }

    public override void Update(double delta)
    {
        if (_pleb.isDead || _pleb == null)
            return;
        if (isSearchingForBush)
        {
            _pleb.Velocity = _pleb.direction * _pleb.speed;
            if (wanderTime > 0)
                wanderTime -= delta;
            else
                RandomizeWander();
            SearchForBush();
        }
        
        if (!isSearchingForBush) 
        {
            //Pleb found bush and takes food from it
            if ((closestBush.Position - _pleb.Position).Length() <= 5)
            {
                int takeAmount = 100 - _pleb.hunger;
                if (takeAmount <= closestBush.resourceCount)
                {
                    _pleb.hunger += takeAmount;
                    closestBush.resourceCount -= takeAmount;
                }
                else
                {
                    _pleb.hunger += closestBush.resourceCount;
                    closestBush.resourceCount = 0;
                }
            }
            else
            {
                _pleb.Velocity = _pleb.direction * _pleb.speed;
            }
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
        _pleb.MoveAndSlide();
    }
    
    private void SearchForBush()
    {
        var bodies = detectionArea.GetOverlappingBodies();
        if (bodies != null && bodies.Count > 0)
        {
            foreach (Node2D body in bodies)
            {
                if (body.GetGroups().Contains("nature"))
                {
                    if (body.GetType() == StaticBody2D)    //fix fix fix - musí být typu Bush
                    {
                        Vector2 position = body.GetPosition();
                        Vector2 distance = position - _pleb.Position;
                        Bush bush = body as Bush; 
                        if (distance.Length() < shortest.Length() &&  bush.resourceCount > 0)
                        {
                            shortest = distance;
                            closestBush = bush; 
                        }
                    }
                }
            }
            _pleb.direction = shortest.Normalized();
            isSearchingForBush = false;
        }
        else
        {
            isSearchingForBush = true;
        }
    }
    
    private void RandomizeWander()
    {
        isSearchingForBush = true;
        _pleb.direction = new Vector2(rdm.Next(-100, 100) / 100f, rdm.Next(-100, 100) / 100f).Normalized();
        wanderTime = rdm.Next(400, 1600) / 100f;
    }
}
