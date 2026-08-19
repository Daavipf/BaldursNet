namespace BaldursNet.Domain.Entities;

public class GameObject(string name, string description, Position pos)
{
  public string Name { get; set; } = name;
  public string Description { get; set; } = description;
  public Position Position { get; set; } = pos;
  public bool IsActive { get; set; } = true;
}