namespace GachaSimulator.Models;

public class Player
{
 public string Name { get; set; }
 public int Coins { get; set; }
 public List<Item> Inventory { get; set; }

 public Player(string name)
 {
  Name = name;
  Coins = 1000;
  Inventory = new List<Item>();
 }

 public void AddItemToInventory(Item item)
 {
  Inventory.Add(item);
 }

 public bool CanAfford(int price)
 {
  return Coins >= price;
 }

 public void DeductCoins(int amount)
 {
  Coins -= amount;
 }

 public void SellItems(Item item)
 {
  if (Inventory.Remove(item))
  {
   Coins += item.MarketValue;
  }
 }

 public void Reset()
 {
  Coins = 1000;
  Inventory.Clear();
 } 
 
}