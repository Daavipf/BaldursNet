using BaldursNet.Application.Interfaces;
using BaldursNet.Application.Services;

namespace BaldursNet.State;

public class MainMenuState : IGameState
{
  private readonly IUserInterface UI;
  private int SelectionMenuIndex;

  public MainMenuState(IUserInterface ui)
  {
    UI = ui;
  }
  public void Update(GameEngine engine)
  {
    switch (SelectionMenuIndex)
    {
      case 0:
        engine.ChangeState(new ExplorationState(engine.ExplorationUI));
        break;
      case 1:
        engine.ChangeState(new LoadMenuState());
        break;
      case 2:
        engine.ChangeState(new OptionsMenuState());
        break;
      case 3:
        engine.ChangeState(null);
        break;
    }
  }

  public void Render(GameEngine engine)
  {
    SelectionMenuIndex = UI.RenderScreen<string>(engine);
  }
}