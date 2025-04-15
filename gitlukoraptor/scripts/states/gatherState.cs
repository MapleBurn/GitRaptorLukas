using Godot;
using System;
using System.Collections.Generic;

public partial class gatherState : State
{
    [Export] private Pleb _pleb;
    [Export] private Area2D detectionArea;
    
    private Random rdm =  new Random();
    private double wanderTime;
    private bool isSearchingForBush = false;
    private bool isGoingToBush = false;
    private Bush closestBush;
    private Vector2 shortest = new Vector2(10000, 10000);
    
    public override void Enter()
    {
        //detectionArea = GetNode<Area2D>("DetectionArea");
        //enter funkce
    }

    public override void Update(double delta)
    {
        if (_pleb.isDead && _pleb == null)
            return;
        if (!isGoingToBush)
            SearchForFood();
        if (isSearchingForBush)
        {
            _pleb.Velocity = _pleb.direction * _pleb.speed;
            if (wanderTime > 0)
                wanderTime -= delta;
            else
                RandomizeWander();
        }

        if ((closestBush.Position - _pleb.Position).Length() <= 5)
        {
            isSearchingForBush = false;
            isGoingToBush = false;
            
            //bush se ubere resource, pleb přibere hunger
            closestBush.resourceCount -= (100 - _pleb.hunger);
        }
            
        //když najde bush aby k němu šel - v searchforfood určíme pouze direction ale nedáváme velocity
        
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

    private void SearchForFood()
    {
        var bodies = detectionArea.GetOverlappingBodies();
        if (bodies != null && bodies.Count > 0)
        {
            foreach (Node2D body in bodies)
            {
                if (body.GetGroups().Contains("nature"))
                {
                    if (body.Name.ToString().StartsWith("bush"))
                    {
                        Vector2 position = body.GetPosition();
                        Vector2 distance = position - _pleb.Position;
                        Bush bush = body as Bush; 
                        //změnit aby to porovnávalo velikosti vektorů - pak blaBABABBA
                        if (distance.Length() < shortest.Length() &&  bush.resourceCount > 0)
                        {
                            shortest = distance;
                            closestBush = bush; 
                        }
                    }
                }
            }
            _pleb.direction = shortest.Normalized();
            isGoingToBush = true;
        }
        else
        {
            RandomizeWander();
        }
    }
    
    private void RandomizeWander()
    {
        isSearchingForBush = true;
        _pleb.direction = new Vector2(rdm.Next(-100, 100) / 100f, rdm.Next(-100, 100) / 100f).Normalized();
        wanderTime = rdm.Next(400, 1600) / 100f;
    }
}
