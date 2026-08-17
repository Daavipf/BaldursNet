using BaldursNet.State.Enums;

namespace BaldursNet.Presentation.ConsoleUI;

public class RoomMenuParams
{
  public RoomDto Room { get; set; }
  public ExplorationTab ActiveTab { get; set; }
  public SelectionMenuParams<Room> TabContentMenu { get; set; }
}