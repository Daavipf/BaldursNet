using System;
using System.Linq;
using BaldursNet.Application.Interfaces;
using BaldursNet.Application.Services;
using BaldursNet.Domain.Entities;
using BaldursNet.Domain.Enums;
using BaldursNet.Infrastructure.Data;

namespace BaldursNet.Tests;

public static class SystemVerification
{
  public static bool RunAllTests()
  {
    Console.WriteLine("=================================================");
    Console.WriteLine("  INICIANDO BATERIA DE TESTES DE SISTEMAS RPG    ");
    Console.WriteLine("=================================================\n");

    bool allPassed = true;

    allPassed &= TestJsonDataLoading();
    allPassed &= TestRoomGraphAndLockedExits();
    allPassed &= TestInventoryWeightAndTransfer();
    allPassed &= TestEquipmentSlotsAndAttributes();
    allPassed &= TestConsumableUsage();
    allPassed &= TestCombatTurnAndLevelUp();

    Console.WriteLine("\n=================================================");
    if (allPassed)
    {
      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine("  TODOS OS TESTES FORAM CONCLUÍDOS COM SUCESSO!  ");
      Console.ResetColor();
    }
    else
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine("  OCORRERAM FALHAS NOS TESTES!                   ");
      Console.ResetColor();
    }
    Console.WriteLine("=================================================");

