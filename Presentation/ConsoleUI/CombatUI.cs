using System;
using System.Collections.Generic;
using BaldursNet.Domain.Entities;
using BaldursNet.Domain.Enums;

namespace BaldursNet.Presentation.ConsoleUI;

public class CombatUI
{
  public enum CombatAction
  {
    None,
    Attack,
    Skill,
    UseItem,
    Defend,
    Flee
  }

  public (CombatAction Action, object? Extra) RenderCombatTurn(Player player, Enemy enemy, List<string> combatLog)
  {
    var mainOptions = new List<(string Label, CombatAction Action)>
    {
      ("Atacar com Arma Principal", CombatAction.Attack),
      ("Habilidade / Magia Especial", CombatAction.Skill),
      ("Usar Item Consumível (Mochila / Atalhos)", CombatAction.UseItem),
      ("Postura Defensiva (Reduz 50% do dano e recupera PM)", CombatAction.Defend),
      ("Tentar Fugir do Combate", CombatAction.Flee)
    };

    int selected = 0;

    while (true)
    {
      Console.Clear();

      RenderCombatHeader(player, enemy);

      // Renderiza Log de Combate
      Console.WriteLine("── Relatório do Confronto ──");
      int logStart = Math.Max(0, combatLog.Count - 6);
      for (int i = logStart; i < combatLog.Count; i++)
      {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($" > {combatLog[i]}");
        Console.ResetColor();
      }
      Console.WriteLine(new string('─', 78));

      Console.ForegroundColor = ConsoleColor.Cyan;
      Console.WriteLine("SUA VEZ DE AGIR:");
      Console.ResetColor();

      for (int i = 0; i < mainOptions.Count; i++)
      {
        if (i == selected)
        {
          Console.ForegroundColor = ConsoleColor.Green;
          Console.WriteLine($" ► {mainOptions[i].Label}");
          Console.ResetColor();
        }
        else
        {
          Console.WriteLine($"   {mainOptions[i].Label}");
        }
      }

      Console.ForegroundColor = ConsoleColor.DarkYellow;
      Console.WriteLine("\n[↑/↓] Escolher Ação  |  [Enter] Confirmar");
      Console.ResetColor();

      var key = Console.ReadKey(intercept: true).Key;

      if (key == ConsoleKey.UpArrow)
      {
        selected = (selected <= 0) ? mainOptions.Count - 1 : selected - 1;
      }
      else if (key == ConsoleKey.DownArrow)
      {
        selected = (selected >= mainOptions.Count - 1) ? 0 : selected + 1;
      }
      else if (key == ConsoleKey.Enter)
      {
        var action = mainOptions[selected].Action;

        if (action == CombatAction.Skill)
        {
          var skill = SelectSkill(player);
          if (skill != null)
          {
            return (CombatAction.Skill, skill);
          }
        }
        else if (action == CombatAction.UseItem)
        {
          var item = SelectConsumable(player);
          if (item != null)
          {
            return (CombatAction.UseItem, item);
          }
        }
        else
        {
          return (action, null);
        }
      }
    }
  }

