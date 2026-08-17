using BaldursNet.Application.Services;

namespace BaldursNet.Application.Interfaces;

public interface IGameState
{
  void Update(GameEngine engine);
  void Render(GameEngine engine);
}