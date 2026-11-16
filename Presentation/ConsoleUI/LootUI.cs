using System;
using System.Collections.Generic;
using BaldursNet.Domain.Entities;

namespace BaldursNet.Presentation.ConsoleUI;

public class LootUI
{
  public enum LootAction
  {
    None,
    TakeItem,
    StoreItem,
    TakeAll,
    StoreAll,
    Close
  }

  public (LootAction Action, Item? TargetItem) Render(Container container, Player player, string? statusMessage = null)
  {
    int currentSide = 0; // 0 = Container, 1 = Jogador
    int containerIndex = 0;
    int playerIndex = 0;

    while (true)
    {
      Console.Clear();

      Console.ForegroundColor = ConsoleColor.Cyan;
      Console.WriteLine($"=== TRANSFERÊNCIA DE ITENS: {container.Name.ToUpper()} ===");
      Console.ResetColor();
      Console.WriteLine($"{container.Description}\n");

      // Painéis lado a lado ou cabeçalhos
      Console.ForegroundColor = currentSide == 0 ? ConsoleColor.Green : ConsoleColor.White;
      Console.Write($"  [ {container.Name} ] ");
      Console.ResetColor();
      Console.ForegroundColor = ConsoleColor.DarkGray;
      Console.Write($"({container.Inventory.CurrentWeight}/{container.Capacity} kg)");
      Console.ResetColor();

      Console.Write("     ◄►     ");

      Console.ForegroundColor = currentSide == 1 ? ConsoleColor.Green : ConsoleColor.White;
      Console.Write($"  [ Mochila de {player.Name} ] ");
      Console.ResetColor();
      Console.ForegroundColor = ConsoleColor.DarkGray;
      Console.WriteLine($"({player.Inventory.CurrentWeight}/{player.Inventory.MaxWeight} kg)");
      Console.ResetColor();

      Console.WriteLine(new string('─', 78));

      if (currentSide == 0)
      {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"► ITENS EM {container.Name.ToUpper()}:");
        Console.ResetColor();

        if (container.Inventory.Items.Count == 0)
        {
          Console.ForegroundColor = ConsoleColor.DarkGray;
          Console.WriteLine("  [Recipiente Vazio]");
          Console.ResetColor();
        }
        else
        {
          for (int i = 0; i < container.Inventory.Items.Count; i++)
          {
            var item = container.Inventory.Items[i];
            string prefix = (i == containerIndex) ? " ► " : "   ";
            if (i == containerIndex)
            {
              Console.ForegroundColor = ConsoleColor.Green;
              Console.WriteLine($"{prefix}{item.Name} - {item.Weight} kg {item.GetStatSummary()}");
              Console.ResetColor();
            }
            else
            {
              Console.WriteLine($"{prefix}{item.Name} - {item.Weight} kg {item.GetStatSummary()}");
            }
          }
        }
      }
      else
      {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"► ITENS NA SUA MOCHILA:");
        Console.ResetColor();

        if (player.Inventory.Items.Count == 0)
        {
          Console.ForegroundColor = ConsoleColor.DarkGray;
          Console.WriteLine("  [Sua Mochila está Vazia]");
          Console.ResetColor();
        }
        else
        {
          for (int i = 0; i < player.Inventory.Items.Count; i++)
          {
            var item = player.Inventory.Items[i];
            string prefix = (i == playerIndex) ? " ► " : "   ";
            if (i == playerIndex)
            {
              Console.ForegroundColor = ConsoleColor.Green;
              Console.WriteLine($"{prefix}{item.Name} - {item.Weight} kg {item.GetStatSummary()}");
              Console.ResetColor();
            }
            else
            {
              Console.WriteLine($"{prefix}{item.Name} - {item.Weight} kg {item.GetStatSummary()}");
            }
          }
        }
      }

      if (!string.IsNullOrEmpty(statusMessage))
      {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($" * {statusMessage}");
        Console.ResetColor();
      }

      Console.ForegroundColor = ConsoleColor.DarkYellow;
      Console.WriteLine("\n" + new string('─', 78));
      Console.WriteLine("[←/→] Mudar de Lado  |  [↑/↓] Navegar  |  [Enter] Transferir  |  [T] Pegar Tudo  |  [ESC] Fechar");
      Console.ResetColor();

      var key = Console.ReadKey(intercept: true).Key;

      if (key == ConsoleKey.LeftArrow || key == ConsoleKey.RightArrow)
      {
        currentSide = (currentSide == 0) ? 1 : 0;
      }
      else if (key == ConsoleKey.UpArrow)
      {
        if (currentSide == 0 && container.Inventory.Items.Count > 0)
        {
          containerIndex = (containerIndex <= 0) ? container.Inventory.Items.Count - 1 : containerIndex - 1;
        }
        else if (currentSide == 1 && player.Inventory.Items.Count > 0)
        {
          playerIndex = (playerIndex <= 0) ? player.Inventory.Items.Count - 1 : playerIndex - 1;
        }
      }
      else if (key == ConsoleKey.DownArrow)
      {
        if (currentSide == 0 && container.Inventory.Items.Count > 0)
        {
          containerIndex = (containerIndex >= container.Inventory.Items.Count - 1) ? 0 : containerIndex + 1;
        }
        else if (currentSide == 1 && player.Inventory.Items.Count > 0)
        {
          playerIndex = (playerIndex >= player.Inventory.Items.Count - 1) ? 0 : playerIndex + 1;
        }
      }
      else if (key == ConsoleKey.Enter)
      {
        if (currentSide == 0 && container.Inventory.Items.Count > 0 && containerIndex < container.Inventory.Items.Count)
        {
          return (LootAction.TakeItem, container.Inventory.Items[containerIndex]);
        }
        else if (currentSide == 1 && player.Inventory.Items.Count > 0 && playerIndex < player.Inventory.Items.Count)
        {
          return (LootAction.StoreItem, player.Inventory.Items[playerIndex]);
        }
      }
      else if (key == ConsoleKey.T)
      {
        return (LootAction.TakeAll, null);
      }
      else if (key == ConsoleKey.G)
      {
        return (LootAction.StoreAll, null);
      }
      else if (key == ConsoleKey.Escape)
      {
        return (LootAction.Close, null);
      }
    }
  }
}
