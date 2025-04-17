using Godot;
using System;
using System.Runtime.Intrinsics.Arm;

public partial class NatureObject : StaticBody2D, Entity
{
    //object data
    public Sprite2D sprite;
    public string name;
    public int health;
    public int maxHealth;
    public int maxResource;
    public int resourceCount;
    public int resourceProduction;
    public string type;
    public bool showDetails;    
    public bool favorite;
    
    //other nodes
    private Timer resourceTimer;
    private DetailPopup detailPopup;
    
    public override void _Ready()
    {
        resourceTimer = GetNode<Timer>("/root/world1/Timers/ResourceTimer");
        sprite = GetNode<Sprite2D>("Sprite2D");
        detailPopup = GetNode<DetailPopup>("/root/world1/Hud/DetailPopup");
        resourceTimer.Timeout += () => resourceTimer_Tick();
        Prepare();
    }
    public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx)
    {
        if (@event is InputEventMouseButton && @event.IsPressed() && @event.IsActionPressed("clickRight"))
        {
            
            showDetails = true;
            detailPopup.Display(this);
        }
    }

    private void resourceTimer_Tick()
    {
        if (health <= 0)
        {
            Die();
            return;
        }
        
        resourceCount += resourceProduction;
        if (resourceCount > maxResource)
        {
            resourceCount = maxResource;
        }
        detailPopup.Update();
    }

    public virtual void Prepare()
    {
        name = "Nature";
        health = 1;
        maxHealth = 1;
        maxResource = 100;
        resourceCount = 1;
        resourceProduction = 5;
        type = "Type";
        showDetails = false;    
        favorite = false;
    }
    public virtual void Die()
    {
        if (showDetails)
            detailPopup.CloseDetail();
        QueueFree();
    }
    
    
}
