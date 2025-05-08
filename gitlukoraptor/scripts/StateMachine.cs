using Godot;
using System;
using Godot.Collections;

public partial class StateMachine : Node
{
    [Export] private State inicialState;
    public State currentState;
    private Pleb _pleb;
    private Dictionary<string, State> states = new Dictionary<string, State>();

    public override void _Ready()
    {
        //state machine will check if pleb is alive before it updates current state
        _pleb = GetParent<Pleb>();
        
        foreach (var child in GetChildren())
        {
            if (child is State state)
            {
                states[state.Name] = state;
                state.Connect(State.SignalName.StateChanged, Callable.From<State, string>(StateChanged));
            }
        }

        if (inicialState != null)
        {
            inicialState.Enter();
            currentState = inicialState;
        }
    }

    public override void _Process(double delta)
    {
        if (currentState != null && !_pleb.isDead)
            currentState.Update(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (currentState != null && !_pleb.isDead)
            currentState.PhysicsUpdate(delta);
    }

    public void StateChanged(State state, string newStateName)
    {
        if (state != currentState)
        {
            return;
        }
        State newState = states[newStateName];
        if (newState == null)
        {
            return;
        }

        if (currentState != null)
        {
            currentState.Exit();
        }
        
        newState.Enter();
        currentState = newState;
    }
}
