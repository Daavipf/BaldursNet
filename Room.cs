namespace BaldursNet;

public class Room
{
  public string Name { get; private set; }
  public string Description { get; private set; }
  public List<Room> _exits { get; }

  public Room(string name, string description)
  {
    Name = name;
    Description = description;
    _exits = [];
  }

  public void AddExit(Room room)
  {
    _exits.Add(room);
  }

  public Room GetExit(int index)
  {
    return _exits[index];
  }

  public List<Room> GetAvailableExits()
  {
    return _exits;
  }
}