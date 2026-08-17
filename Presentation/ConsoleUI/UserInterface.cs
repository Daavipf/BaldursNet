using BaldursNet.Application.Interfaces;

namespace BaldursNet.Presentation.ConsoleUI;

public class UserInterface : IUserInterface
{
  public int RenderScreen<T>(SelectionMenuParams<T> menuParams, List<string>? tabs)
  {
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

      if (tabs != null && tabs.Count > 0)
      {
        Console.WriteLine();
        RenderTabs(tabs);
        Console.WriteLine();
      }

      RenderSelectibleMenu(menuParams, selectedIndex);

      int? selectedOption = CaptureInputKey(menuParams.Items.Count, menuParams.CanCancel, ref selectedIndex);

      if (selectedOption.HasValue)
      {
        return selectedOption.Value;
      }
    }
  }

  public void RenderTabs(List<string> tabs)
  {
    Console.WriteLine(string.Join(" | ", tabs));
    Console.WriteLine(new string('-', Console.WindowWidth > 0 ? Console.WindowWidth : 30));
  }

  public void RenderSelectibleMenu<T>(SelectionMenuParams<T> menuParams, int selectedIndex)
  {
    for (int i = 0; i < menuParams.Items.Count; i++)
    {
      if (i == selectedIndex)
      {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($" > {menuParams.DisplaySelector(menuParams.Items[i])}");
        Console.ResetColor();
      }
      else
      {
        Console.WriteLine($"   {menuParams.DisplaySelector(menuParams.Items[i])}");
      }
    }

    Console.WriteLine($"\n{menuParams.Prompt}");
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

  public void ShowMessage(string message)
  {
    Console.WriteLine(message);
    Console.ReadKey(true);
    return;
  }
}