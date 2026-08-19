namespace BaldursNet.Application.Dtos;

public class GameObjectDto
{
  public string Type { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public PositionDto Position { get; set; } = new PositionDto();

  // Type = Character
  public int Life { get; set; }

  // Type = Container
  public int Capacity { get; set; }
}