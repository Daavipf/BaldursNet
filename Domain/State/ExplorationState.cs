using BaldursNet.Application.Interfaces;
using BaldursNet.Application.Services;
using BaldursNet.Presentation.ConsoleUI;

namespace BaldursNet.Domain.State;

public class ExplorationState : IGameState
{
  public void Update(GameEngine engine)
  {
    var exits = engine.CurrentRoom.GetAvailableExits();

    if (exits.Count == 0)
    {
      engine.UI.ShowMessage("Não há saídas nesta sala.");
      engine.ChangeState(new MainMenuState());
      return;
    }

    SelectionMenuParams<Room> menuParams = new(
      items: exits,
      title: engine.CurrentRoom.Name,
      description: engine.CurrentRoom.Description,
      displaySelector: exit => exit.Name,
      prompt: "\n[↑/↓] Selecionar  |  [Enter] Entrar  |  [ESC] Voltar"
    );

    int selectedOption = engine.UI.RenderSelectibleMenu<Room>(menuParams);

    if (selectedOption == -1)
    {
      engine.ChangeState(new MainMenuState());
      return;
    }

    engine.CurrentRoom = exits[selectedOption];
  }
}