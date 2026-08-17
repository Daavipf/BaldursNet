using BaldursNet.Application.Interfaces;
using BaldursNet.State;

namespace BaldursNet.Application.Services;

public class GameEngine(IUserInterface ui, IWorldLoader worldLoader)
{
  private IGameState? CurrentState;
  private Stack<IGameState> StateStack = new();
  public Room CurrentRoom { get; set; } = worldLoader.GetStartingRoom("tavern");
  public IUserInterface UI { get; } = ui;

  public void ChangeState(IGameState? newState)
  {
    CurrentState = newState;
  }

  public void Start()
  {
    ChangeState(new MainMenuState());
    RunLoop();
  }

  private void RunLoop()
  {
    while (CurrentState != null)
    {
      CurrentState.Update(this);
    }
  }
}