using Godot;
using System;

public partial class gatherState : State
{
    [Export] private Pleb _pleb;
    public override void Enter()
    {
        //enter funkce
    }

    public override void Update(double delta)
    {
        //pro příště: vytvořit check pro nejbližší bush, když bush nenajde tak vybere dir kterou půjde x sekund
        //vyrobit žraní z bush, bush jídlo
        if (_pleb.hunger >= 70)
        {
            EmitSignal(SignalName.StateChanged, this, "idleState");
        }
    }

    public override void PhysicsUpdate(double delta)
    {}
}
