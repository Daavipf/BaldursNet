using BaldursNet.Application.Services;

namespace BaldursNet.Application.Interfaces;

public interface IUserInterface
{
  void ShowMessage(string message);
  (string? Tab, int Index)? RenderScreen(GameEngine engine);
}