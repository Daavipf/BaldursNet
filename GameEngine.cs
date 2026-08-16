using BaldursNet.ConsoleUI;

namespace BaldursNet;

public class GameEngine
{
  private GameState _currentState;
  private Room _currentRoom;
  private IUserInterface _userInterface;

  public GameEngine(IUserInterface userInterface)
  {
    _userInterface = userInterface;
    _currentRoom = InitWorld();
  }

  public void Start()
  {
    InitWorld();
    _currentState = GameState.MainMenu;
    RunLoop();
  }

  // MÉTODO TEMPORÁRIO. LOGO SERÁ REFATORADO
  private Room InitWorld()
  {
    var tavern = new Room("Taverna do Javali", "O ar cheira a cerveja velha e fumaça.");
    var street = new Room("Rua Principal", "Uma rua de paralelepípedos escura.");
    var alley = new Room("Beco Escuro", "Você mal consegue ver um palmo à frente.");

    tavern.AddExit(street);

    street.AddExit(tavern);
    street.AddExit(alley);

    alley.AddExit(street);

    return tavern;
  }

  private void RunLoop()
  {
    while (_currentState != GameState.Exit)
    {
      switch (_currentState)
      {
        case GameState.MainMenu:
          _currentState = _userInterface.HandleMainMenu();
          break;
        case GameState.Playing:
          var (nextState, nextRoom) = _userInterface.HandleExploration(_currentRoom);
          _currentState = nextState;
          if (nextRoom != null)
          {
            _currentRoom = nextRoom;
          }
          break;
        case GameState.OptionsMenu:
          _currentState = _userInterface.HandleOptionsMenu();
          break;
        case GameState.LoadMenu:
          _currentState = _userInterface.HandleLoadMenu();
          break;
      }
    }
  }
}