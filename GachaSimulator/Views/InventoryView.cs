using GachaSimulator.Models;
using Spectre.Console;

namespace GachaSimulator.Views;

public class InventoryView
{
    public void Render(Player player, int selectedIndex)
    {
        Console.SetCursorPosition(0,0);

        var rightGrid = new Grid();
        rightGrid.AddColumn();

        if (player.Inventory.Count == 0)
        {
            rightGrid.AddRow(new Markup("[grey]Brak wybranego przedmiotu.[/]"));
            rightGrid.AddRow(new Markup("\n[grey]Otwórz kilka pakietów, aby zapełnić swoją kolekcję![/]"));
        }
        else
        {
            var item = player.Inventory[selectedIndex];
            string colorTag = GetRarityColor(item.ItemRarity);
            rightGrid.AddRow(new Markup($"[bold]Nazwa:[/] {colorTag}{item.Name}[/]"));
            rightGrid.AddRow(new Markup($"[bold]Rzadkość:[/] {colorTag}{item.ItemRarity}[/]\n"));
            rightGrid.AddRow(new Markup($"[bold cyan]Atak:[/] {item.AttackValue} pkt"));
            rightGrid.AddRow(new Markup($"[bold cyan]Obrona:[/] {item.DefenseValue} pkt"));
            rightGrid.AddRow(new Markup($"[bold cyan]Życie:[/] {item.HealthValue} pkt"));
            rightGrid.AddRow(new Markup($"[bold yellow]Wartość sprzedaży:[/] {item.MarketValue} monet\n"));
            rightGrid.AddRow(new Markup("[bold]Opis:[/]"));
            rightGrid.AddRow(new Markup($"[italic]{item.Description}[/]"));
        }

        var rightPanel = new Panel(rightGrid)
            .Header("[bold green]Statystyki postaci[/]")
            .Expand()
            .BorderColor(Color.Green);
        rightPanel.Height = 17;
        
        
        
        var leftGrid = new Grid();
        leftGrid.AddColumn();
        leftGrid.AddRow(new Markup(
            $"Portfel: [bold yellow] {player.Coins} monet[/] | Ilość Postaci: [bold]{player.Inventory.Count}[/]\n"));
        leftGrid.AddRow(new Markup("[grey] GÓRA/DÓŁ - wybór | S - sprzedaj[/]"));
        leftGrid.AddRow(new Markup("[grey] R - reset | /ESC - powrót[/]\n"));
        
        if (player.Inventory.Count == 0)
        {
            leftGrid.AddRow(new Markup("[yellow]Twój ekwipunek jest obecnie pusty.[/]"));   
        }
        else
        {
            int maxVisable = 12;
            int startIndex = Math.Max(0, selectedIndex - maxVisable / 2);
            int endIndex = Math.Min(player.Inventory.Count, startIndex + maxVisable);

            for (int i = startIndex; i < endIndex; i++)
            {
                var currentItem = player.Inventory[i];
                string colorTag = GetRarityColor(currentItem.ItemRarity);

                if (i == selectedIndex)
                {
                    leftGrid.AddRow(new Markup($"[bold invert]> {currentItem.Name} ({currentItem.MarketValue}m) <[/]"));
                }
                else
                {
                    leftGrid.AddRow(
                        new Markup($"  {colorTag}{currentItem.Name}[/] [grey]({currentItem.MarketValue}m)[/]"));
                }
            }
        }
        var leftPanel = new Panel(leftGrid)
            .Header("[bold yellow]Twoja Kolekcja (Ekwipunek)[/]")
            .Expand()
            .BorderColor(Color.Yellow);
        leftPanel.Height = 17;

        var layout = new Table()
            .Border(TableBorder.None)
            .HideHeaders()
            .Expand();

        layout.AddColumn(new TableColumn("").Width(46));
        layout.AddColumn(new TableColumn(""));
        layout.AddRow(leftPanel, rightPanel);

        AnsiConsole.Write(layout);
    }

    private string GetRarityColor(Rarity rarity)
    {
        return rarity switch
        {
            Rarity.Common => "[white]",
            Rarity.Rare => "[dodgerblue2]",
            Rarity.Epic => "[fuchsia]",
            Rarity.Legendary => "[gold1]",
            _ => "[white]"
        };
    }
}