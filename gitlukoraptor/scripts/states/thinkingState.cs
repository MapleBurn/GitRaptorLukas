using Godot;
using System;

public partial class thinkingState : State
{
    [Export] private Pleb _pleb;
    public void Enter()
    {}

    public void Update(double delta)
    {
        //EmitSignal(SignalName.StateChanged); nedokončeno
        
    }

    public void PhysicsUpdate(double delta)
    {}
}
