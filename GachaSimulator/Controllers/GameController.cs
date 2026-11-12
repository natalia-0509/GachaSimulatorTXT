using GachaSimulator.Models;
using GachaSimulator.Views;

namespace GachaSimulator.Controllers;

public class GameController
{
    private Player _player;
    private List<Blindbox> _boxes;
    private MainMenuView _view;
    private InventoryView _inventoryview;
    private int _selectedIndex = 0;

    public GameController()
    {
        _player = new Player("student");
        _boxes = GenerateSampleBoxes();
        _view = new MainMenuView();
        _inventoryview = new InventoryView();
    }

    public void Start()
    {
        Console.CursorVisible = false;
        Console.Clear();
        bool isRunning = true;

        while (isRunning)
        {
            _view.Render(_boxes, _selectedIndex, _player);
            var keyInfo = Console.ReadKey(true);
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    _selectedIndex--;
                    if (_selectedIndex < 0) _selectedIndex = _boxes.Count - 1;
                    break;
                case ConsoleKey.DownArrow:
                    _selectedIndex++;
                    if (_selectedIndex >= _boxes.Count) _selectedIndex = 0;
                    break;
                case ConsoleKey.Enter:
                    TryOpenBox(_boxes[_selectedIndex]);
                    Console.Clear();
                    break;
                case ConsoleKey.I:
                    OpenInventory();
                    Console.Clear();
                    break;
                case ConsoleKey.Escape:
                    isRunning = false;
                    break;
            }
        }
    }

    private void TryOpenBox(Blindbox box)
    {
        if (_player.CanAfford(box.Cost))
        {
            _player.DeductCoins(box.Cost);
            Item droppedItem = box.Open();
            _player.AddItemToInventory(droppedItem);
            
            // TODO: Tutaj wstawimy w przyszłości animację otwierania z nowego widoku (AnimationView)
            Console.Clear();
            Console.WriteLine($"\n[SYMULACJA ANIMACJI] Wylosowano: {droppedItem.Name} ({droppedItem.ItemRarity})!");
            Console.WriteLine("Wciśnij dowolny klawisz, aby wrócić do menu...");
            Console.ReadKey(true);
        }
        else
        {
            Console.Clear();
            Console.WriteLine("\nNie masz wystarczająco dużo monet na tę skrzynkę!");
            Console.WriteLine("Wciśnij dowolny klawisz, aby wrócić do menu...");
            Console.ReadKey(true);
        }
    }

    private List<Blindbox> GenerateSampleBoxes()
    {
        var fantasyItems = new List<Item>
        {
            new Item("Zardzewiały Miecz", Rarity.Common, "Tępy, ale działa.", 10, 5, 2, 5),
            new Item("Stalowy Sztylet", Rarity.Rare, "Ostra broń dla zwinnych.", 30, 15, 20, 50),
            new Item("Magiczny Pierścień", Rarity.Epic, "Lekko świeci w ciemności.", 55, 20, 25, 200),
            new Item("Miecz Tysiąca Prawd", Rarity.Legendary, "Broń ostateczna.", 100, 75, 50, 1000)
        };
        var scifiItems = new List<Item>
        {
            new Item("Stary Blaster", Rarity.Common, "Z zaciętym spustem.", 15, 10, 5, 20),
            new Item("Kombinezon Ochronny", Rarity.Rare, "Chroni przed radiacją.", 50, 30, 30, 80),
            new Item("Miecz Świetlny", Rarity.Epic, "Elegancka broń na bardziej cywilizowane czasy.", 100, 50, 25, 200),
            new Item("Dezintegrator", Rarity.Legendary, "Pozbądź się wszystkiego, wszędzie.", 150, 100, 75, 1500)
        };
        return new List<Blindbox>
        {
            new Blindbox("Skrzynka Fantasy", 150, fantasyItems),
            new Blindbox("Skrzynka Sci-Fi", 300, scifiItems),
            new Blindbox("Demo", 1000, new List<Item>())
        };
    }

    private void OpenInventory()
    {
        Console.Clear();
        int invIndex = 0;
        bool inInventory = true;

        while (inInventory)
        {
            if (invIndex >= _player.Inventory.Count)
            {
                invIndex = Math.Max(0, _player.Inventory.Count - 1);
            }
            _inventoryview.Render(_player, invIndex);
            var keyInfo = Console.ReadKey(true);
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    if (_player.Inventory.Count > 0)
                    {
                        invIndex--;
                        if (invIndex < 0) invIndex = _player.Inventory.Count - 1;
                    }
                    break;
                case ConsoleKey.DownArrow:
                    if (_player.Inventory.Count > 0)
                    {
                        invIndex++;
                        if (invIndex >= _player.Inventory.Count) invIndex = 0;
                    }
                    break;
                case ConsoleKey.S:
                    if (_player.Inventory.Count > 0)
                    {
                        _player.SellItems(_player.Inventory[invIndex]);
                            Console.Clear();
                    }
                    break;
                case ConsoleKey.I: 
                    case ConsoleKey.Escape:
                    inInventory = false;
                    break;
            }
        }
    }
}