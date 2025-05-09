using Godot;
using System;

public partial class DetailPopup : Panel
{
    //Common Details
    private TextureRect _textureRect;
    private Label _name;
    private Label _health;
    
    //Living Details
    private Label _team;
    private CheckButton _favoriteToggle;
    private Label _hunger;
    private Label _state;
    private StateMachine _stateMachine;
    
    //Nature Details
    private Label _resource;
    private Label _production;
    private Label _type;

    //Detail variables
    private Node2D _livingDetails;
    private Node2D _natureDetails;
    private Entity _entity;

    public override void _Ready()
    { 
        _textureRect = GetNode<TextureRect>("CommonDetails/TextureRect"); 
        _name = GetNode<Label>("CommonDetails/nameLbl"); 
        _health = GetNode<Label>("CommonDetails/healthLbl"); 
        _livingDetails = GetNode<Node2D>("LivingDetails"); 
        _team = GetNode<Label>("LivingDetails/teamLbl"); 
        _favoriteToggle = GetNode<CheckButton>("CommonDetails/favorite/favoriteToggle"); 
        _hunger = GetNode<Label>("LivingDetails/hungerLbl"); 
        _state  = GetNode<Label>("LivingDetails/stateLbl"); 
        _natureDetails = GetNode<Node2D>("NatureDetails");
        _resource = GetNode<Label>("NatureDetails/resourceLbl"); 
        _production  = GetNode<Label>("NatureDetails/productionLbl"); 
        _type  = GetNode<Label>("NatureDetails/typeLbl");
    }
    
    public void Display(Entity entity)
    {
        if (entity is LivingObject)
        {
            LivingObject living = entity as LivingObject;
            LivingObject oldLiving = _entity as LivingObject;
            if (oldLiving != null && oldLiving != living)
            {
                    oldLiving.showDetails = false;
            }
            _stateMachine = living.GetNode<StateMachine>("StateMachine");
            _natureDetails.Visible = false;
            _livingDetails.Visible = true;
        }
        if (entity is NatureObject)
        {
            NatureObject nature = entity as NatureObject;
            NatureObject oldnature = _entity as NatureObject;
            if (oldnature != null && oldnature != nature)
            {
                oldnature.showDetails = false;
            }
            
            _natureDetails.Visible = true;
            _livingDetails.Visible = false;
        }
        Visible = true;
        _entity = entity;
        Update();
    }
    public void Update()
    {
        if (_entity is LivingObject){
            LivingObject living = _entity as LivingObject;
            
            if  (living.isDead)
                return;
            //vezme current frame a udělá z něj texture - needs fixing
            SpriteFrames spriteFrames = living.sprite.SpriteFrames;
            string animationName = living.sprite.Animation;
            int frameIndex = living.sprite.Frame;
            _textureRect.Texture = spriteFrames.GetFrameTexture(animationName, frameIndex);;
            
            _name.Text = _entity.GetType() + " - " + living.name;
            _team.Text = "Team: " + living.team;
            _favoriteToggle.ButtonPressed = living.favorite;
            _health.Text = "Health:\n" + living.health + "/" + living.maxHealth;
            _hunger.Text = "Hunger:\n" + living.hunger + "/" + living.maxHunger;
            _state.Text = "State:\n" + _stateMachine.currentState.Name.ToString().Substring(0, _stateMachine.currentState.Name.ToString().Length-5);
        }
        if (_entity is NatureObject){
            NatureObject nature = _entity as NatureObject;
            _textureRect.Texture = nature.sprite.Texture;
            _name.Text = nature.name;
            _type.Text = "Type: " + nature.type;
            _favoriteToggle.ButtonPressed = nature.favorite;
            _health.Text = "Health:\n" + nature.health + "/" + nature.maxHealth;
            _resource.Text = "Resource:\n" + nature.resourceCount;
            _production.Text = "Production:\n" + nature.resourceProduction;
            
        }
    }
    public void CloseDetail()
    {
        if(_entity is LivingObject){
            LivingObject living = _entity as LivingObject;
            living.showDetails = false;
        }
        if(_entity is NatureObject){
            NatureObject nature = _entity as NatureObject;
            nature.showDetails = false;
        }
        this.Visible = false;
    }

    private void KillEntity()
    {
        if (_entity != null)
            _entity.Die();
    }

    private void ToggleFavorite(bool toggle)
    {
        if (_entity is LivingObject)
        {
            LivingObject living = _entity as LivingObject;
            living.favorite = toggle;
        }
        if (_entity is NatureObject)
        {
            NatureObject nature = _entity as NatureObject;
            nature.favorite = toggle;
        }
    }
}
