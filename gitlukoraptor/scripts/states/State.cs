   using Godot;
using System;

public partial class State : Node
{
    [Signal] public delegate void StateChangedEventHandler(State requestingState, string newStateName);

    public virtual void Enter()
    {
        
    }

    public virtual void Exit()
    {
        
    }

    public virtual void Update(double delta)
    {
        
    }

    public virtual void PhysicsUpdate(double delta)
    {}
}
