using Godot;
using System;

public partial class gatherState : State
{
    [Export] private Pleb _pleb;
    private Area2D detectionArea;
    
    public override void Enter()
    {
        detectionArea = GetNode<Area2D>("DetectionArea");
        //enter funkce
    }

    public override void Update(double delta)
    {/*
        //pro příště: vytvořit check pro nejbližší bush, když bush nenajde tak vybere dir kterou půjde x sekund
        //vyrobit žraní z bush, bush jídlo
        var bodies = detectionArea.GetOverlappingBodies();
        if  (bodies != null && bodies.Count > 0)
        {
            foreach (Node2D body in bodies)
            {
                if (body.GetGroups().Contains("nature"))
                {
                    //go to bush
                }
            }
        }
        
        if (_pleb.hunger >= 80)
        {
            EmitSignal(SignalName.StateChanged, this, "idleState");
        }
        */
    }

    public override void PhysicsUpdate(double delta)
    {}
}
