using BaldursNet.Application.Factories;
using BaldursNet.Application.Interfaces;
using BaldursNet.Application.Services;

namespace BaldursNet;

public class Program
{
  static void Main(string[] args)
  {
    IGameObjectFactory gameObjectFactory = new GameObjectFactory();
    IRoomFactory roomFactory = new RoomFactory(gameObjectFactory);
    IWorldLoader worldLoader = new JsonWorldLoader("world.json", roomFactory);
    var engine = new GameEngine(worldLoader);
    engine.Start();
  }
}