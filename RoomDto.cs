namespace BaldursNet;

public class RoomDto
{
  public string Id { get; set; }
  public string Name { get; set; }
  public string Description { get; set; }
  public List<string> Exits { get; set; } = [];
}