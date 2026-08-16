namespace BaldursNet.ConsoleUI;

public interface IUserInterface
{
  GameState HandleMainMenu();
  (GameState NextState, Room? NextRoom) HandleExploration(Room currentRoom);
  GameState HandleOptionsMenu();
  GameState HandleLoadMenu();
}