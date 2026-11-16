using System;
using System.Collections.Generic;
using BaldursNet.Application.Interfaces;
using BaldursNet.Application.Services;
using BaldursNet.Presentation.UIComponents;

namespace BaldursNet.Presentation.ConsoleUI;

public class ExplorationUI : IUserInterface
{
  public (string? Tab, int Index)? RenderScreen(GameEngine engine)
  {
    var tabs = RoomTabs.InitializeTabs(engine.CurrentRoom, engine.Player);

    int currentTabIndex = 0;
    int selectedIndex = 0;

    while (true)
    {
      Console.Clear();

      // Barra de status superior do herói
      RenderHeroStatusBar(engine);

      // Cabeçalho da sala
      Console.ForegroundColor = ConsoleColor.Cyan;
      Console.WriteLine($"\n=== {engine.CurrentRoom.Name.ToUpper()} ===");
      Console.ResetColor();

      if (!string.IsNullOrEmpty(engine.CurrentRoom.Description))
      {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine($"{engine.CurrentRoom.Description}");
        Console.ResetColor();
      }

      // Abas
      if (currentTabIndex >= tabs.Count) currentTabIndex = 0;
      var currentTab = tabs[currentTabIndex];

      RoomTabs.RenderTabs(tabs, currentTabIndex);

      // Conteúdo da aba selecionada
      if (currentTab.Items.Count == 0)
      {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("\n  [Vazio - Não há nada nesta seção]\n");
        Console.ResetColor();
      }
      else
      {
        SelectionMenuParams<object> menuParams = new(
          items: currentTab.Items,
          title: currentTab.Title,
          description: null,
          displaySelector: currentTab.DisplaySelector,
          prompt: "\n[←/→] Mudar Aba  |  [↑/↓] Selecionar  |  [Enter] Interagir  |  [I] Mochila  |  [ESC] Menu"
        );

        SelectibleMenu.ShowMenu(menuParams, selectedIndex);
      }

      // Log de mensagens recentes
      RenderMessageLog(engine);

      // Captura de entrada
      var key = CaptureInputKey(currentTab.Items.Count, ref selectedIndex, ref currentTabIndex, tabs.Count);

      if (key == ConsoleKey.Enter && currentTab.Items.Count > 0)
      {
        return (currentTab.Title, selectedIndex);
      }
      else if (key == ConsoleKey.I)
      {
        // Atalho para abrir inventário
        return ("Herói & Mochila", 0);
      }
      else if (key == ConsoleKey.Escape)
      {
        return null;
      }
    }
  }

  private static void RenderHeroStatusBar(GameEngine engine)
  {
    var p = engine.Player;
    Console.ForegroundColor = ConsoleColor.DarkCyan;
    Console.WriteLine(new string('═', 78));
    Console.ResetColor();

    Console.Write(" HERÓI: ");
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write($"{p.Name} (Nv. {p.Level})");
    Console.ResetColor();

    Console.Write("  |  PV: ");
    Console.ForegroundColor = p.CurrentHp > p.TotalMaxHp * 0.3 ? ConsoleColor.Green : ConsoleColor.Red;
    Console.Write($"{p.CurrentHp}/{p.TotalMaxHp}");
    Console.ResetColor();

    Console.Write("  |  PM: ");
    Console.ForegroundColor = ConsoleColor.Blue;
    Console.Write($"{p.CurrentMp}/{p.TotalMaxMp}");
    Console.ResetColor();

    Console.Write("  |  OURO: ");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write($"{p.Gold} 🪙");
    Console.ResetColor();

    Console.Write("  |  CARGA: ");
    Console.ForegroundColor = p.Inventory.IsFull ? ConsoleColor.Red : ConsoleColor.White;
    Console.Write($"{p.Inventory.CurrentWeight}/{p.Inventory.MaxWeight} kg");
    Console.ResetColor();

    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.DarkCyan;
    Console.WriteLine(new string('═', 78));
    Console.ResetColor();
  }

  private static void RenderMessageLog(GameEngine engine)
  {
    if (engine.MessageLog.Count == 0) return;

    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.DarkYellow;
    Console.WriteLine("── Diário de Ações ──");
    Console.ResetColor();

    foreach (var msg in engine.MessageLog)
    {
      Console.ForegroundColor = ConsoleColor.Yellow;
      Console.WriteLine($" * {msg}");
      Console.ResetColor();
    }
  }

  public void ShowMessage(string message)
  {
    Console.WriteLine(message);
    Console.WriteLine("\n[Pressione qualquer tecla para continuar]");
    Console.ReadKey(true);
  }

  private ConsoleKey CaptureInputKey(int itemsCount, ref int selectedIndex, ref int tabIndex, int tabsCount)
  {
    var key = Console.ReadKey(intercept: true).Key;

    if (key == ConsoleKey.UpArrow && itemsCount > 0)
    {
      selectedIndex = (selectedIndex <= 0) ? itemsCount - 1 : selectedIndex - 1;
    }
    else if (key == ConsoleKey.DownArrow && itemsCount > 0)
    {
      selectedIndex = (selectedIndex >= itemsCount - 1) ? 0 : selectedIndex + 1;
    }
    else if (key == ConsoleKey.LeftArrow)
    {
      tabIndex = (tabIndex <= 0) ? tabsCount - 1 : tabIndex - 1;
      selectedIndex = 0;
    }
    else if (key == ConsoleKey.RightArrow)
    {
      tabIndex = (tabIndex >= tabsCount - 1) ? 0 : tabIndex + 1;
      selectedIndex = 0;
    }

    return key;
  }
}
