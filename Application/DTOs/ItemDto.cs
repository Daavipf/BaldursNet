namespace BaldursNet.Application.Dtos;

public class ItemDto
{
  public string Id { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public string Type { get; set; } = "Misc";
  public string Rarity { get; set; } = "Common";
  public int Weight { get; set; } = 1;
  public int Value { get; set; } = 0;
  public int AttackBonus { get; set; }
  public int DefenseBonus { get; set; }
  public int HpBonus { get; set; }
  public int MpBonus { get; set; }
  public string? TargetSlot { get; set; }
  public int HealAmount { get; set; }
  public int ManaAmount { get; set; }
  public string? KeyId { get; set; }
}
