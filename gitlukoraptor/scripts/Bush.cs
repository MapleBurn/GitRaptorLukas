using Godot;
using System;

public partial class Bush : StaticBody2D, Entity
{
    //bush data
    public Sprite2D sprite;
    public string name = "Bush";
    public int health = 50;
    public bool favorite = false;
    
    public int resourceCount = 100;
    public int resourceProduction = 5;
    public string type = "Food";
    
    public bool showDetails;
    
    public override void _Ready()
    {
        sprite = GetNode<Sprite2D>("Sprite2D");
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
    
    public void Die()
    {
        QueueFree();
    }
}
