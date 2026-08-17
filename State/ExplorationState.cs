using BaldursNet.Application.Interfaces;
using BaldursNet.Application.Services;
using BaldursNet.Presentation.ConsoleUI;
using BaldursNet.State.Enums;

namespace BaldursNet.State;

public class ExplorationState : IGameState
{
  private int SelectionMenuIndex;
  private ExplorationTab CurrentTab = ExplorationTab.Exits;

  public void Update(GameEngine engine)
  {
    var exits = engine.CurrentRoom.GetAvailableExits();

    /*if (exits.Count == 0)
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

    int selectedOption = engine.UI.RenderSelectibleMenu<Room>(menuParams);*/

    if (SelectionMenuIndex == -1)
    {
      engine.ChangeState(new MainMenuState());
      return;
    }

    engine.CurrentRoom = exits[SelectionMenuIndex];
  }

  public void Render(GameEngine engine)
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

    SelectionMenuIndex = engine.UI.RenderScreen<Room>(menuParams, ["Salas", "Personagens", "PoIs"]);
  }
}