using System;
using System.Collections.Generic;
using System.Linq;
using BaldursNet.Domain.Entities;
using BaldursNet.Presentation.ConsoleUI;

namespace BaldursNet.Presentation.UIComponents;

public static class RoomTabs
{
  public static List<TabInfo> InitializeTabs(Room currentRoom, Player player)
  {
    var tabs = new List<TabInfo>();

    // 1. Aba Saídas
    var exits = currentRoom.GetAvailableExits();
    tabs.Add(new TabInfo
    {
      Title = $"Saídas ({exits.Count})",
      Items = exits.Cast<object>().ToList(),
      DisplaySelector = item =>
      {
        var exit = (RoomExit)item;
        var lockInfo = exit.IsLocked ? " [Trancada 🔒]" : "";
        return $"{exit.DisplayName}{lockInfo}";
      }
    });

    // 2. Aba Inimigos (se houver inimigos vivos)
    var enemies = currentRoom.GetAliveEnemies();
    if (enemies.Count > 0)
    {
      tabs.Add(new TabInfo
      {
        Title = $"Inimigos ({enemies.Count})",
        Items = enemies.Cast<object>().ToList(),
        DisplaySelector = item =>
        {
          var enemy = (Enemy)item;
          return $"⚔ {enemy.Name} (Nv. {enemy.Level}) [PV: {enemy.CurrentHp}/{enemy.MaxHp}]";
        }
      });
    }

    // 3. Aba Personagens / NPCs (se houver)
    var npcs = currentRoom.GetNpcs();
    if (npcs.Count > 0)
    {
      tabs.Add(new TabInfo
      {
        Title = $"Personagens ({npcs.Count})",
        Items = npcs.Cast<object>().ToList(),
        DisplaySelector = item =>
        {
          var npc = (Npc)item;
          var role = npc.IsMerchant ? " [Mercador 💰]" : "";
          return $"💬 {npc.Name}{role}";
        }
      });
    }

    // 4. Aba Recipientes / Containers (se houver)
    var containers = currentRoom.GetContainers();
    if (containers.Count > 0)
    {
      tabs.Add(new TabInfo
      {
        Title = $"Recipientes ({containers.Count})",
        Items = containers.Cast<object>().ToList(),
        DisplaySelector = item =>
        {
          var cont = (Container)item;
          var lockInfo = cont.IsLocked ? " [Trancado 🔒]" : "";
          return $"📦 {cont.Name} ({cont.Inventory.Items.Count} itens, {cont.Inventory.CurrentWeight}/{cont.Capacity} kg){lockInfo}";
        }
      });
    }

    // 5. Aba Itens no Chão (se houver)
    var worldItems = currentRoom.GetWorldItems();
    if (worldItems.Count > 0)
    {
      tabs.Add(new TabInfo
      {
        Title = $"Itens no Chão ({worldItems.Count})",
        Items = worldItems.Cast<object>().ToList(),
        DisplaySelector = item =>
        {
          var wi = (WorldItem)item;
          return $"✨ {wi.Item.Name} {wi.Item.GetStatSummary()}";
        }
      });
    }

    // 6. Aba Herói / Mochila (sempre acessível para ver status e inventário)
    tabs.Add(new TabInfo
    {
      Title = "Herói & Mochila",
      Items = new List<object> { player },
      DisplaySelector = _ => $"Abrir Mochila & Equipamentos ({player.Inventory.CurrentWeight}/{player.Inventory.MaxWeight} kg)"
    });

    return tabs;
  }

  public static void RenderTabs(List<TabInfo> tabs, int selectedTabIndex)
  {
    Console.WriteLine();
    for (int i = 0; i < tabs.Count; i++)
    {
      if (i == selectedTabIndex)
      {
        Console.Write(" ");
        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.Black;
        Console.Write($"  {tabs[i].Title}  ");
        Console.ResetColor();
        Console.Write(" ");
      }
      else
      {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($"  {tabs[i].Title}  ");
        Console.ResetColor();
      }
    }
    Console.WriteLine("\n" + new string('─', Math.Min(Console.WindowWidth > 0 ? Console.WindowWidth - 2 : 78, 78)));
  }
}
