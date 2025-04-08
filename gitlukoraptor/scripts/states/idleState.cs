using Godot;
using System;

public partial class idleState : State
{
    [Export] private Pleb _pleb;

    public void Enter()
    {
    }

    public void Update(double delta)
    {
        if (true) //dočasná podmínka
        {
            EmitSignal(SignalName.StateChanged, this, "idleState");
        }
    }

    public void PhysicsUpdate(double delta)
    {}
}
