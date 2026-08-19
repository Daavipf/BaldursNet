using BaldursNet.Application.Interfaces;
using BaldursNet.Domain.Entities;
using BaldursNet.Presentation.ConsoleUI;
using BaldursNet.State;

namespace BaldursNet.Application.Services;

public class GameEngine(IWorldLoader worldLoader) : IGameEngine
{
  private IGameState? CurrentState;
  private Stack<IGameState> StateStack = new();
  public Room CurrentRoom { get; set; } = worldLoader.GetStartingRoom("tavern");
  public IUserInterface MainMenuUI = new MainMenuUI();
  public IUserInterface ExplorationUI = new ExplorationUI();

  public void ChangeState(IGameState? newState)
  {
    CurrentState = newState;
  }

  public void Start()
  {
    ChangeState(new MainMenuState(MainMenuUI));
    RunLoop();
  }

  private void RunLoop()
  {
    while (CurrentState != null)
    {
      CurrentState.Render(this);
      CurrentState.Update(this);
    }
  }
}