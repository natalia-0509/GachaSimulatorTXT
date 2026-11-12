namespace GachaSimulator.Models;

public enum Rarity 
{
	Common,
	Rare,
	Epic,
	Legendary
}

public class Item
{
	public string Name { get; set; }
	public Rarity ItemRarity { get; set; }
	public string Description { get; set; }

	public int AttackValue { get; set; }
	public int DefenseValue  { get; set; }
	public int HealthValue  { get; set; }
	public int MarketValue { get; set; }

	public Item(string name, Rarity rarity, string description, int attack, int defense, int health, int price)
		{
		Name = name;
		ItemRarity = rarity;
		Description = description;
		AttackValue = attack;
		DefenseValue = defense;
		HealthValue = health;
		MarketValue = price;
		}

}