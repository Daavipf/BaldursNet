using BaldursNet.Application.Interfaces;

namespace BaldursNet.Presentation.ConsoleUI;

public class UserInterface : IUserInterface
{
  public int RenderSelectibleMenu<T>(SelectionMenuParams<T> menuParams)
  {
    if (menuParams.Items == null || menuParams.Items.Count == 0)
      return -1;

    int selectedIndex = 0;
    menuParams.DisplaySelector ??= (item => item?.ToString() ?? string.Empty);

    while (true)
    {
      Console.Clear();
      Console.WriteLine($"=== {menuParams.Title} ===");
      if (menuParams.Description != null) Console.WriteLine($"{menuParams.Description}");
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
      var key = Console.ReadKey(intercept: true).Key;
      if (key == ConsoleKey.UpArrow)
      {
        selectedIndex = (selectedIndex == 0) ? menuParams.Items.Count - 1 : selectedIndex - 1;
      }
      else if (key == ConsoleKey.DownArrow)
      {
        selectedIndex = (selectedIndex == menuParams.Items.Count - 1) ? 0 : selectedIndex + 1;
      }
      else if (key == ConsoleKey.Enter)
      {
        return selectedIndex;
      }
      else if (menuParams.CanCancel && key == ConsoleKey.Escape)
      {
        return -1;
      }
    }
  }

  public void ShowMessage(string message)
  {
    Console.WriteLine(message);
    Console.ReadKey(true);
    return;
  }
}