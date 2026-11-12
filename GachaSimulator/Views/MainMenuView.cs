using System.Text;
using Spectre.Console;
using GachaSimulator.Models;

namespace GachaSimulator.Views;

public class MainMenuView
{
    
    public void Render(List<Blindbox> boxes, int selectedIndex, Player player)
    {
        Console.SetCursorPosition(0, 0);
        
        var selectedBox = boxes[selectedIndex];

        var rightGrid = new Grid();
        rightGrid.AddColumn();
        rightGrid.AddRow(new Markup($"[bold yellow]Cena otwarcia:[/] {selectedBox.Cost} monet\n"));
        rightGrid.AddRow(new Markup("[bold]Możliwe przedmioty do zdobycia:[/]\n"));
        

        foreach (var item in selectedBox.PossibleItems)
        {
            string colorTag = item.ItemRarity switch
            {
                Rarity.Common => "[white]",
                Rarity.Rare => "[dodgerblue2]",
                Rarity.Epic => "[fuchsia]",
                Rarity.Legendary => "[gold1]",
                _ => "[white]"
            };
            rightGrid.AddRow(new Markup($"- {colorTag}{item.Name}[/] [grey]({item.ItemRarity})[/]"));
        }

        var rightPanel = new Panel(rightGrid)
            .Header($"[bold green]Statystyki: {selectedBox.CategoryName}[/]")
            .Expand()
            .BorderColor(Color.Green);
        
        var leftGrid = new Grid();
        leftGrid.AddColumn();

        leftGrid.AddRow(
            new Markup($"Witaj, [bold cyan]{player.Name}[/]! Portfel: [bold yellow]{player.Coins} monet[/]\n"));
        leftGrid.AddRow(new Markup("[grey]Strzałki GÓRA/DÓŁ - nawigacja[/]"));
        leftGrid.AddRow(new Markup("[grey]ENTER - otwórz,I - ekwipunek, ESC - wyjście[/]\n"));
        
        for (int i = 0; i < boxes.Count; i++)
        {
            if (i == selectedIndex)
            {
                leftGrid.AddRow(new Markup($"[bold invert]> {boxes[i].CategoryName} <[/]"));
            }
            else
            {
               leftGrid.AddRow(new Markup($"  {boxes[i].CategoryName}"));
                
            }
        }
        var leftPanel = new Panel(leftGrid)
            .Header("[bold blue]Wybierz skrzynkę[/]")
            .Expand()
            .BorderColor(Color.Blue);
        var columns = new Columns(leftPanel, rightPanel);
        AnsiConsole.Write(columns);
    }
}