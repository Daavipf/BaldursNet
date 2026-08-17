using BaldursNet.Presentation.ConsoleUI;

namespace BaldursNet.Application.Interfaces;

public interface IUserInterface
{
  int RenderSelectibleMenu<T>(SelectionMenuParams<T> menuParams);
  void ShowMessage(string message);
}