using Godot;
using System;

public partial class AttackState : State
{
    //nodes
    [Export] private Pleb _pleb;
    
    //variables
    private Random rdm = Pleb.rdm;

    public override void Enter()
    {}

    public override void Update(double delta)
    {}

    public override void PhysicsUpdate(double delta)
    {}
    
    public override void Exit()
    {}
}
