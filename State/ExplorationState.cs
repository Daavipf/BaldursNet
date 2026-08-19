using BaldursNet.Application.Interfaces;
using BaldursNet.Application.Services;
using BaldursNet.Domain.Entities;
using BaldursNet.State.Enums;

namespace BaldursNet.State;

public class ExplorationState : IGameState
{
  private readonly IUserInterface UI;
  private int NextRoomIndex;
  private ExplorationTab CurrentTab;

  public ExplorationState(IUserInterface ui)
  {
    UI = ui;
    CurrentTab = ExplorationTab.Exits;
  }

  public void Update(GameEngine engine)
  {
    var exits = engine.CurrentRoom.GetAvailableExits();

    if (NextRoomIndex == -1)
    {
      // QUANDO TIVER A STATE STACK IMPLEMENTADA, AQUI VIRÁ
      // O POP DA STACK
      engine.ChangeState(new MainMenuState(engine.MainMenuUI));
      return;
    }

    engine.CurrentRoom = exits[NextRoomIndex];
  }

  public void Render(GameEngine engine)
  {
    var exits = engine.CurrentRoom.GetAvailableExits();

    if (exits.Count == 0)
    {
      UI.ShowMessage("Não há saídas nesta sala.");
      engine.ChangeState(new MainMenuState(engine.MainMenuUI));
      return;
    }

    NextRoomIndex = UI.RenderScreen<Room>(engine);
  }
}