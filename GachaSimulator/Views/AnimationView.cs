using GachaSimulator.Models;
using Spectre.Console;

namespace GachaSimulator.Views;

public class AnimationView
{
        public void PlayOpenAnimation (string boxName, Item droppedItem)
        {
            Console.Clear();
            string[] frames = new string[]
            {
                //Klatka 0
                "      +---------+      \n" +
                "     /         /|      \n" +
                "    +---------+ |      \n" +
                "    |  ?   ?  | +      \n" +
                "    |    ?    |/       \n" +
                "    +---------+        ",

                //Klatka 1
                "    +---------+  ))    \n" +
                "   /         /|        \n" +
                "  +---------+ |  ))    \n" +
                "  |  ?   ?  | +        \n" +
                "  |    ?    |/         \n" +
                "  +---------+          ",

                //Klatka 2
                "  ((  +---------+      \n" +
                "     /         /|      \n" +
                "  ((+---------+ |      \n" +
                "    |  ?   ?  | +      \n" +
                "    |    ?    |/       \n" +
                "    +---------+        ",

                //Klatka 3
                "   *  +---------+  *   \n" +
                "     /   \\ /   /|      \n" +
                "  + +---- X ----+ +    \n" +
                "    |  ! / \\ !  | +    \n" +
                "   *  |   !   |/  *    \n" +
                "      +---------+      ",

                //Klatka 4
                "    \\  *  |  *  /      \n" +
                "  *  '. \\ | / .'  *    \n" +
                "  ---  -- * --  ---    \n" +
                "  *  .' / | \\ '.  *    \n" +
                "    /  *  |  *  \\      \n" +
                "                       "

            };
            int[] sequence = { 0, 1, 2, 1, 2, 1, 2, 3, 3, 4 };
            int[] delayes = { 250, 180, 180, 130, 130, 90, 90, 220, 300 };

            for (int i = 0; i < frames.Length; i++)
            {
                Console.SetCursorPosition(0,0);

                int frameIndex = sequence[i];
                Color borderColor = frameIndex switch
                {
                    3 => Color.Yellow,
                    4 => GetSpectreColor(droppedItem.ItemRarity),
                    _ => Color.Blue
                };
                string statusText = frameIndex < 3
                    ? "[grey]Otwieranie pakietu...[/]"
                    : frameIndex == 3
                        ? "[bold yellow]Twoje życzenia się spełnia![/]"
                        : "[bold green] BUM![/]";
                var content = new Markup(
                $"[bold cyan]{boxName}[/]\n\n" +
                $"[bold white]{frames[frameIndex]}[/]\n\n" +
                $"{statusText}"
                );

                var panel = new Panel(Align.Center(content))
                    .Header("[bold yellow]LOSOWANIE[/]")
                    .BorderColor(borderColor);
                    panel.Width =50;
                
                AnsiConsole.Write(panel);
                Thread.Sleep(delayes[i]);
            }
            ShowResultCard(droppedItem);
        }

        private void ShowResultCard(Item Item)
        {
            Console.Clear();
            Console.SetCursorPosition(0, 0);

            string colorTag = GetMarkupColor(Item.ItemRarity);
            Color borderColor = GetSpectreColor(Item.ItemRarity);
            
            var boxBorder = Item.ItemRarity == Rarity.Legendary ? BoxBorder.Double : BoxBorder.Rounded;
            string rarityBanner = Item.ItemRarity == Rarity.Legendary
                ? $"[bold gold1] ★ {Item.ItemRarity} ★[/] "
                : $"[bold]{colorTag}{Item.ItemRarity}[/][/]";

            var resultGrid = new Grid();
            resultGrid.AddColumn();
            resultGrid.AddRow(Align.Center(new Markup(rarityBanner)));
            resultGrid.AddRow(Align.Center(new Markup($"\n [bold underline]{colorTag}{Item.Name}[/][/]\n")));
            resultGrid.AddRow(Align.Center(new Markup($"[italic grey]\"{Item.Description}\"[/]\n")));
            resultGrid.AddRow(Align.Center(new Markup($"[red]Atak:[/] [bold]{Item.AttackValue} pkt[/] | [dodgerblue2]Obrona:[/] [bold]{Item.DefenseValue} pkt[/] | [green]Życie:[/] [bold]{Item.HealthValue} pkt[/] | [yellow]Wartość:[/] [bold]{Item.MarketValue}  monet[/]\n")));
            resultGrid.AddRow(Align.Center(new Markup($"[grey]Wciśnij dowolny przycisk, aby kontynuować...[/]")));
            
            var resultPanel = new Panel(resultGrid)
                .Header("[bold green] ZDOBYTO NOWĄ POSTAĆ![/]")
                .Border(boxBorder)
                .BorderColor(borderColor);
            resultPanel.Width =54;
            
            AnsiConsole.Write(resultPanel);

            while (Console.KeyAvailable)
            {
                Console.ReadKey(true);
            }
            Console.ReadKey(true);
        }

        private string GetMarkupColor(Rarity Rarity) => Rarity switch
        {
            Rarity.Common => "[white]",
            Rarity.Rare => "[dodgerblue2]",
            Rarity.Epic => "[fuchsia]",
            Rarity.Legendary => "[gold1]",
            _ => "[white]"
        };

        private Color GetSpectreColor(Rarity Rarity) => Rarity switch
        {
            Rarity.Common => Color.White,
            Rarity.Rare => Color.DodgerBlue2,
            Rarity.Epic => Color.Fuchsia,
            Rarity.Legendary => Color.Gold1,
            _ => Color.White
        };

}