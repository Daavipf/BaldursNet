using BaldursNet.Application.Interfaces;
using BaldursNet.Application.Services;
using BaldursNet.Domain.Entities;
using BaldursNet.Presentation.UIComponents;

namespace BaldursNet.Presentation.ConsoleUI;

public class ExplorationUI : IUserInterface
{
  public int RenderScreen<T>(GameEngine engine)
  {
    var exits = engine.CurrentRoom.GetAvailableExits();

    SelectionMenuParams<Room> menuParams = new(
      items: exits,
      title: engine.CurrentRoom.Name,
      description: engine.CurrentRoom.Description,
      displaySelector: exit => exit.Name,
      prompt: "\n[↑/↓] Selecionar  |  [Enter] Entrar  |  [ESC] Voltar"
    );

    if (menuParams.Items == null || menuParams.Items.Count == 0)
      return -1;

    int selectedIndex = 0;
    menuParams.DisplaySelector ??= (item => item?.ToString() ?? string.Empty);

    while (true)
    {
      Console.Clear();

      Console.WriteLine($"=== {menuParams.Title} ===");
      if (menuParams.Description != null)
        Console.WriteLine($"{menuParams.Description}");

      SelectibleMenu.ShowMenu(menuParams, selectedIndex);

      int? selectedOption = CaptureInputKey(menuParams.Items.Count, menuParams.CanCancel, ref selectedIndex);

      if (selectedOption.HasValue)
      {
        return selectedOption.Value;
      }
    }
  }

  public void ShowMessage(string message)
  {
    Console.WriteLine(message);
    Console.ReadKey(true);
    return;
  }

  public void RenderTabs(List<string> tabs)
  {
    Console.WriteLine(string.Join(" | ", tabs));
    Console.WriteLine(new string('-', Console.WindowWidth > 0 ? Console.WindowWidth : 30));
  }

  private int? CaptureInputKey(int menuParamsItemsCount, bool menuParamsCanCancel, ref int selectedIndex)
  {
    var key = Console.ReadKey(intercept: true).Key;

    if (key == ConsoleKey.UpArrow)
    {
      selectedIndex = (selectedIndex == 0) ? menuParamsItemsCount - 1 : selectedIndex - 1;
    }
    else if (key == ConsoleKey.DownArrow)
    {
      selectedIndex = (selectedIndex == menuParamsItemsCount - 1) ? 0 : selectedIndex + 1;
    }
    else if (key == ConsoleKey.Enter)
    {
      return selectedIndex;
    }
    else if (menuParamsCanCancel && key == ConsoleKey.Escape)
    {
      return -1;
    }

    return null;
  }
}