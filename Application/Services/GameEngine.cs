using BaldursNet.ConsoleUI;

namespace BaldursNet;

public class GameEngine
{
  private GameState _currentState;
  private Room _currentRoom;
  private readonly IUserInterface UserInterface;
  private readonly IWorldLoader WorldLoader;

  public GameEngine(IUserInterface userInterface, IWorldLoader worldLoader)
  {
    UserInterface = userInterface;
    WorldLoader = worldLoader;
    _currentRoom = WorldLoader.GetStartingRoom("tavern");
  }

  public void Start()
  {
    _currentState = GameState.MainMenu;
    RunLoop();
  }

  private void RunLoop()
  {
    while (_currentState != GameState.Exit)
    {
      switch (_currentState)
      {
        case GameState.MainMenu:
          _currentState = UserInterface.HandleMainMenu();
          break;
        case GameState.Playing:
          var (nextState, nextRoom) = UserInterface.HandleExploration(_currentRoom);
          _currentState = nextState;
          if (nextRoom != null)
          {
            _currentRoom = nextRoom;
          }
          break;
        case GameState.OptionsMenu:
          _currentState = UserInterface.HandleOptionsMenu();
          break;
        case GameState.LoadMenu:
          _currentState = UserInterface.HandleLoadMenu();
          break;
      }
    }
  }
}