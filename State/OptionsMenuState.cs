using BaldursNet.Application.Interfaces;
using BaldursNet.Application.Services;
using BaldursNet.Presentation.ConsoleUI;

namespace BaldursNet.State;

public class OptionsMenuState : IGameState
{
  public void Update(GameEngine engine)
  {
    Console.WriteLine("Em desenvolvimento");
    engine.ChangeState(new MainMenuState(new MainMenuUI()));
  }

  public void Render(GameEngine engine)
  {
    Console.WriteLine("Em desenvolvimento");
    engine.ChangeState(new MainMenuState(new MainMenuUI()));
  }
}