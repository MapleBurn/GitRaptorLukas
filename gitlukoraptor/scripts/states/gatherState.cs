using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using GDC = Godot.Collections;

public partial class gatherState : State
{
    //nodes
    [Export] private Pleb _pleb;
    private Area2D detectionArea;
    private NatureObject closestFood;
    //private NavigationAgent2D navAgent;
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
        detectionArea = _pleb.detectionArea;
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
            //_pleb.memory[Pleb.MemoryKey.travelPoint] = currentPath.Last();
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
        Vector2 targetPos = Vector2.Zero;
        if (currentPath.Count > 1)
        {
            targetPos = Pleb.map.ToGlobal(Pleb.map.MapToLocal(currentPath[0]));
            _pleb.direction = (targetPos - _pleb.GlobalPosition).Normalized(); 
            _pleb.Velocity = _pleb.direction * _pleb.speed;
            
            if (_pleb.GlobalPosition.DistanceTo(targetPos) < 5f)
            { 
                currentPath.RemoveAt(0); //remove the first point of the path as it's our position (we're already there)
            }
        }
        else
        {
            SearchForFood();
        }
        
        if (!foundFood)
            SearchForFood();
        
        if (foundFood) 
        {
            //Pleb found bush and takes food from it
            //massive issue - closestFood can get deleted, and then we're trying to access a disposed object
            if (closestFood != null  && _pleb.GlobalPosition.DistanceTo(targetPos) < 5f)
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
                if(_pleb.hunger < (_pleb.maxHunger / 100f) * 80)
                {
                    foundFood = false;
                }
            }
        }
        
        _pleb.currentPath = currentPath;
        _pleb.MoveAndSlide();
    }
    
    public override void Exit()
    {
        _pleb.currentPath = currentPath;
        animatedSprite.Stop();
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

                                var targetPos = Pleb.map.LocalToMap(closestFood.GlobalPosition);
                                var startPos = Pleb.map.LocalToMap(position);
                                astarGrid.GetIdPath(startPos, targetPos);
                                currentPath.RemoveAt(0);
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
                Vector2I targetPos = Pleb.map.LocalToMap(lastSeenBush);
                Vector2 endPos = Pleb.map.ToGlobal(Pleb.map.MapToLocal(currentPath[0]));
                if (_pleb.GlobalPosition.DistanceTo(endPos) < 5f)
                {
                        _pleb.memory.Remove(Pleb.MemoryKey.lastSeenBush);
                }
            }
            else
            {
                RandomizePath();
            }
        }
    }
    
    private void RandomizePath()
    {
        Vector2I position = Pleb.map.LocalToMap(_pleb.GlobalPosition);
        int moveRadius = 25;
        int maxAttempts = 10;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2I movePoint = new Vector2I(rdm.Next(-moveRadius, moveRadius), rdm.Next(-moveRadius, moveRadius));
            Vector2I candidate = position + movePoint;
            
            Vector2I cellType = Pleb.map.GetCellAtlasCoords(candidate);
            if (cellType != LivingObject.water && cellType != LivingObject.shallow && cellType != LivingObject.mountain)
            {
                candidate = new Vector2I(Mathf.Abs(candidate.X), Mathf.Abs(candidate.Y));   //has to be positive so it'll be on the map
                if (candidate > Pleb.mapSize) //has to be on the map
                    candidate = Pleb.mapSize;
                currentPath = astarGrid.GetIdPath(position, candidate);
                currentPath.RemoveAt(0);
                return;
            }
        }
        //if all attempts would fail he stands still
        GD.Print("I didn't find any path ._.");
        Exit();
        EmitSignal(State.SignalName.StateChanged, this, "idleState");
    }
}
