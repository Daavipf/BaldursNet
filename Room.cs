namespace BaldursNet;

public class Room(string name, string description)
{
  public string Name { get; private set; } = name;
  public string Description { get; private set; } = description;
  public List<Room> Exits { get; } = [];

  public void AddExit(Room room)
  {
    Exits.Add(room);
  }

  public Room GetExit(int index)
  {
    return Exits[index];
  }

  public List<Room> GetAvailableExits()
  {
    return Exits;
  }
}