using BaldursNet.Application.Interfaces;
using BaldursNet.Application.Services;
using BaldursNet.Domain.Entities;
using BaldursNet.State.Enums;
using System.Linq;

namespace BaldursNet.State;

public class ExplorationState : IGameState
{
  private readonly IUserInterface UI;
  private string SelectedTab;
  private int SelectedIndex;

  public ExplorationState(IUserInterface ui)
  {
    UI = ui;
    SelectedTab = "Saídas";
    SelectedIndex = -1;
  }

  public void Update(GameEngine engine)
  {
    if (SelectedIndex == -1)
    {
      // QUANDO TIVER A STATE STACK IMPLEMENTADA, AQUI VIRÁ O POP
      engine.ChangeState(new MainMenuState(engine.MainMenuUI));
      return;
    }

    switch (SelectedTab)
    {
      case "Saídas":
        var exits = engine.CurrentRoom.GetAvailableExits();
        engine.CurrentRoom = exits[SelectedIndex];
        break;

      case "Character":
        var characters = engine.CurrentRoom.GetGameObjects<Character>();
        var selectedCharacter = characters[SelectedIndex];

        // Exemplo: engine.ChangeState(new DialogState(selectedCharacter));
        break;

      case "Container":
        var containers = engine.CurrentRoom.GetGameObjects<Container>();
        var selectedContainer = containers[SelectedIndex];

        // Exemplo: engine.ChangeState(new LootState(selectedContainer));
        break;
    }

    SelectedIndex = -1;
  }

  public void Render(GameEngine engine)
  {
    var exits = engine.CurrentRoom.GetAvailableExits();

    if (exits.Count == 0 && (engine.CurrentRoom.Objects == null || engine.CurrentRoom.Objects.Count == 0))
    {
      UI.ShowMessage("Não há nada para fazer ou saídas nesta sala.");
      engine.ChangeState(new MainMenuState(engine.MainMenuUI));
      return;
    }

    var result = UI.RenderScreen(engine);

    if (result == null)
    {
      SelectedIndex = -1;
    }
    else
    {
      SelectedTab = result.Value.Tab!;
      SelectedIndex = result.Value.Index;
    }
  }
}