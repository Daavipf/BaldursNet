namespace BaldursNet.Application.Dtos;

public class RoomDto
{
  public string Id { get; set; }
  public string Name { get; set; }
  public string Description { get; set; }
  public List<string> Exits { get; set; } = [];
  public List<GameObjectDto> Objects { get; set; } = [];
}