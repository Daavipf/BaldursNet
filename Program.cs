using BaldursNet.Application.Interfaces;
using BaldursNet.Application.Services;
using BaldursNet.Presentation.ConsoleUI;

namespace BaldursNet;

public class Program
{
  static void Main(string[] args)
  {
    IUserInterface userInterface = new UserInterface();
    IWorldLoader worldLoader = new JsonWorldLoader("world.json");
    var engine = new GameEngine(userInterface, worldLoader);
    engine.Start();
  }
}