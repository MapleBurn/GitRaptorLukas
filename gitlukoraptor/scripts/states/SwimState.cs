using Godot;
using System;
using GDC = Godot.Collections;

public partial class SwimState : State
{
    //nodes
    [Export] private Pleb _pleb;
    private AnimatedSprite2D animatedSprite;
    //private NavigationAgent2D navAgent;
    private AStarGrid2D astarGrid;
    
    //variables
    private Random rdm = Pleb.rdm;
    private GDC.Array<Vector2I> currentPath;

    public override void Enter()
    {
        animatedSprite = _pleb.sprite;
        animatedSprite.Play("swim");
        
        currentPath = _pleb.currentPath;
        astarGrid = Pleb.astarGrid;
        
        //swimming is slower than running
        _pleb.speed = _pleb.baseSpeed / 2;
        
        //if (_pleb.memory.TryGetValue(Pleb.MemoryKey.travelPoint, out Vector2 travelPoint))
        //pleb swims back to the closest shore
        if (currentPath.Count < 1)
            FindShore();
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
            GD.Print("Choosing new path :(");
            FindShore();
        }
            
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
        Vector2 targetPos = NavigationServer2D.MapGetClosestPoint(map, _pleb.GlobalPosition);
        targetPos = Pleb.map.LocalToMap(targetPos);
        Vector2I position = Pleb.map.LocalToMap(_pleb.GlobalPosition);
        
        currentPath = astarGrid.GetIdPath(position, (Vector2I)targetPos);
    }
}
