using Godot;
using System;

public partial class DetailPopup : Panel
{
    //Common Details
    private TextureRect _textureRect;
    private Label _name;
    private Label _health;
    
    //Pleb Details
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
    private Node2D _plebDetails;
    private Node2D _natureDetails;
    private Entity _entity;

    public override void _Ready()
    { 
        _textureRect = GetNode<TextureRect>("CommonDetails/TextureRect"); 
        _name = GetNode<Label>("CommonDetails/nameLbl"); 
        _health = GetNode<Label>("CommonDetails/healthLbl"); 
        _plebDetails = GetNode<Node2D>("PlebDetails"); 
        _team = GetNode<Label>("PlebDetails/teamLbl"); 
        _favoriteToggle = GetNode<CheckButton>("CommonDetails/favorite/favoriteToggle"); 
        _hunger = GetNode<Label>("PlebDetails/hungerLbl"); 
        _state  = GetNode<Label>("PlebDetails/stateLbl"); 
        _natureDetails = GetNode<Node2D>("NatureDetails");
        _resource = GetNode<Label>("NatureDetails/resourceLbl"); 
        _production  = GetNode<Label>("NatureDetails/productionLbl"); 
        _type  = GetNode<Label>("NatureDetails/typeLbl");
    }
    
    public void Display(Entity entity)
    {
        if (entity is Pleb)
        {
            Pleb pleb = entity as Pleb;
            Pleb oldPleb = _entity as Pleb;
            if (oldPleb != null && oldPleb != pleb)
            {
                    oldPleb.showDetails = false;
            }
            _stateMachine = pleb.GetNode<StateMachine>("StateMachine");
            _natureDetails.Visible = false;
            _plebDetails.Visible = true;
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
            _plebDetails.Visible = false;
        }
        Visible = true;
        _entity = entity;
        Update();
    }
    public void Update()
    {
        if (_entity is Pleb){
            Pleb pleb = _entity as Pleb;
            
            if  (pleb.isDead)
                return;
            //vezme current frame a udělá z něj texture - needs fixing
            SpriteFrames spriteFrames = pleb.sprite.SpriteFrames;
            string animationName = pleb.sprite.Animation;
            int frameIndex = pleb.sprite.Frame;
            _textureRect.Texture = spriteFrames.GetFrameTexture(animationName, frameIndex);;
            
            _name.Text = "Pleb - " + pleb.name;
            _team.Text = "Team: " + pleb.team;
            _favoriteToggle.ButtonPressed = pleb.favorite;
            _health.Text = "Health:\n" + pleb.health + "/" + pleb.maxHealth;
            _hunger.Text = "Hunger:\n" + pleb.hunger;
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
        if(_entity is Pleb){
            Pleb pleb = _entity as Pleb;
            pleb.showDetails = false;
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
        if (_entity is Pleb)
        {
            Pleb pleb = _entity as Pleb;
            pleb.favorite = toggle;
        }
        if (_entity is NatureObject)
        {
            NatureObject nature = _entity as NatureObject;
            nature.favorite = toggle;
        }
    }
}