    return allPassed;
  }

  private static bool TestJsonDataLoading()
  {
    Console.Write("[TESTE 1] Carregamento de Itens, Inimigos e Mundo via JSON... ");
    try
    {
      var itemRepo = new ItemRepository("Data/items.json");
      var enemyRepo = new EnemyRepository(itemRepo, "Data/enemies.json");
      var worldLoader = new WorldLoader("Data/world.json", itemRepo, enemyRepo);

      var items = itemRepo.GetAllItems();
      if (!items.ContainsKey("iron_sword") || !items.ContainsKey("potion_health_small") || !items.ContainsKey("crypt_key"))
      {
        throw new Exception("Itens esperados não foram carregados do catálogo JSON!");
      }

      var rat = enemyRepo.CreateEnemy("giant_rat");
      if (rat.Name != "Ratazana Carniceira" || rat.MaxHp != 28)
      {
        throw new Exception("Inimigo 'giant_rat' não foi carregado corretamente!");
      }

      var rooms = worldLoader.LoadWorld();
      if (!rooms.ContainsKey("tavern") || !rooms.ContainsKey("street") || !rooms.ContainsKey("sanctum"))
      {
        throw new Exception("Salas do grafo mundial não foram encontradas!");
      }

      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine("PASSOU (Itens, Inimigos e Salas carregados com sucesso)");
      Console.ResetColor();
      return true;
    }
    catch (Exception ex)
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine($"FALHOU: {ex.Message}");
      Console.ResetColor();
      return false;
    }
  }

  private static bool TestRoomGraphAndLockedExits()
  {
    Console.Write("[TESTE 2] Grafo de Ambientes e Saídas Trancadas com Chaves... ");
    try
    {
      var itemRepo = new ItemRepository("Data/items.json");
      var enemyRepo = new EnemyRepository(itemRepo, "Data/enemies.json");
      var worldLoader = new WorldLoader("Data/world.json", itemRepo, enemyRepo);
      var rooms = worldLoader.LoadWorld();

      var street = rooms["street"];
      var armoryExit = street.Exits.FirstOrDefault(e => e.TargetRoomId == "armory");
      if (armoryExit == null || !armoryExit.IsLocked || armoryExit.RequiredKeyId != "armory_key")
      {
        throw new Exception("Saída para armaria deveria estar trancada exigindo 'armory_key'!");
      }

      var player = new Player("Herói Teste");
      if (player.HasKey("armory_key"))
      {
        throw new Exception("Jogador não deveria ter a chave no início!");
      }

      var key = itemRepo.CreateItem("rusty_key");
      player.Inventory.AddItem(key);

      if (!player.HasKey("armory_key"))
      {
        throw new Exception("Jogador deveria reconhecer posse da chave 'armory_key'!");
      }

      if (!armoryExit.Unlock("armory_key") || armoryExit.IsLocked)
      {
        throw new Exception("A saída deveria ser destrancada com a chave correta!");
      }

      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine("PASSOU (Grafo interligado e sistema de chaves funcionando)");
      Console.ResetColor();
      return true;
    }
    catch (Exception ex)
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine($"FALHOU: {ex.Message}");
      Console.ResetColor();
      return false;
    }
  }

  private static bool TestInventoryWeightAndTransfer()
  {
    Console.Write("[TESTE 3] Inventário, Limite de Capacidade e Transferência... ");
    try
    {
      var chest = new Container(20, "Baú de Carvalho", "Um baú espaçoso", new Position(0,0,0));
      var player = new Player("Herói Teste");
      player.Inventory.MaxWeight = 10;
      player.Inventory.Clear();

      var sword = new Item { Id = "heavy_blade", Name = "Lâmina Pesada", Weight = 6 };
      var shield = new Item { Id = "heavy_shield", Name = "Escudo Pesado", Weight = 5 };

      chest.Inventory.AddItem(sword);
      chest.Inventory.AddItem(shield);

      // Transfere o primeiro item para o jogador
      if (!chest.Inventory.TransferTo(player.Inventory, sword, out _))
      {
        throw new Exception("Deveria conseguir transferir a espada (6 kg em mochila de 10 kg)!");
      }

      if (player.Inventory.CurrentWeight != 6 || chest.Inventory.CurrentWeight != 5)
      {
        throw new Exception($"Pesos calculados após transferência estão incorretos! Player: {player.Inventory.CurrentWeight}/10, Baú: {chest.Inventory.CurrentWeight}/20");
      }

      // Tenta transferir o segundo item (6 + 5 = 11 > 10): deve ser impedido pelo limite de peso do jogador!
      if (chest.Inventory.TransferTo(player.Inventory, shield, out _))
      {
        throw new Exception("Transferência não deveria ser permitida pois excede o peso máximo do jogador!");
      }

      // Transfere de volta da mochila para o baú
      if (!player.Inventory.TransferTo(chest.Inventory, sword, out _))
      {
        throw new Exception("Deveria conseguir transferir a espada de volta para o baú!");
      }

      if (player.Inventory.Items.Count != 0 || chest.Inventory.Items.Count != 2)
      {
        throw new Exception("Contagem de itens inconsistente após transferência reversa!");
      }

      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine("PASSOU (Capacidades e transferências respeitadas com exatidão)");
      Console.ResetColor();
      return true;
    }
    catch (Exception ex)
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine($"FALHOU: {ex.Message}");
      Console.ResetColor();
      return false;
    }
  }

  private static bool TestEquipmentSlotsAndAttributes()
  {
    Console.Write("[TESTE 4] Espaços de Equipamento e Atualização de Atributos... ");
    try
    {
      var player = new Player("Herói Teste");
      player.Inventory.Clear();
      int initialAttack = player.TotalAttack;
      int initialDefense = player.TotalDefense;
      int initialHp = player.TotalMaxHp;

      var sword = new Item
      {
        Id = "test_sword",
        Name = "Espada Sagrada",
        Type = ItemType.Weapon,
        AttackBonus = 15,
        TargetSlot = EquipmentSlot.Weapon,
        Weight = 3
      };

      var armor = new Item
      {
        Id = "test_armor",
        Name = "Armadura Dracônica",
        Type = ItemType.Armor,
        DefenseBonus = 12,
        TargetSlot = EquipmentSlot.Armor,
        Weight = 6
      };

      var ring = new Item
      {
        Id = "test_ring",
        Name = "Anel do Titã",
        Type = ItemType.Accessory,
        HpBonus = 50,
        TargetSlot = EquipmentSlot.Accessory,
        Weight = 1
      };

      player.Inventory.AddItem(sword);
      player.Inventory.AddItem(armor);
      player.Inventory.AddItem(ring);

      if (!player.Equip(sword, out _)) throw new Exception("Falha ao equipar arma!");
      if (!player.Equip(armor, out _)) throw new Exception("Falha ao equipar armadura!");
      if (!player.Equip(ring, out _)) throw new Exception("Falha ao equipar anel!");

      if (player.TotalAttack != initialAttack + 15)
      {
        throw new Exception($"Ataque total incorreto! Esperado: {initialAttack + 15}, Obtido: {player.TotalAttack}");
      }

      if (player.TotalDefense != initialDefense + 12)
      {
        throw new Exception($"Defesa total incorreta! Esperado: {initialDefense + 12}, Obtido: {player.TotalDefense}");
      }

      if (player.TotalMaxHp != initialHp + 50)
      {
        throw new Exception($"Vida máxima total incorreta! Esperado: {initialHp + 50}, Obtido: {player.TotalMaxHp}");
      }

      // Desequipar
      if (!player.Unequip(EquipmentSlot.Weapon, out _))
      {
        throw new Exception("Falha ao desequipar arma!");
      }

      if (player.TotalAttack != initialAttack)
      {
        throw new Exception("Ataque deveria ter voltado ao valor inicial!");
      }

      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine("PASSOU (Slots de equipamentos atualizam atributos perfeitamente)");
      Console.ResetColor();
      return true;
    }
    catch (Exception ex)
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine($"FALHOU: {ex.Message}");
      Console.ResetColor();
      return false;
    }
  }

  private static bool TestConsumableUsage()
  {
    Console.Write("[TESTE 5] Uso de Consumíveis e Restauração de PV/PM... ");
    try
    {
      var player = new Player("Herói Teste");
      player.CurrentHp = 20; // Ferido
      player.CurrentMp = 5;  // Pouca mana

      var potion = new Item
      {
        Id = "potion",
        Name = "Elixir Completo",
        Type = ItemType.Consumable,
        HealAmount = 40,
        ManaAmount = 20,
        Weight = 1
      };
      player.Inventory.AddItem(potion);

      if (!player.UseConsumable(potion, out var msg))
      {
        throw new Exception($"Falha ao usar poção: {msg}");
      }

      if (player.CurrentHp != 60 || player.CurrentMp != 25)
      {
        throw new Exception($"Valores de cura incorretos! HP={player.CurrentHp}, MP={player.CurrentMp}");
      }

      if (player.Inventory.Items.Contains(potion))
      {
        throw new Exception("Poção deveria ter sido removida do inventário após uso!");
      }

      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine("PASSOU (Consumíveis curam PV/PM e são consumidos)");
      Console.ResetColor();
      return true;
    }
    catch (Exception ex)
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine($"FALHOU: {ex.Message}");
      Console.ResetColor();
      return false;
    }
  }

  private static bool TestCombatTurnAndLevelUp()
  {
    Console.Write("[TESTE 6] Mecânicas de Combate, Drops de Loot e Level Up... ");
    try
    {
      var player = new Player("Herói Teste");
      var enemy = new Enemy("orc", "Orc Furioso", "Monstro forte", 40, 10, 8, 2, 2)
      {
        XpReward = 70,
        GoldReward = 30
      };

      var dropItem = new Item { Id = "ruby", Name = "Rubi Precioso", Weight = 1, Value = 50 };
      enemy.LootDrops.Add(new LootDropEntry { Item = dropItem, DropChance = 1.0 });

      // Aplica dano ao inimigo
      enemy.TakeDamage(40);
      if (enemy.IsAlive)
      {
        throw new Exception("Inimigo com 40 de dano deveria estar morto!");
      }

      var drops = enemy.GenerateLoot();
      if (drops.Count != 1 || drops[0].Id != "ruby")
      {
        throw new Exception("Drop de 100% de chance não foi gerado corretamente!");
      }

      int oldLevel = player.Level;
      bool leveledUp = player.GainXp(enemy.XpReward, out _);
      player.GainGold(enemy.GoldReward);

      if (!leveledUp || player.Level != oldLevel + 1)
      {
        throw new Exception("Jogador deveria ter subido de nível com 70 de XP!");
      }

      if (player.Gold < 30)
      {
        throw new Exception("Ouro do jogador não foi incrementado corretamente!");
      }

      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine("PASSOU (Combate, drops e subida de nível validados com sucesso)");
      Console.ResetColor();
      return true;
    }
    catch (Exception ex)
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine($"FALHOU: {ex.Message}");
      Console.ResetColor();
      return false;
    }
  }
}
