using Godot;
using System;
using Godot.Collections;

public partial class StateMachine : Node
{
    private State currentState;
    private Dictionary<string, State> states = new Dictionary<string, State>();

    public override void _Ready()
    {
        foreach (var child in GetChildren())
        {
            if (child is State)
            {
                states[child.Name] = child as State;
                child.Connect(State.SignalName.StateChanged, Callable.From(StateChanged));
            }
        }
    }

    public override void _Process(double delta)
    {
        if (currentState != null)
            currentState.Update(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (currentState != null)
            currentState.PhysicsUpdate(delta);
    }

    public void StateChanged()
    {
        
    }
}
