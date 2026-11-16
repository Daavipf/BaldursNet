using System;
using System.Collections.Generic;
using BaldursNet.Domain.Entities;

namespace BaldursNet.Presentation.ConsoleUI;

public class DialogueUI
{
  public enum DialogueAction
  {
    None,
    OpenShop,
    Rest,
    Leave
  }

  public DialogueAction Render(Npc npc, Player player)
  {
    var options = new List<(string Label, DialogueAction Action)>();

    if (npc.IsMerchant && npc.MerchantStock != null)
    {
      options.Add(("Ver Mercadorias à Venda 💰", DialogueAction.OpenShop));
    }

    if (npc.Id == "bartender")
    {
      options.Add(("Descansar na Taverna (Recupera todos os PV e PM por 10 moedas)", DialogueAction.Rest));
    }

    options.Add(("Despedir-se", DialogueAction.Leave));

    int selected = 0;
    while (true)
    {
      Console.Clear();
      Console.ForegroundColor = ConsoleColor.Cyan;
      Console.WriteLine($"=== CONVERSANDO COM: {npc.Name.ToUpper()} ===");
      Console.ResetColor();
      Console.WriteLine($"{npc.Description}\n");

      Console.ForegroundColor = ConsoleColor.Yellow;
      Console.WriteLine("── Mensagens do Personagem ──");
      Console.ResetColor();

      if (npc.DialogueLines.Count == 0)
      {
        Console.WriteLine($"\"{npc.Name} olha para você em silêncio e acena com a cabeça.\"");
      }
      else
      {
        foreach (var line in npc.DialogueLines)
        {
          Console.WriteLine($" * \"{line}\"");
        }
      }

      Console.WriteLine("\n" + new string('─', 78));
      Console.WriteLine("O que você deseja fazer?");

      for (int i = 0; i < options.Count; i++)
      {
        if (i == selected)
        {
          Console.ForegroundColor = ConsoleColor.Green;
          Console.WriteLine($" ► {options[i].Label}");
          Console.ResetColor();
        }
        else
        {
          Console.WriteLine($"   {options[i].Label}");
        }
      }

      Console.ForegroundColor = ConsoleColor.DarkYellow;
      Console.WriteLine("\n[↑/↓] Escolher  |  [Enter] Confirmar  |  [ESC] Sair");
      Console.ResetColor();

      var key = Console.ReadKey(intercept: true).Key;
      if (key == ConsoleKey.UpArrow)
      {
        selected = (selected <= 0) ? options.Count - 1 : selected - 1;
      }
      else if (key == ConsoleKey.DownArrow)
      {
        selected = (selected >= options.Count - 1) ? 0 : selected + 1;
      }
      else if (key == ConsoleKey.Enter)
      {
        return options[selected].Action;
      }
      else if (key == ConsoleKey.Escape)
      {
        return DialogueAction.Leave;
      }
    }
  }
}
