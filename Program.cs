using BaldursNet.ConsoleUI;

namespace BaldursNet;

public class Program
{
  static void Main(string[] args)
  {
    ISelectionMenu consoleMenu = new SelectionMenu();
    IUserInterface userInterface = new UserInterface(consoleMenu);
    var engine = new GameEngine(userInterface);
    engine.Start();
  }
}