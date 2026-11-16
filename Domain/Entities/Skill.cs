namespace BaldursNet.Domain.Entities;

public class Skill
{
  public string Id { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public int ManaCost { get; set; }
  public int Power { get; set; }
  public bool IsHealing { get; set; }
  public string Element { get; set; } = "Físico";

  public Skill Clone()
  {
    return new Skill
    {
      Id = Id,
      Name = Name,
      Description = Description,
      ManaCost = ManaCost,
      Power = Power,
      IsHealing = IsHealing,
      Element = Element
    };
  }
}
