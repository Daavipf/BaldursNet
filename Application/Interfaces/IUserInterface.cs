using BaldursNet.Presentation.ConsoleUI;

namespace BaldursNet.Application.Interfaces;

public interface IUserInterface
{
  void RenderSelectibleMenu<T>(SelectionMenuParams<T> menuParams, int selectedIndex);
  void ShowMessage(string message);
  void RenderTabs(List<string> tabs);
  int RenderScreen<T>(SelectionMenuParams<T> menuParams, List<string>? tabs);
}