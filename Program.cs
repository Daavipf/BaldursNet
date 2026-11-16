using System;
using System.Text;
using BaldursNet.Application.Interfaces;
using BaldursNet.Application.Services;
using BaldursNet.Infrastructure.Data;
using BaldursNet.Tests;

namespace BaldursNet;

public class Program
{
  static void Main(string[] args)
  {
    try
    {
      Console.OutputEncoding = Encoding.UTF8;
      Console.Title = "Baldur's Net 10.0 - RPG de Terminal";
    }
    catch
    {
      // Ignora se o ambiente do terminal não suportar mudança de título
    }

    if (args.Length > 0 && (args[0] == "--test" || args[0] == "test"))
    {
      bool success = SystemVerification.RunAllTests();
      Environment.Exit(success ? 0 : 1);
      return;
    }

    var itemRepo = new ItemRepository("Data/items.json");
    var enemyRepo = new EnemyRepository(itemRepo, "Data/enemies.json");
    IWorldLoader worldLoader = new WorldLoader("Data/world.json", itemRepo, enemyRepo);

    var engine = new GameEngine(worldLoader);
    engine.Start();
  }
}
