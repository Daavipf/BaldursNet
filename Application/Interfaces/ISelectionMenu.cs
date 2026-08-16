namespace BaldursNet.ConsoleUI;

public interface ISelectionMenu
{
  int RenderSelectibleMenu<T>(SelectionMenuParams<T> menuParams);
  void ShowMessage(string message);
}