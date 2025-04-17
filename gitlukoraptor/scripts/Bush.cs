using Godot;
using System;
using System.Runtime.Intrinsics.Arm;

public partial class Bush : NatureObject
{
    private Random rdm = new Random();
    public override void Prepare()
    {
        
        name = "Bush";
        health = 50;
        maxHealth = 50;
        maxResource = 100;
        type = "Food";
        resourceProduction = rdm.Next(5, 20);
        resourceCount = rdm.Next(50, 100);
    }
}
