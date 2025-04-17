using Godot;
using System;
using System.Runtime.Intrinsics.Arm;

public partial class Bush : StaticBody2D, Entity
{
    //bush data
    private Random rdm = new Random();
    public Sprite2D sprite;
    public string name = "Bush";
    public int health = 50;
    public bool favorite = false;
    
    public int resourceCount = 100;
    public int resourceProduction = 5;
    public string type = "Food";
    public bool showDetails;
    
    //other nodes
    private Timer bushTick;
    
    public override void _Ready()
    {
        bushTick = GetNode<Timer>("/root/world1/Timers/BushTick");
        sprite = GetNode<Sprite2D>("Sprite2D");
        resourceProduction = rdm.Next(5, 20);
        
        bushTick.Timeout += () => bushTick_Tick();
    }
    public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx)
    {
        if (@event is InputEventMouseButton && @event.IsPressed() && @event.IsActionPressed("clickRight"))
        {
            DetailPopup detailPopup = GetNode<DetailPopup>("/root/world1/Hud/DetailPopup");
            showDetails = true;
            detailPopup.Display(this);
        }
    }

    private void bushTick_Tick()
    {
        resourceCount += resourceProduction;
    }
    
    public void Die()
    {
        QueueFree();
    }
}
