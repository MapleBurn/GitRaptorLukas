﻿using Godot;
using System;
using System.Collections.Generic;

public partial class LivingObject : CharacterBody2D, Entity
{
	public static string[] namePresets = ["Snorky", "Blimbo", "Wibbles", "Zonker", "Boinkle", "Squibble", "Doodle", "Fizzle", "Wobble", "Jiggly", "Fizzbin", "Noodle", "Plonker", "Goober", "Ignacio", "Dexter", "Lazaro", "Caius", "Amias", "Orion", "Zephyr", "Lucian", "Thaddeus", "Cassian", "Erasmus", "Balthazar", "Isidore", "Phineas", "Leandro", "Octavian", "Quillon", "Dario", "Alaric", "Simeon", "Tiberius", "Steve", "Mike", "David", "John", "Mark", "Michael", "Scott", "Jeff", "Paul", "Chris", "Robert", "Dave", "Tom", "Jim", "Tim", "Kevin", "Joe", "Brian", "Domini", "Dara", "Destry", "Elowen", "Gaia", "Gloria", "Garnet", "Harbor", "Harlow", "Lucretia", "Lumen", "Lura", "Lux", "Tansy", "Conquest", "Victor", "Sunny", "Chase", "Wade", "Clay", "Reed", "Sky", "Ash", "Dawn", "River", "Flint", "Coral", "Pearl", "Haven", "Frost", "Echo", "Breeze", "Blaze", "Stone", "Vale", "Meadow"];
	
	public Timer gameTimer;
	public Timer dieTimer;
	public Vector2 direction = Vector2.Zero;
	public AnimatedSprite2D sprite;
	public float baseSpeed = 50f;
	public float speed;
	public NavigationAgent2D navAgent;
	public static Rid navMap;
	private bool initialized;
	
	public static readonly Random rdm = new Random();
	
	public int maxHealth = 100;
	public int health;
	public float regenThreshold = 80f;
	public int maxHunger = 100;
	public int hunger;
	public string name = "Name";
	public bool favorite = false;
	public string team = "none";
	
	public bool isDead;
	public bool isOnWater;
	
	public bool showDetails;
	private static DetailPopup detailPopup;
	private static TileMapLayer map;
	
	public override void _Ready()
	{
		if (!initialized)
			Initialize();
		gameTimer.Timeout += () => gameTimer_Tick();

		PrepareStats();
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

	public void Initialize()
	{
		gameTimer = GetNode<Timer>("/root/world1/Timers/GameTimer");
		dieTimer = GetNode<Timer>("DieTimer");
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		detailPopup = GetNode<DetailPopup>("/root/world1/Hud/DetailPopup");
		map = GetNode<TileMapLayer>("/root/world1/worldgen/GroundTiles");
		navAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		initialized = true;
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
