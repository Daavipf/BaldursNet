using BaldursNet.ConsoleUI;

namespace BaldursNet;

public class Program
{
  static void Main(string[] args)
  {
    IConsoleMenu consoleMenu = new ConsoleMenu();
    var engine = new GameEngine(consoleMenu);
    engine.Start();
  }
}