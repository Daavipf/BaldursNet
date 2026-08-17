using BaldursNet.Application.Interfaces;
using BaldursNet.Application.Services;

namespace BaldursNet.State;

public class LoadMenuState : IGameState
{
  public void Update(GameEngine engine)
  {
    engine.UI.ShowMessage("Em desenvolvimento");
    engine.ChangeState(new MainMenuState());
  }
}