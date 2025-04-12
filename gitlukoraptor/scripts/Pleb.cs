using Godot;
using System;

public partial class Pleb : CharacterBody2D, Entity
{
	public static string[] namePresets = ["Snorky", "Blimbo", "Wibbles", "Zonker", "Boinkle", "Squibble", "Doodle", "Fizzle", "Wobble", "Jiggly", "Fizzbin", "Noodle", "Plonker", "Goober", "Ignacio", "Dexter", "Lazaro", "Caius", "Amias", "Orion", "Zephyr", "Lucian", "Thaddeus", "Cassian", "Erasmus", "Balthazar", "Isidore", "Phineas", "Leandro", "Octavian", "Quillon", "Dario", "Alaric", "Simeon", "Tiberius", "Steve", "Mike", "David", "John", "Mark", "Michael", "Scott", "Jeff", "Paul", "Chris", "Robert", "Dave", "Tom", "Jim", "Tim", "Kevin", "Joe", "Brian", "Domini", "Dara", "Destry", "Elowen", "Gaia", "Glora", "Garnet", "Harbor", "Harlow", "Lucretia", "Lumen", "Lura", "Lux", "Tansy", "Sunny", "Chase", "Wade", "Clay", "Reed", "Sky", "Ash", "Dawn", "River", "Flint", "Coral", "Pearl", "Haven", "Frost", "Echo", "Breeze", "Blaze", "Stone", "Vale", "Meadow"];
	
	private Timer gameTimer;
	public Vector2 direction = Vector2.Zero;
	public float speed = 50f;
	
	//pleb data
	public Sprite2D sprite;
	public int health = 100;
	public string name = "Ignac";
	public bool favorite = false;
	
	public int hunger = 100;
	public string team = "none";
	
	
	public bool showDetails;
	DetailPopup detailPopup;
	
	public override void _Ready()
	{
		gameTimer = GetNode<Timer>("/root/world1/GameTick");
		sprite = GetNode<Sprite2D>("Sprite2D");
		detailPopup = GetNode<DetailPopup>("/root/world1/Hud/DetailPopup");
		
		gameTimer.Timeout += () => gameTimer_Tick();

		name = GenerateName();
	}
	
	public override void _PhysicsProcess(double delta)
	{
	}
	
	private void gameTimer_Tick()
	{
		if (health <= 0)
		{
			Die();
			return;
		}
		if  (hunger <= 0)
			health--;
		else
			hunger--;

		if (showDetails)
		{
			detailPopup.Update();
		}
	}

	public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx)
	{
		if (@event is InputEventMouseButton && @event.IsPressed() && @event.IsActionPressed("clickRight"))
		{
			showDetails = true;
			detailPopup.Display(this);
		}
	}

	public string GenerateName()
	{
		Random rnd = new Random();
		string name = "";
		char[] vowels = ['a', 'e', 'i', 'o', 'u', '&'];
		char[] consonants = ['b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'y', 'z', '&', '&'];
		if (rnd.Next(5) > 1)
		{
			bool wasVowelLast = true;
			for (int i = 0; i <= 10; i++)
			{
				char letter;
				if (rnd.Next(100) < 70)
				{
					if (wasVowelLast)
					{
						letter = consonants[rnd.Next(consonants.Length)];
						wasVowelLast = false;
					}
					else
					{
						letter = vowels[rnd.Next(vowels.Length)];
						wasVowelLast = true;
					}

					if (name.Length > 2 && letter == '&')
					{
						break;
					}

					if (letter == '&')
					{
						continue;
					}

					name += letter;
				}
			}
		}
		else
		{
			name = namePresets[rnd.Next(namePresets.Length)];
		}
		name = char.ToUpper(name[0]) + name.Substring(1);
		return name;
	}
	public void Die()
	{
		QueueFree();
	}
	
}
