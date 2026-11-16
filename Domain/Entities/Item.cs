using BaldursNet.Domain.Enums;

namespace BaldursNet.Domain.Entities;

public class Item
{
  public string Id { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public ItemType Type { get; set; } = ItemType.Misc;
  public ItemRarity Rarity { get; set; } = ItemRarity.Common;
  public int Weight { get; set; } = 1;
  public int Value { get; set; } = 0;

  // Equipamento
  public int AttackBonus { get; set; }
  public int DefenseBonus { get; set; }
  public int HpBonus { get; set; }
  public int MpBonus { get; set; }
  public EquipmentSlot? TargetSlot { get; set; }

  // Consumível
  public int HealAmount { get; set; }
  public int ManaAmount { get; set; }

  // Chave
  public string? KeyId { get; set; }

  public Item Clone()
  {
    return new Item
    {
      Id = Id,
      Name = Name,
      Description = Description,
      Type = Type,
      Rarity = Rarity,
      Weight = Weight,
      Value = Value,
      AttackBonus = AttackBonus,
      DefenseBonus = DefenseBonus,
      HpBonus = HpBonus,
      MpBonus = MpBonus,
      TargetSlot = TargetSlot,
      HealAmount = HealAmount,
      ManaAmount = ManaAmount,
      KeyId = KeyId
    };
  }

  public string GetStatSummary()
  {
    var stats = new List<string>();
    if (AttackBonus != 0) stats.Add($"+{AttackBonus} ATQ");
    if (DefenseBonus != 0) stats.Add($"+{DefenseBonus} DEF");
    if (HpBonus != 0) stats.Add($"+{HpBonus} PV");
    if (MpBonus != 0) stats.Add($"+{MpBonus} PM");
    if (HealAmount > 0) stats.Add($"Cura {HealAmount} PV");
    if (ManaAmount > 0) stats.Add($"Restaura {ManaAmount} PM");
    stats.Add($"{Weight} kg");

    return stats.Count > 0 ? $"({string.Join(", ", stats)})" : string.Empty;
  }
}
