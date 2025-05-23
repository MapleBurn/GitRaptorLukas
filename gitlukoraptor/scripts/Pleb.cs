using Godot;
using System;
using System.Collections.Generic;

public partial class Pleb : LivingObject
{
	
	//additional state data
	public bool isDead;
	public bool isOnWater;
	public enum MemoryKey { lastSeenBush, travelPoint };	//serves as an enum of all possible keys for the memory dictionary (made for mistake prevention)
	public Dictionary<MemoryKey, Vector2> memory = new Dictionary<MemoryKey, Vector2>();	//pleb will store useful information about what it has seen (for example the last bush pleb saw)
	
	public bool showDetails;
	private static DetailPopup detailPopup;
	private static TileMapLayer map;
	
	public override void PrepareStats()
	{
		name = GenerateName();
		baseSpeed += rdm.Next(-10, 15);
		speed = baseSpeed;
		health = maxHealth;
		hunger = (maxHunger / 100) * 80 + rdm.Next(-10, 20);
	}
}
