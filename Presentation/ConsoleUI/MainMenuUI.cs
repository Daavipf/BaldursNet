using BaldursNet.Application.Interfaces;
using BaldursNet.Application.Services;
using BaldursNet.Presentation.UIComponents;

namespace BaldursNet.Presentation.ConsoleUI;

public class MainMenuUI : IUserInterface
{
  public int RenderScreen<T>(GameEngine engine)
  {
    List<string> items = ["Iniciar Jogo", "Carregar Jogo", "Opções", "Sair"];
    SelectionMenuParams<string> menuParams = new(
      title: "BALDUR'S NET 10.0",
      items: items,
      canCancel: false
    );

    if (menuParams.Items == null || menuParams.Items.Count == 0)
      return -1;

    int selectedIndex = 0;
    menuParams.DisplaySelector ??= (item => item?.ToString() ?? string.Empty);

    while (true)
    {
      Console.Clear();

      Console.WriteLine($"=== {menuParams.Title} ===");

      SelectibleMenu.ShowMenu<string>(menuParams, selectedIndex);

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