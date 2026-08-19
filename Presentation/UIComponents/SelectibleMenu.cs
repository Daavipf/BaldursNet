using BaldursNet.Presentation.ConsoleUI;

namespace BaldursNet.Presentation.UIComponents;

public static class SelectibleMenu
{
  public static void ShowMenu<T>(SelectionMenuParams<T> menuParams, int selectedIndex)
  {
    for (int i = 0; i < menuParams.Items.Count; i++)
    {
      if (i == selectedIndex)
      {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($" > {menuParams.DisplaySelector!(menuParams.Items[i])}");
        Console.ResetColor();
      }
      else
      {
        Console.WriteLine($"   {menuParams.DisplaySelector!(menuParams.Items[i])}");
      }
    }

    Console.WriteLine($"\n{menuParams.Prompt}");
  }
}