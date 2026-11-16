using BaldursNet.Domain.Enums;

namespace BaldursNet.Domain.Entities;

public class PlayerEquipment
{
  public Item? Weapon { get; set; }
  public Item? Armor { get; set; }
  public Item? Shield { get; set; }
  public Item? Helmet { get; set; }
  public Item? Accessory { get; set; }
  public Item? QuickSlot1 { get; set; }
  public Item? QuickSlot2 { get; set; }

  public int TotalAttackBonus => Weapon?.AttackBonus ?? 0;
  public int TotalDefenseBonus => (Armor?.DefenseBonus ?? 0) + (Shield?.DefenseBonus ?? 0) + (Helmet?.DefenseBonus ?? 0);
  public int TotalHpBonus => (Accessory?.HpBonus ?? 0) + (Armor?.HpBonus ?? 0);
  public int TotalMpBonus => (Weapon?.MpBonus ?? 0) + (Accessory?.MpBonus ?? 0);

  public Item? GetItem(EquipmentSlot slot) => slot switch
  {
    EquipmentSlot.Weapon => Weapon,
    EquipmentSlot.Armor => Armor,
    EquipmentSlot.Shield => Shield,
    EquipmentSlot.Helmet => Helmet,
    EquipmentSlot.Accessory => Accessory,
    EquipmentSlot.QuickSlot1 => QuickSlot1,
    EquipmentSlot.QuickSlot2 => QuickSlot2,
    _ => null
  };

  public bool Equip(Item item, out Item? previouslyEquipped)
  {
    previouslyEquipped = null;
    var slot = item.TargetSlot;

    if (!slot.HasValue)
      return false;

    previouslyEquipped = GetItem(slot.Value);

    switch (slot.Value)
    {
      case EquipmentSlot.Weapon:
        Weapon = item;
        break;
      case EquipmentSlot.Armor:
        Armor = item;
        break;
      case EquipmentSlot.Shield:
        Shield = item;
        break;
      case EquipmentSlot.Helmet:
        Helmet = item;
        break;
      case EquipmentSlot.Accessory:
        Accessory = item;
        break;
      case EquipmentSlot.QuickSlot1:
        QuickSlot1 = item;
        break;
      case EquipmentSlot.QuickSlot2:
        QuickSlot2 = item;
        break;
      default:
        return false;
    }

    return true;
  }

  public Item? Unequip(EquipmentSlot slot)
  {
    Item? removed = GetItem(slot);
    switch (slot)
    {
      case EquipmentSlot.Weapon:
        Weapon = null;
        break;
      case EquipmentSlot.Armor:
        Armor = null;
        break;
      case EquipmentSlot.Shield:
        Shield = null;
        break;
      case EquipmentSlot.Helmet:
        Helmet = null;
        break;
      case EquipmentSlot.Accessory:
        Accessory = null;
        break;
      case EquipmentSlot.QuickSlot1:
        QuickSlot1 = null;
        break;
      case EquipmentSlot.QuickSlot2:
        QuickSlot2 = null;
        break;
    }
    return removed;
  }
}
