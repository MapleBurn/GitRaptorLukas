using Godot;
using System;
using System.Collections.Generic;
using GDC = Godot.Collections;

public partial class LivingObject : CharacterBody2D, Entity
{
	//names
	public static string[] namePresets = ["Snorky", "Blimbo", "Wibbles", "Zonker", "Boinkle", "Squibble", "Doodle", "Fizzle", "Wobble", "Jiggly", "Fizzbin", "Noodle", "Plonker", "Goober", "Ignacio", "Dexter", "Lazaro", "Caius", "Amias", "Orion", "Zephyr", "Lucian", "Thaddeus", "Cassian", "Erasmus", "Balthazar", "Isidore", "Phineas", "Leandro", "Octavian", "Quillon", "Dario", "Alaric", "Simeon", "Tiberius", "Steve", "Mike", "David", "John", "Mark", "Michael", "Scott", "Jeff", "Paul", "Chris", "Robert", "Dave", "Tom", "Jim", "Tim", "Kevin", "Joe", "Brian", "Domini", "Dara", "Destry", "Elowen", "Gaia", "Gloria", "Garnet", "Harbor", "Harlow", "Lucretia", "Lumen", "Lura", "Lux", "Tansy", "Conquest", "Victor", "Sunny", "Chase", "Wade", "Clay", "Reed", "Sky", "Ash", "Dawn", "River", "Flint", "Coral", "Pearl", "Haven", "Frost", "Echo", "Breeze", "Blaze", "Stone", "Vale", "Meadow"];
	
	//nodes
	public Timer gameTimer;
	public Timer dieTimer;
	public Vector2 direction = Vector2.Zero;
	public AnimatedSprite2D sprite;
	public NavigationAgent2D navAgent;
	public static Rid navMap;
	private static DetailPopup detailPopup;
	public static TileMapLayer map;
	public bool initialized;

	
	//astar
	public static AStarGrid2D astarGrid = new AStarGrid2D();
	public static Vector2I mapSize;
	public static readonly Vector2I shallow = new Vector2I(0, 0);	//tile atlas coords for types of surfaces
	public static readonly Vector2I water = new Vector2I(1, 0);
	public static readonly Vector2I mountain = new Vector2I(5, 0);
	public GDC.Array<Vector2I> currentPath =  new GDC.Array<Vector2I>();
	
	//variables
	public static readonly Random rdm = new Random();
	public int maxHealth = 100;
	public int health;
	public float regenThreshold = 80f;
	public int maxHunger = 100;
	public int hunger;
	public float baseSpeed = 50f;
	public float speed;
	public string name = "Name";
	public bool favorite = false;
	public string team = "none";
	
	public bool isDead;
	public bool isOnWater;
	public bool showDetails;
	
	public override void _Ready()
	{
		if (!initialized)
			Initialize();
		gameTimer.Timeout += () => gameTimer_Tick();

		PrepareStats();
	}

	public void Initialize()
	{
		gameTimer = GetNode<Timer>("/root/world1/Timers/GameTimer");
		dieTimer = GetNode<Timer>("DieTimer");
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		detailPopup = GetNode<DetailPopup>("/root/world1/Hud/DetailPopup");
		map = GetNode<TileMapLayer>("/root/world1/worldgen/GroundTiles");
		navAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		InitializeAStarGrid();
		UpdateAStar();
		initialized = true;
	}
	
	private void InitializeAStarGrid()
	{
		mapSize = map.GetUsedRect().Size;
		astarGrid.Region = new Rect2I(0, 0, mapSize.X, mapSize.Y);
		astarGrid.CellSize = map.TileSet.TileSize;
		astarGrid.Update();
	}
	
	public override void _PhysicsProcess(double delta)
	{ 
		Vector2I tilePos = (Vector2I)(Position / 16);
		Vector2I Celltype = map.GetCellAtlasCoords(tilePos);
		if (Celltype == new Vector2I(0, 0) || Celltype == new Vector2I(1, 0))
			isOnWater = true;
		else
			isOnWater = false;
	}
	public virtual void PrepareStats()
	{
		speed = baseSpeed;
		health = maxHealth;
		hunger = maxHunger;
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
		
		if (hunger > (float)maxHunger / 100f * regenThreshold && health < maxHealth)
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
	
	public static string GenerateName()
	{
		string name = "";
		char[] vowels = ['a', 'e', 'i', 'o', 'u', '&'];
		char[] consonants = ['b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'y', 'z', '&', '&'];
		if (rdm.Next(5) > 1)
		{
			bool wasVowelLast = true;
			for (int i = 0; i <= 10; i++)
			{
				char letter;
				if (rdm.Next(100) < 70)
				{
					if (wasVowelLast)
					{
						letter = consonants[rdm.Next(consonants.Length)];
						wasVowelLast = false;
					}
					else
					{
						letter = vowels[rdm.Next(vowels.Length)];
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
			name = namePresets[rdm.Next(namePresets.Length)];
		}
		name = char.ToUpper(name[0]) + name.Substring(1);
		return name;
	}

	public void UpdateAStar()	//call this function every time the map changes
	{
		for (int x = 0; x < mapSize.X; x++)
		{
			for (int y = 0; y < mapSize.Y; y++)
			{
				Vector2I tileCoords = new Vector2I(x, y);
				Vector2I tileType = map.GetCellAtlasCoords(tileCoords);

				if (tileType == mountain)
					astarGrid.SetPointSolid(tileCoords, true); // Not walkable
				else
				{
					astarGrid.SetPointSolid(tileCoords, false);

					if (tileType == shallow || tileType == water)
						astarGrid.SetPointWeightScale(tileCoords, 5.0f); // High cost
					else
						astarGrid.SetPointWeightScale(tileCoords, 1.0f); // Normal land
				}
			}
		}
	}
	
	public virtual void Die()
	{
		isDead = true;
		dieTimer.Start();
		sprite.Play("death");
		if (showDetails)
			detailPopup.CloseDetail();

		dieTimer.Timeout += () => { QueueFree(); };
	}
}
