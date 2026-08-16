namespace BaldursNet.ConsoleUI;

public interface IConsoleMenu
{
  int RenderSelectibleMenu<T>(ConsoleMenuParams<T> menuParams);
  void ShowMessage(string message);
}