using Godot;
using System;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using GDC = Godot.Collections;

public partial class WanderState : State
{
    //nodes
    [Export] private Pleb _pleb;
    [Export] private Area2D detectionArea;
    private AnimatedSprite2D animatedSprite;
    private AStarGrid2D astarGrid;
    
    //variables
    private Random rdm = Pleb.rdm;
    private float moveRadius = 300f;
    private GDC.Array<Vector2I> currentPath;

    public override void Enter()
    {
        currentPath = _pleb.currentPath;
        animatedSprite = _pleb.sprite;
        animatedSprite.Play("walk");
        
        astarGrid = Pleb.astarGrid;
        if (currentPath.Count < 1)
            RandomizePath();
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
            //_pleb.memory[Pleb.MemoryKey.travelPoint] = currentPath.Last();
            Exit();
            EmitSignal(State.SignalName.StateChanged, this, "swimState");
        }
        if (_pleb.hunger <= (_pleb.maxHunger / 2f))  //add path finished condition
        {
            Exit();
            EmitSignal(State.SignalName.StateChanged, this, "gatherState");
        }
    }

    public override void PhysicsUpdate(double delta)
    {
        MemoryUpdate();

        if (currentPath.Count > 1)
        {
            Vector2 targetPos = Pleb.map.ToGlobal(Pleb.map.MapToLocal(currentPath[0]));
            _pleb.direction = (targetPos - _pleb.GlobalPosition).Normalized(); 
            _pleb.Velocity = _pleb.direction * _pleb.speed;
            
            if (_pleb.GlobalPosition.DistanceTo(targetPos) < 5f)
            { 
                currentPath.RemoveAt(0); //remove the first point of the path as it's our position (we're already there)
            }
        }
        else
        {
            GD.Print("Choosing new path :D");
            RandomizePath();
        }
            
        _pleb.MoveAndSlide();
    }

    public override void Exit()
    {
        _pleb.currentPath = currentPath;
        animatedSprite.Stop();
    }
    
    private void RandomizePath()
    {
        //there a chance pleb will stop so it looks more natural
        if (rdm.Next(20) < 8)
        {
            Exit();
            EmitSignal(State.SignalName.StateChanged, this, "idleState");
        }
		
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
                //currentPath.RemoveAt(0);
                return;
            }
        }
        //if all attempts would fail he stands still
        GD.Print("I didn't find any path :O");
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
