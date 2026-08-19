using BaldursNet.Application.Interfaces;
using BaldursNet.Application.Services;
using BaldursNet.Presentation.ConsoleUI;

namespace BaldursNet.State;

public class MainMenuState : IGameState
{
  private int SelectionMenuIndex;
  public void Update(GameEngine engine)
  {
    switch (SelectionMenuIndex)
    {
      case 0:
        engine.ChangeState(new ExplorationState());
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
    List<string> options = ["Iniciar Jogo", "Carregar Jogo", "Opções", "Sair"];
    SelectionMenuParams<string> menuParams = new(
      title: "BALDUR'S NET 10.0",
      items: options,
      canCancel: false
    );

    SelectionMenuIndex = engine.UI.RenderScreen<string>(menuParams, null);
  }
}