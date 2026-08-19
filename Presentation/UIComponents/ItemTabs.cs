using BaldursNet.Domain.Entities;
using BaldursNet.Presentation.ConsoleUI;

namespace BaldursNet.Presentation.UIComponents;

public static class ItemTabs
{
  public static List<TabInfo> InitializeTabs(List<Room> exits, List<GameObject> gameObjects)
  {
    var tabs = new List<TabInfo>
    {
      new TabInfo
      {
        Title = "Saídas",
        Items = exits.Cast<object>().ToList(),
        DisplaySelector = item => ((Room)item).Name
      }
    };

    if (gameObjects != null)
    {
      var groupedObjects = gameObjects.GroupBy(o => o.GetType().Name);

      foreach (var group in groupedObjects)
      {
        tabs.Add(new TabInfo
        {
          Title = group.Key,
          Items = group.Cast<object>().ToList(),
          DisplaySelector = item => ((GameObject)item).Name
        });
      }
    }

    return tabs;
  }
  public static void RenderTabs(List<TabInfo> tabs, int selectedTabIndex)
  {
    for (int i = 0; i < tabs.Count; i++)
    {
      if (i == selectedTabIndex)
      {
        Console.Write("   ");
        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.Black;
        Console.Write($" {tabs[i].Title} ");
        Console.ResetColor();
      }
      else
      {
        Console.Write($"    {tabs[i].Title} ");
      }
    }
    Console.WriteLine();
  }
}