using BaldursNet.Application.Interfaces;
using BaldursNet.Application.Services;

namespace BaldursNet.State;

public class MainMenuState : IGameState
{
  private readonly IUserInterface _ui;
  private int _selectionIndex = -1;

  public MainMenuState(IUserInterface ui)
  {
    _ui = ui;
  }

  public void Update(GameEngine engine)
  {
    switch (_selectionIndex)
    {
      case 0: // Iniciar / Continuar Jogo
        engine.ChangeState(new ExplorationState(engine.ExplorationUI));
        break;

      case 1: // Guia & Mecânicas
        engine.PushState(new LoadMenuState());
        break;

      case 2: // Reiniciar Novo Jogo
        engine.RestartGame();
        break;

      case 3: // Sair
        engine.ChangeState(null);
        break;
    }

    _selectionIndex = -1;
  }

  public void Render(GameEngine engine)
  {
    var result = _ui.RenderScreen(engine);
    _selectionIndex = result?.Index ?? 3;
  }
}
