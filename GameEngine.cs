using BaldursNet.ConsoleUI;

namespace BaldursNet;

public class GameEngine
{
  private GameState _currentState;
  private Room? _currentRoom;
  private IConsoleMenu _consoleMenu;

  public GameEngine(IConsoleMenu consoleMenu)
  {
    _consoleMenu = consoleMenu;
  }

  public void Start()
  {
    InitWorld();
    _currentState = GameState.MainMenu;
    RunLoop();
  }

  private void InitWorld()
  {
    var tavern = new Room("Taverna do Javali", "O ar cheira a cerveja velha e fumaça.");
    var street = new Room("Rua Principal", "Uma rua de paralelepípedos escura.");
    var alley = new Room("Beco Escuro", "Você mal consegue ver um palmo à frente.");

    tavern.AddExit(street);

    street.AddExit(tavern);
    street.AddExit(alley);

    alley.AddExit(street);

    _currentRoom = tavern;
  }

  private void RunLoop()
  {
    while (_currentState != GameState.Exit)
    {
      switch (_currentState)
      {
        case GameState.MainMenu:
          HandleMenu();
          break;
        case GameState.Playing:
          HandleExploration();
          break;
        case GameState.OptionsMenu:
          _consoleMenu.ShowMessage("Em Desenolvimento...");
          break;
        case GameState.LoadMenu:
          _consoleMenu.ShowMessage("Em Desenolvimento...");
          break;
      }
    }
  }

  private void HandleMenu()
  {
    List<(string Label, GameState State)> options = [
      ("Iniciar Jogo", GameState.Playing),
      ("Carregar Save", GameState.LoadMenu),
      ("Opções", GameState.OptionsMenu),
      ("Sair", GameState.Exit)
    ];

    ConsoleMenuParams<(string Label, GameState State)> menuParams = new(
      items: options,
      title: "BALDUR'S NET 10.0 ",
      canCancel: false,
      displaySelector: opt => opt.Label);

    int selectedOption = _consoleMenu.RenderSelectibleMenu(menuParams);

    if (selectedOption >= 0 && selectedOption < options.Count)
    {
      _currentState = options[selectedOption].State;
    }
  }

  private void HandleExploration()
  {
    var exits = _currentRoom.GetAvailableExits();

    if (exits.Count == 0)
    {
      _consoleMenu.ShowMessage("Não há saídas nesta sala.");
    }

    ConsoleMenuParams<Room> menuParams = new(
      items: exits,
      title: _currentRoom.Name,
      description: _currentRoom.Description,
      displaySelector: exit => exit.Name,
      prompt: "\n[↑/↓] Selecionar  |  [Enter] Entrar  |  [ESC] Voltar"
    );

    int option = _consoleMenu.RenderSelectibleMenu(menuParams);

    if (option == -1)
    {
      _currentState = GameState.MainMenu;
      return;
    }

    _currentRoom = exits[option];
  }
}