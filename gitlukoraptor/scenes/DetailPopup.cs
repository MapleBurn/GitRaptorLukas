using Godot;
using System;

public partial class DetailPopup : Panel
{
    [Export] private TextureRect _textureRect;
    [Export] private Label _name;
    [Export] private Label _team;
    [Export] private CheckButton _favoriteToggle;
    [Export] private Label _health;
    [Export] private Label _hunger;
    [Export] private Label _state;
    private Pleb _pleb;
    private StateMachine _stateMachine;

    public void Display(Pleb pleb)
    {
        if (_pleb != null && _pleb != pleb)
        {
            _pleb.showDetails = false;
        }
        Visible = true;
        _pleb = pleb;
        _stateMachine = _pleb.GetNode<StateMachine>("StateMachine");
    }
    public void Update()
    {
        _textureRect.Texture = _pleb.sprite.Texture;
        _name.Text = "Name:" + _pleb.name;
        _team.Text = "Team:" + _pleb.teamId;
        _favoriteToggle.ButtonPressed = _pleb.favorite;
        _health.Text = "Health:\n" + _pleb.health;
        _hunger.Text = "Hunger:\n" + _pleb.hunger;
        _state.Text = "State:\n" + _stateMachine.currentState.Name.ToString().Substring(0, _stateMachine.currentState.Name.ToString().Length-5);

    }
    private void CloseDetail()
    {
        _pleb.showDetails = false;
        this.Visible = false;
    }

    private void ToggleFavorite(bool toggle)
    {
        _pleb.favorite = _favoriteToggle.ButtonPressed;
    }
}
