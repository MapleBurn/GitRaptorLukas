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
    
    //Bush Details
    private Label _resource;
    private Label _production;
    private Label _type;

    //Detail variables
    private Node2D _plebDetails;
    private Node2D _bushDetails;
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
        _bushDetails = GetNode<Node2D>("BushDetails");
        _resource = GetNode<Label>("BushDetails/resourceLbl"); 
        _production  = GetNode<Label>("BushDetails/productionLbl"); 
        _type  = GetNode<Label>("BushDetails/typeLbl");
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
            _bushDetails.Visible = false;
            _plebDetails.Visible = true;
        }
        if (entity is Bush)
        {
            Bush bush = entity as Bush;
            Bush oldBush = _entity as Bush;
            if (oldBush != null && oldBush != bush)
            {
                oldBush.showDetails = false;
            }
            
            _bushDetails.Visible = true;
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
            
            //vezme current frame a udělá z něj texture - needs fixing
            SpriteFrames spriteFrames = pleb.sprite.SpriteFrames;
            string animationName = pleb.sprite.Animation;
            int frameIndex = pleb.sprite.Frame;
            _textureRect.Texture = spriteFrames.GetFrameTexture(animationName, frameIndex);;
            
            _name.Text = "Pleb - " + pleb.name;
            _team.Text = "Team: " + pleb.team;
            _favoriteToggle.ButtonPressed = pleb.favorite;
            _health.Text = "Health:\n" + pleb.health;
            _hunger.Text = "Hunger:\n" + pleb.hunger;
            _state.Text = "State:\n" + _stateMachine.currentState.Name.ToString().Substring(0, _stateMachine.currentState.Name.ToString().Length-5);
        }
        if (_entity is Bush){
            Bush bush = _entity as Bush;
            _textureRect.Texture = bush.sprite.Texture;
            _name.Text = bush.name;
            _type.Text = "Type: " + bush.type;
            _favoriteToggle.ButtonPressed = bush.favorite;
            _health.Text = "Health:\n" + bush.health;
            _resource.Text = "Resource:\n" + bush.resourceCount;
            _production.Text = "Production:\n" + bush.resourceProduction;
            
        }
    }
    public void CloseDetail()
    {
        if(_entity is Pleb){
            Pleb pleb = _entity as Pleb;
            pleb.showDetails = false;
        }
        if(_entity is Bush){
            Bush bush = _entity as Bush;
            bush.showDetails = false;
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
        if (_entity is Bush)
        {
            Bush bush = _entity as Bush;
            bush.favorite = toggle;
        }
    }
}