  private static Skill? SelectSkill(Player player)
  {
    int selected = 0;
    while (true)
    {
      Console.Clear();
      Console.ForegroundColor = ConsoleColor.Cyan;
      Console.WriteLine($"=== HABILIDADES & MAGIAS (PM Atual: {player.CurrentMp}/{player.TotalMaxMp}) ===");
      Console.ResetColor();

      for (int i = 0; i < player.Skills.Count; i++)
      {
        var sk = player.Skills[i];
        bool canCast = player.CurrentMp >= sk.ManaCost;
        string prefix = (i == selected) ? " ► " : "   ";
        string costStr = $"[{sk.ManaCost} PM]";

        if (i == selected)
        {
          Console.ForegroundColor = canCast ? ConsoleColor.Green : ConsoleColor.Red;
          Console.WriteLine($"{prefix}{sk.Name} {costStr} ({sk.Element}) - Poder: {sk.Power}");
          Console.ResetColor();
          Console.ForegroundColor = ConsoleColor.DarkGray;
          Console.WriteLine($"       └ {sk.Description}");
          Console.ResetColor();
        }
        else
        {
          Console.ForegroundColor = canCast ? ConsoleColor.White : ConsoleColor.DarkGray;
          Console.WriteLine($"{prefix}{sk.Name} {costStr} ({sk.Element}) - Poder: {sk.Power}");
          Console.ResetColor();
        }
      }

      Console.ForegroundColor = ConsoleColor.DarkYellow;
      Console.WriteLine("\n[↑/↓] Navegar  |  [Enter] Usar Habilidade  |  [ESC] Voltar");
      Console.ResetColor();

      var key = Console.ReadKey(intercept: true).Key;
      if (key == ConsoleKey.UpArrow)
      {
        selected = (selected <= 0) ? player.Skills.Count - 1 : selected - 1;
      }
      else if (key == ConsoleKey.DownArrow)
      {
        selected = (selected >= player.Skills.Count - 1) ? 0 : selected + 1;
      }
      else if (key == ConsoleKey.Enter)
      {
        var sk = player.Skills[selected];
        if (player.CurrentMp >= sk.ManaCost)
        {
          return sk;
        }
      }
      else if (key == ConsoleKey.Escape)
      {
        return null;
      }
    }
  }

  private static Item? SelectConsumable(Player player)
  {
    var consumables = new List<Item>();
    foreach (var item in player.Inventory.Items)
    {
      if (item.Type == ItemType.Consumable) consumables.Add(item);
    }

    if (consumables.Count == 0)
    {
      Console.Clear();
      Console.WriteLine("Você não possui nenhum item consumível na mochila!");
      Console.WriteLine("\n[Pressione qualquer tecla para voltar]");
      Console.ReadKey(true);
      return null;
    }

    int selected = 0;
    while (true)
    {
      Console.Clear();
      Console.ForegroundColor = ConsoleColor.Cyan;
      Console.WriteLine("=== USAR ITEM CONSUMÍVEL ===");
      Console.ResetColor();

      for (int i = 0; i < consumables.Count; i++)
      {
        var item = consumables[i];
        string prefix = (i == selected) ? " ► " : "   ";
        if (i == selected)
        {
          Console.ForegroundColor = ConsoleColor.Green;
          Console.WriteLine($"{prefix}{item.Name} {item.GetStatSummary()}");
          Console.ResetColor();
        }
        else
        {
          Console.WriteLine($"{prefix}{item.Name} {item.GetStatSummary()}");
        }
      }

      Console.ForegroundColor = ConsoleColor.DarkYellow;
      Console.WriteLine("\n[↑/↓] Selecionar  |  [Enter] Consumir  |  [ESC] Cancelar");
      Console.ResetColor();

      var key = Console.ReadKey(intercept: true).Key;
      if (key == ConsoleKey.UpArrow)
      {
        selected = (selected <= 0) ? consumables.Count - 1 : selected - 1;
      }
      else if (key == ConsoleKey.DownArrow)
      {
        selected = (selected >= consumables.Count - 1) ? 0 : selected + 1;
      }
      else if (key == ConsoleKey.Enter)
      {
        return consumables[selected];
      }
      else if (key == ConsoleKey.Escape)
      {
        return null;
      }
    }
  }

  private static void RenderCombatHeader(Player player, Enemy enemy)
  {
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"⚔══════════════════════════ ARENA DE COMBATE ══════════════════════════⚔");
    Console.ResetColor();

    // Inimigo
    Console.ForegroundColor = ConsoleColor.DarkRed;
    Console.Write($" INIMIGO: ");
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write($"{enemy.Name} (Nv. {enemy.Level} - {enemy.EnemyType})");
    Console.ResetColor();
    Console.WriteLine();

