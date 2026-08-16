using BaldursNet.ConsoleUI;

namespace BaldursNet;

public class Program
{
  static void Main(string[] args)
  {
    ISelectionMenu consoleMenu = new SelectionMenu();
    IUserInterface userInterface = new UserInterface(consoleMenu);
    IWorldLoader worldLoader = new JsonWorldLoader("world.json");
    var engine = new GameEngine(userInterface, worldLoader);
    engine.Start();
  }
}