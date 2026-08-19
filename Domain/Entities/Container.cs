namespace BaldursNet.Domain.Entities;

public class Container : GameObject
{
  public int Capacity { get; set; }

  public Container(int cap, string name, string description, Position pos)
    : base(name, description, pos)
  {
    Capacity = cap;
  }


}