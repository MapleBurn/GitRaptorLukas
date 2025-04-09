using Godot;
using System;

public partial class attackState : State
{
    [Export] private Pleb _pleb;
    public override void Enter()
    {
    }

    public override void Update(double delta)
    {
        if (true) //dočasná podmínka
        {
            EmitSignal(SignalName.StateChanged, this, "idleState");
        }
    }

    public override void PhysicsUpdate(double delta)
    {}
}
