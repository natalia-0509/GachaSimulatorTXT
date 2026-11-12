using GachaSimulator.Controllers;
using GachaSimulator.Models;

class Program
{
    static void Main(string[] args)
    {
        var gameController = new GameController();
        gameController.Start();
    }
}

