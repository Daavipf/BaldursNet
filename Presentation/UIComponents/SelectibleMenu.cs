using System;
using BaldursNet.Presentation.ConsoleUI;

namespace BaldursNet.Presentation.UIComponents;

public static class SelectibleMenu
{
  public static void ShowMenu<T>(SelectionMenuParams<T> menuParams, int selectedIndex)
  {
    if (menuParams.Items.Count == 0)
    {
      Console.ForegroundColor = ConsoleColor.DarkGray;
      Console.WriteLine("   [Nenhum item disponível]");
      Console.ResetColor();
      return;
    }

    for (int i = 0; i < menuParams.Items.Count; i++)
    {
      string text = menuParams.DisplaySelector != null
        ? menuParams.DisplaySelector(menuParams.Items[i])
        : menuParams.Items[i]?.ToString() ?? string.Empty;

      if (i == selectedIndex)
      {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"  ► {text}");
        Console.ResetColor();
      }
      else
      {
        Console.WriteLine($"    {text}");
      }
    }

    if (!string.IsNullOrEmpty(menuParams.Prompt))
    {
      Console.ForegroundColor = ConsoleColor.DarkYellow;
      Console.WriteLine(menuParams.Prompt);
      Console.ResetColor();
    }
  }
}
