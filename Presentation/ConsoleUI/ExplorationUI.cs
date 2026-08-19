using System;
using System.Collections.Generic;
using System.Linq;
using BaldursNet.Application.Interfaces;
using BaldursNet.Application.Services;
using BaldursNet.Domain.Entities;
using BaldursNet.Presentation.UIComponents;

namespace BaldursNet.Presentation.ConsoleUI;

public class ExplorationUI : IUserInterface
{
  public (string Tab, int Index)? RenderScreen(GameEngine engine)
  {
    var exits = engine.CurrentRoom.GetAvailableExits();

    var tabs = ItemTabs.InitializeTabs(exits, engine.CurrentRoom.Objects);

    int currentTabIndex = 0;
    int selectedIndex = 0;

    while (true)
    {
      Console.Clear();

      var currentTab = tabs[currentTabIndex];

      Console.WriteLine($"=== {engine.CurrentRoom.Name} ===");
      if (!string.IsNullOrEmpty(engine.CurrentRoom.Description))
        Console.WriteLine($"{engine.CurrentRoom.Description}\n");

      ItemTabs.RenderTabs(tabs, currentTabIndex);

      if (currentTab.Items.Count == 0)
      {
        Console.WriteLine("\n[Vazio]");
      }
      else
      {
        SelectionMenuParams<object> menuParams = new(
          items: currentTab.Items,
          title: currentTab.Title,
          description: null,
          displaySelector: currentTab.DisplaySelector,
          prompt: "\n[←/→] Mudar Aba  |  [↑/↓] Selecionar  |  [Enter] Interagir  |  [ESC] Voltar"
        );

        SelectibleMenu.ShowMenu(menuParams, selectedIndex);
      }

      var key = CaptureInputKey(currentTab.Items.Count, ref selectedIndex, ref currentTabIndex, tabs.Count);

      if (key == ConsoleKey.Enter && currentTab.Items.Count > 0)
      {
        return (currentTab.Title, selectedIndex);
      }
      else if (key == ConsoleKey.Escape)
      {
        return null;
      }
    }
  }

  public void ShowMessage(string message)
  {
    Console.WriteLine(message);
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