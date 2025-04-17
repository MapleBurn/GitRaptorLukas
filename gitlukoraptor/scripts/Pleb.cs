using Godot;
using System;

public partial class Pleb : CharacterBody2D, Entity
{
	public static string[] namePresets = ["Snorky", "Blimbo", "Wibbles", "Zonker", "Boinkle", "Squibble", "Doodle", "Fizzle", "Wobble", "Jiggly", "Fizzbin", "Noodle", "Plonker", "Goober", "Ignacio", "Dexter", "Lazaro", "Caius", "Amias", "Orion", "Zephyr", "Lucian", "Thaddeus", "Cassian", "Erasmus", "Balthazar", "Isidore", "Phineas", "Leandro", "Octavian", "Quillon", "Dario", "Alaric", "Simeon", "Tiberius", "Steve", "Mike", "David", "John", "Mark", "Michael", "Scott", "Jeff", "Paul", "Chris", "Robert", "Dave", "Tom", "Jim", "Tim", "Kevin", "Joe", "Brian", "Domini", "Dara", "Destry", "Elowen", "Gaia", "Gloria", "Garnet", "Harbor", "Harlow", "Lucretia", "Lumen", "Lura", "Lux", "Tansy", "Conquest", "Victor", "Sunny", "Chase", "Wade", "Clay", "Reed", "Sky", "Ash", "Dawn", "River", "Flint", "Coral", "Pearl", "Haven", "Frost", "Echo", "Breeze", "Blaze", "Stone", "Vale", "Meadow"];
	
	private Timer gameTimer;
	private Timer dieTimer;
	public Vector2 direction = Vector2.Zero;
	public AnimatedSprite2D sprite;
	public float speed = 50f;
	Random rnd = new Random();
	
	//pleb data
	public int maxHealth = 100;
	public int health = 100;
	public int hunger = 75;
	public string name = "Ignacio";
	public bool favorite = false;
	public string team = "none";
	public bool isDead = false;
	
	
	public bool showDetails;
	DetailPopup detailPopup;
	
	public override void _Ready()
	{
		gameTimer = GetNode<Timer>("/root/world1/Timers/PlebTimer");
		dieTimer = GetNode<Timer>("DieTimer");
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		detailPopup = GetNode<DetailPopup>("/root/world1/Hud/DetailPopup");
		
		gameTimer.Timeout += () => gameTimer_Tick();

		name = GenerateName();
		speed += rnd.Next(-10, 15);
		hunger += rnd.Next(25);
	}
	
	public override void _PhysicsProcess(double delta)
	{
	}
	
	private void gameTimer_Tick()
	{
		if (isDead)
			return;
		if (health <= 0)
		{
			Die();
			return;
		}
		if  (hunger <= 0)
			health--;
		else
			hunger--;

		//healing when not hungry
		if (hunger > 80 && health < maxHealth)
		{ 
			health++;
			hunger--;
		}

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

	public void Animate()
	{
		if (Velocity == Vector2.Zero)
			sprite.Play("idle");
		else if (direction > Vector2.Zero)
		{
			sprite.FlipH = false;
			sprite.Play("walk");
		}
		else if (direction < Vector2.Zero)
		{
			sprite.FlipH = true;
			sprite.Play("walk");
		}
	}
	
	public void Die()
	{
		isDead = true;
		dieTimer.Start();
		sprite.Play("death");
		if (showDetails)
			detailPopup.CloseDetail();

		dieTimer.Timeout += () => { QueueFree(); };
	}
}
