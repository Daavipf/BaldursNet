namespace BaldursNet.Domain.Entities;

public class Character : GameObject
{
  public int Life { get; set; }

  public Character(int life, string name, string description, Position pos)
    : base(name, description, pos)
  {
    Life = life;
  }


}