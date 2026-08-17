using BaldursNet.Application.Interfaces;
using BaldursNet.Application.Services;
using BaldursNet.Presentation.ConsoleUI;

namespace BaldursNet.Domain.State;

public class MainMenuState : IGameState
{
  public void Update(GameEngine engine)
  {
    List<string> options = ["Iniciar Jogo", "Carregar Jogo", "Opções", "Sair"];
    SelectionMenuParams<string> menuParams = new(
      title: "BALDUR'S NET 10.0",
      items: options,
      canCancel: false
    );

    int selectedOption = engine.UI.RenderSelectibleMenu<string>(menuParams);

    switch (selectedOption)
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
}