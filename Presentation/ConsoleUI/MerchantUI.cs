using System;
using System.Collections.Generic;
using BaldursNet.Domain.Entities;

namespace BaldursNet.Presentation.ConsoleUI;

public class MerchantUI
{
  public enum ShopAction
  {
    None,
    Buy,
    Sell,
    Close
  }

  public (ShopAction Action, Item? Item) Render(Npc merchant, Player player, string? message = null)
  {
    int mode = 0; // 0 = Comprar, 1 = Vender
    int buyIndex = 0;
    int sellIndex = 0;

    var stock = merchant.MerchantStock?.Items ?? [];

    while (true)
    {
      Console.Clear();
      Console.ForegroundColor = ConsoleColor.Cyan;
      Console.WriteLine($"=== COMÉRCIO COM: {merchant.Name.ToUpper()} ===");
      Console.ResetColor();

      Console.ForegroundColor = ConsoleColor.Yellow;
      Console.Write($" Seu Ouro: {player.Gold} 🪙   |   ");
      Console.ForegroundColor = ConsoleColor.White;
      Console.WriteLine($"Carga da Mochila: {player.Inventory.CurrentWeight} / {player.Inventory.MaxWeight} kg");
      Console.ResetColor();
      Console.WriteLine(new string('─', 78));

      Console.ForegroundColor = ConsoleColor.Cyan;
      Console.Write(mode == 0 ? " [ COMPRAR MERCADORIAS ] " : "   Comprar Mercadorias   ");
      Console.ResetColor();
      Console.Write("    ");
      Console.ForegroundColor = ConsoleColor.Cyan;
      Console.WriteLine(mode == 1 ? " [ VENDER DA MOCHILA ] " : "   Vender da Mochila   ");
      Console.ResetColor();
      Console.WriteLine(new string('─', 78));

      if (mode == 0)
      {
        if (stock.Count == 0)
        {
          Console.ForegroundColor = ConsoleColor.DarkGray;
          Console.WriteLine("  [O mercador não tem mais produtos em estoque]");
          Console.ResetColor();
        }
        else
        {
          for (int i = 0; i < stock.Count; i++)
          {
            var it = stock[i];
            string prefix = (i == buyIndex) ? " ► " : "   ";
            bool canAfford = player.Gold >= it.Value;
            if (i == buyIndex)
            {
              Console.ForegroundColor = canAfford ? ConsoleColor.Green : ConsoleColor.Red;
              Console.WriteLine($"{prefix}{it.Name} - Preço: {it.Value} 🪙 | Peso: {it.Weight} kg {it.GetStatSummary()}");
              Console.ResetColor();
              Console.ForegroundColor = ConsoleColor.DarkGray;
              Console.WriteLine($"       └ {it.Description}");
              Console.ResetColor();
            }
            else
            {
              Console.ForegroundColor = canAfford ? ConsoleColor.White : ConsoleColor.DarkGray;
              Console.WriteLine($"{prefix}{it.Name} - Preço: {it.Value} 🪙 | Peso: {it.Weight} kg {it.GetStatSummary()}");
              Console.ResetColor();
            }
          }
        }
      }
      else
      {
        var bagItems = player.Inventory.Items;
        if (bagItems.Count == 0)
        {
          Console.ForegroundColor = ConsoleColor.DarkGray;
          Console.WriteLine("  [Sua mochila está vazia para venda]");
          Console.ResetColor();
        }
        else
        {
          for (int i = 0; i < bagItems.Count; i++)
          {
            var it = bagItems[i];
            int sellValue = Math.Max(1, it.Value / 2);
            string prefix = (i == sellIndex) ? " ► " : "   ";
            if (i == sellIndex)
            {
              Console.ForegroundColor = ConsoleColor.Green;
              Console.WriteLine($"{prefix}{it.Name} - Valor de Venda: {sellValue} 🪙 | Peso: {it.Weight} kg {it.GetStatSummary()}");
              Console.ResetColor();
            }
            else
            {
              Console.WriteLine($"{prefix}{it.Name} - Valor de Venda: {sellValue} 🪙 | Peso: {it.Weight} kg {it.GetStatSummary()}");
            }
          }
        }
      }

      if (!string.IsNullOrEmpty(message))
      {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($" * {message}");
        Console.ResetColor();
      }

      Console.ForegroundColor = ConsoleColor.DarkYellow;
      Console.WriteLine("\n" + new string('─', 78));
      Console.WriteLine("[←/→] Alternar Comprar/Vender  |  [↑/↓] Navegar  |  [Enter] Confirmar  |  [ESC] Voltar");
      Console.ResetColor();

      var key = Console.ReadKey(intercept: true).Key;
      if (key == ConsoleKey.LeftArrow || key == ConsoleKey.RightArrow)
      {
        mode = (mode == 0) ? 1 : 0;
      }
      else if (key == ConsoleKey.UpArrow)
      {
        if (mode == 0 && stock.Count > 0)
        {
          buyIndex = (buyIndex <= 0) ? stock.Count - 1 : buyIndex - 1;
        }
        else if (mode == 1 && player.Inventory.Items.Count > 0)
        {
          sellIndex = (sellIndex <= 0) ? player.Inventory.Items.Count - 1 : sellIndex - 1;
        }
      }
      else if (key == ConsoleKey.DownArrow)
      {
        if (mode == 0 && stock.Count > 0)
        {
          buyIndex = (buyIndex >= stock.Count - 1) ? 0 : buyIndex + 1;
        }
        else if (mode == 1 && player.Inventory.Items.Count > 0)
        {
          sellIndex = (sellIndex >= player.Inventory.Items.Count - 1) ? 0 : sellIndex + 1;
        }
      }
      else if (key == ConsoleKey.Enter)
      {
        if (mode == 0 && stock.Count > 0 && buyIndex < stock.Count)
        {
          return (ShopAction.Buy, stock[buyIndex]);
        }
        else if (mode == 1 && player.Inventory.Items.Count > 0 && sellIndex < player.Inventory.Items.Count)
        {
          return (ShopAction.Sell, player.Inventory.Items[sellIndex]);
        }
      }
      else if (key == ConsoleKey.Escape)
      {
        return (ShopAction.Close, null);
      }
    }
  }
}
