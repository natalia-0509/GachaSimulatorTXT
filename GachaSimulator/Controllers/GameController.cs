using GachaSimulator.Data;
using GachaSimulator.Models;
using GachaSimulator.Views;
using Spectre.Console;

namespace GachaSimulator.Controllers;

public class GameController
{
    private Player _player;
    private List<Blindbox> _boxes;
    private MainMenuView _view;
    private InventoryView _inventoryview;
    private AnimationView _animationView;
    private int _selectedIndex = 0;
    private DatabaseService _db;

    public GameController()
    {
        _player = new Player("student");
        _view = new MainMenuView();
        _inventoryview = new InventoryView();
        _animationView = new AnimationView();
        _db = new DatabaseService();
        _db.InitializeDatabase();
        _player = _db.LoadPlayer();
        _boxes = _db.LoadBlindBoxes();

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
            _db.UpdatePlayerCoins(_player.Coins);
            
            Item droppedItem = box.Open();
            _player.AddItemToInventory(droppedItem);
            _db.AddItemToInventory(droppedItem);
            
            _animationView.PlayOpenAnimation(box.CategoryName, droppedItem);
        }
        else
        {
            Console.Clear();
            var errorPanel = new Panel(new Markup("[bold red]Nie masz wystarczająco dużo monet, aby otworzyć tą skrzynkę![/]\n [grey]Sprzedaj postacie z ekwipunku (klawisz I), aby zdobyć złoto.[/]"))
                .Header("[bold red] Brak środków[/]")
                .BorderColor(Color.Red);
            AnsiConsole.Write(errorPanel);
            Console.ReadKey(true);
        }
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
                        var itemsToSell = _player.Inventory[invIndex];
                        _player.SellItems(itemsToSell);
                        _db.RemoveItemFromInventory(itemsToSell.Id);
                        _db.UpdatePlayerCoins(_player.Coins);
                        Console.Clear();
                    }
                    break;
                case ConsoleKey.R:
                    if (ConfirmReset())
                    {
                        _player.Reset();
                        _db.ResetPlayer();
                        invIndex = 0;
                    }
                    Console.Clear();
                    break;
                case ConsoleKey.I: 
                    case ConsoleKey.Escape:
                    inInventory = false;
                    break;
            }
        }
    }

    private bool ConfirmReset()
    {
        Console.Clear();
        var confirmPanel = new Panel(new Markup("[bold yellow]Czy na pewno chcesz zresetować swój profil?[/]\n" +
                                                "[grey]Wszystkie zebrane postacie zostaną usunięte, a stan konta wróci do 1000 monet.[/]\n\n" +
                                                "[bold green][[ENTER / T]][/] - Tak, resetuj    |    [bold red][[ESC / N]][/] - Anuluj"))
            .Header("[bold red]Potwierdzenie resetu[/]")
            .BorderColor(Color.Yellow);
        AnsiConsole.Write(confirmPanel);

        while (true)
        {
            var key = Console.ReadKey(true).Key;
            if(key == ConsoleKey.Enter || key == ConsoleKey.T)
                return  true;
            if (key == ConsoleKey.Escape || key ==ConsoleKey.N)
                return false;
        }
    }
}