namespace BaldursNet.Domain.Entities;

public abstract class GameObject
{
  public string Id { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public Position Position { get; set; } = new Position(0, 0, 0);
  public bool IsActive { get; set; } = true;

  protected GameObject() { }

  protected GameObject(string name, string description, Position pos)
  {
    Name = name;
    Description = description;
    Position = pos;
  }
}
