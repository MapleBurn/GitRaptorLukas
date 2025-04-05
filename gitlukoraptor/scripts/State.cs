using Godot;
using System;

public partial class State : Node
{
    [Signal] public delegate void StateChangedEventHandler();
    
    public void Enter()
    {}

    public void Exit()
    {
        EmitSignal(SignalName.StateChanged);
    }

    public void Update(double delta)
    {}

    public void PhysicsUpdate(double delta)
    {}
}