    Console.Write(" PV: ");
    RenderBar(enemy.CurrentHp, enemy.MaxHp, ConsoleColor.Red);
    Console.WriteLine($" {enemy.CurrentHp} / {enemy.MaxHp} PV  |  ATQ: {enemy.BaseAttack}  |  DEF: {enemy.BaseDefense}");

    Console.WriteLine(new string('─', 78));

    // Jogador
    Console.ForegroundColor = ConsoleColor.DarkGreen;
    Console.Write($" JOGADOR: ");
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write($"{player.Name} (Nv. {player.Level})");
    Console.ResetColor();
    Console.WriteLine();

    Console.Write(" PV: ");
    RenderBar(player.CurrentHp, player.TotalMaxHp, ConsoleColor.Green);
    Console.WriteLine($" {player.CurrentHp} / {player.TotalMaxHp} PV  |  ATQ: {player.TotalAttack}  |  DEF: {player.TotalDefense}");

    Console.Write(" PM: ");
    RenderBar(player.CurrentMp, player.TotalMaxMp, ConsoleColor.Blue);
    Console.WriteLine($" {player.CurrentMp} / {player.TotalMaxMp} PM");

    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"⚔{new string('═', 76)}⚔\n");
    Console.ResetColor();
  }

  private static void RenderBar(int current, int max, ConsoleColor color)
  {
    int barLength = 20;
    int filled = max > 0 ? (int)Math.Round((double)current / max * barLength) : 0;
    filled = Math.Clamp(filled, 0, barLength);

    Console.Write("[");
    Console.ForegroundColor = color;
    Console.Write(new string('█', filled));
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.Write(new string('░', barLength - filled));
    Console.ResetColor();
    Console.Write("]");
  }

  public void ShowVictoryScreen(Player player, Enemy enemy, List<Item> drops, int xp, int gold, bool leveledUp)
  {
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("╔══════════════════════════════════════════════════════════════════════════════╗");
    Console.WriteLine($"║                             VITÓRIA GLORIOSA!                                ║");
    Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════════╝");
    Console.ResetColor();

    Console.WriteLine($"\nVocê derrotou {enemy.Name} com bravura!");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($" + {xp} Pontos de Experiência");
    Console.WriteLine($" + {gold} Moedas de Ouro");
    Console.ResetColor();

    if (leveledUp)
    {
      Console.ForegroundColor = ConsoleColor.Cyan;
      Console.WriteLine($"\n★★★ VOCÊ SUBIU DE NÍVEL! Agora você é Nível {player.Level}! ★★★");
      Console.WriteLine($"Atributos aumentados! Seus Pontos de Vida e Mana foram restaurados ao máximo!");
      Console.ResetColor();
    }

    if (drops.Count > 0)
    {
      Console.WriteLine("\nEspólios obtidos do monstro:");
      foreach (var drop in drops)
      {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($" * {drop.Name} {drop.GetStatSummary()}");
        Console.ResetColor();
      }
    }

    Console.ForegroundColor = ConsoleColor.DarkYellow;
    Console.WriteLine("\n[Pressione qualquer tecla para continuar]");
    Console.ResetColor();
    Console.ReadKey(true);
  }

  public void ShowDefeatScreen(Player player, Enemy enemy)
  {
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("╔══════════════════════════════════════════════════════════════════════════════╗");
    Console.WriteLine($"║                               VOCÊ FOI DERROTADO...                          ║");
    Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════════╝");
    Console.ResetColor();

    Console.WriteLine($"\nSeu herói sucumbiu aos golpes mortais de {enemy.Name}.");
    Console.WriteLine("Sua lenda se desvanece na escuridão...");

    Console.ForegroundColor = ConsoleColor.DarkYellow;
    Console.WriteLine("\n[Pressione qualquer tecla para retornar]");
    Console.ResetColor();
    Console.ReadKey(true);
  }
}
