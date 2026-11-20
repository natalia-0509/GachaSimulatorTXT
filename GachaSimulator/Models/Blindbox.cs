using System.Collections.Generic;
using System.Linq;

namespace GachaSimulator.Models;

public class Blindbox
{
    public int Id {get;  set;}
    public string CategoryName { get; set; }
    public int Cost { get; set; }
    public List<Item> PossibleItems { get; set; }
    
    private Random _random =  new Random();
    
    public Blindbox (string name, int cost,  List<Item> possibleItems)
    {
        CategoryName = name;
        Cost = cost;
        PossibleItems = possibleItems;
    }

    public Item Open()
    {
        int roll = _random.Next(1, 101);
        Rarity rolledRarity;

        if (roll < 60)
            rolledRarity = Rarity.Common;
        else if (roll <= 85)
            rolledRarity = Rarity.Rare;
        else if (roll <= 98)
            rolledRarity = Rarity.Epic;
        else
            rolledRarity = Rarity.Legendary;
        
        var itemsOfRarity = PossibleItems.Where(i =>i.ItemRarity == rolledRarity).ToList();
        if (itemsOfRarity.Count == 0)
        {
            return PossibleItems[_random.Next(PossibleItems.Count)];
        }
        return itemsOfRarity[_random.Next(itemsOfRarity.Count)];
    }
}