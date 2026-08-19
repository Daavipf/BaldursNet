using BaldursNet.Application.Factories;
using BaldursNet.Application.Interfaces;
using BaldursNet.Application.Services;
using BaldursNet.Presentation.ConsoleUI;

namespace BaldursNet;

public class Program
{
  static void Main(string[] args)
  {
    IUserInterface userInterface = new UserInterface();
    IGameObjectFactory gameObjectFactory = new GameObjectFactory();
    IRoomFactory roomFactory = new RoomFactory(gameObjectFactory);
    IWorldLoader worldLoader = new JsonWorldLoader("world.json", roomFactory);
    var engine = new GameEngine(userInterface, worldLoader);
    engine.Start();
  }
}