namespace BaldursNet.ConsoleUI;

public class UserInterface : IUserInterface
{
  private IConsoleMenu _consoleMenu;

  public UserInterface(IConsoleMenu consoleMenu)
  {
    _consoleMenu = consoleMenu;
  }
  public GameState HandleMainMenu()
  {
    GameState state = GameState.MainMenu;

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
      state = options[selectedOption].State;
    }

    return state;
  }

  public (GameState, Room?) HandleExploration(Room currentRoom)
  {
    var exits = currentRoom.GetAvailableExits();

    if (exits.Count == 0)
    {
      _consoleMenu.ShowMessage("Não há saídas nesta sala.");
    }

    ConsoleMenuParams<Room> menuParams = new(
      items: exits,
      title: currentRoom.Name,
      description: currentRoom.Description,
      displaySelector: exit => exit.Name,
      prompt: "\n[↑/↓] Selecionar  |  [Enter] Entrar  |  [ESC] Voltar"
    );

    int option = _consoleMenu.RenderSelectibleMenu(menuParams);

    if (option == -1)
    {
      return (GameState.MainMenu, null);
    }

    Room nextRoom = exits[option];
    return (GameState.Playing, nextRoom);
  }

  public GameState HandleLoadMenu()
  {
    _consoleMenu.ShowMessage("Em Desenolvimento...");
    return GameState.MainMenu;
  }

  public GameState HandleOptionsMenu()
  {
    _consoleMenu.ShowMessage("Em Desenolvimento...");
    return GameState.MainMenu;
  }
}