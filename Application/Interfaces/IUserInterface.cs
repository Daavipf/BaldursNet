using BaldursNet.Application.Services;

namespace BaldursNet.Application.Interfaces;

public interface IUserInterface
{
  void ShowMessage(string message);
  int RenderScreen<T>(GameEngine gameEngine);
}