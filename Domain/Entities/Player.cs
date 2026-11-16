using System;
using System.Collections.Generic;
using BaldursNet.Domain.Enums;

namespace BaldursNet.Domain.Entities;

public class Player : Character
{
  public int Strength { get; set; } = 12;
  public int Agility { get; set; } = 10;
  public int Intelligence { get; set; } = 10;
  public int Vitality { get; set; } = 12;

  public int CurrentXp { get; set; } = 0;
  public int XpToNextLevel => Level * 60;
  public int Gold { get; set; } = 25;

  public PlayerEquipment Equipment { get; } = new();
  public Inventory Inventory { get; }

  public List<Skill> Skills { get; } = [];

  public int TotalMaxHp => MaxHp + Equipment.TotalHpBonus + (Vitality * 4);
  public int TotalMaxMp => MaxMp + Equipment.TotalMpBonus + (Intelligence * 3);
  public int TotalAttack => BaseAttack + Equipment.TotalAttackBonus + (Strength / 2);
  public int TotalDefense => BaseDefense + Equipment.TotalDefenseBonus + (Vitality / 3);

  public Player(string name)
    : base(80, name, "Um aventureiro determinado em busca de glória e mistérios.", new Position(0, 0, 0))
  {
    Id = "player";
    Level = 1;
    MaxHp = 80;
    CurrentHp = 80;
    MaxMp = 30;
    CurrentMp = 30;
    BaseAttack = 6;
    BaseDefense = 2;

    Inventory = new Inventory(25 + (Strength * 1));

    // Habilidades iniciais
    Skills.Add(new Skill
    {
      Id = "power_strike",
      Name = "Golpe Feroz",
      Description = "Um ataque potente que desfere dano físico ampliado.",
      ManaCost = 6,
      Power = 14,
      IsHealing = false,
      Element = "Físico"
    });

    Skills.Add(new Skill
    {
      Id = "arcane_spark",
      Name = "Centelha Arcana",
      Description = "Disparo de energia pura que ignora parte da defesa do inimigo.",
      ManaCost = 8,
      Power = 16,
      IsHealing = false,
      Element = "Arcano"
    });

    Skills.Add(new Skill
    {
      Id = "minor_heal",
      Name = "Bênção Menor",
      Description = "Canalização suave de energia restauradora para fechar feridas.",
      ManaCost = 10,
      Power = 35,
      IsHealing = true,
      Element = "Sagrado"
    });
  }

  public override void TakeDamage(int damage)
  {
    CurrentHp = Math.Clamp(CurrentHp - damage, 0, TotalMaxHp);
  }

  public override void Heal(int amount)
  {
    CurrentHp = Math.Clamp(CurrentHp + amount, 0, TotalMaxHp);
  }

  public override void RestoreMp(int amount)
  {
    CurrentMp = Math.Clamp(CurrentMp + amount, 0, TotalMaxMp);
  }

  public bool GainXp(int xp, out bool leveledUp)
  {
    CurrentXp += xp;
    leveledUp = false;

    while (CurrentXp >= XpToNextLevel)
    {
      CurrentXp -= XpToNextLevel;
      Level++;
      leveledUp = true;

      // Incrementa atributos ao subir de nível
      Strength += 2;
      Agility += 1;
      Intelligence += 1;
      Vitality += 2;
      MaxHp += 15;
      MaxMp += 8;
      BaseAttack += 2;
      BaseDefense += 1;
      Inventory.MaxWeight += 2;

      // Recupera totalmente HP e MP ao subir de nível
      CurrentHp = TotalMaxHp;
      CurrentMp = TotalMaxMp;
    }

    return leveledUp;
  }

  public void GainGold(int amount)
  {
    Gold += Math.Max(0, amount);
  }

  public bool SpendGold(int amount)
  {
    if (Gold < amount) return false;
    Gold -= amount;
    return true;
  }

  public bool HasKey(string keyId)
  {
    return Inventory.Items.Any(i => i.Type == ItemType.Key && i.KeyId == keyId);
  }

  public bool Equip(Item item, out string message)
  {
    if (!item.TargetSlot.HasValue)
    {
      message = $"{item.Name} não pode ser equipado!";
      return false;
    }

    if (!Inventory.Items.Contains(item))
    {
      message = "O item não está no seu inventário.";
      return false;
    }

    Inventory.RemoveItem(item);

    if (Equipment.Equip(item, out var oldItem) && oldItem != null)
    {
      Inventory.AddItem(oldItem);
      message = $"Equipou {item.Name} e guardou {oldItem.Name} na mochila.";
    }
    else
    {
      message = $"Equipou {item.Name} no espaço {item.TargetSlot.Value}.";
    }

    return true;
  }

  public bool Unequip(EquipmentSlot slot, out string message)
  {
    var item = Equipment.GetItem(slot);
    if (item == null)
    {
      message = "Não há nada equipado neste espaço.";
      return false;
    }

    if (!Inventory.CanAddItem(item))
    {
      message = "Sua mochila está cheia demais para desequipar este item!";
      return false;
    }

    Equipment.Unequip(slot);
    Inventory.AddItem(item);
    message = $"Desequipou {item.Name} para a mochila.";
    return true;
  }

  public bool UseConsumable(Item item, out string message)
  {
    if (item.Type != ItemType.Consumable)
    {
      message = $"{item.Name} não é um item consumível.";
      return false;
    }

    var effects = new List<string>();

    if (item.HealAmount > 0)
    {
      Heal(item.HealAmount);
      effects.Add($"+{item.HealAmount} PV");
    }

    if (item.ManaAmount > 0)
    {
      RestoreMp(item.ManaAmount);
      effects.Add($"+{item.ManaAmount} PM");
    }

    Inventory.RemoveItem(item);

    // Se estiver em quickslot, remove do slot também
    if (Equipment.QuickSlot1 == item) Equipment.QuickSlot1 = null;
    if (Equipment.QuickSlot2 == item) Equipment.QuickSlot2 = null;

    message = $"Você consumiu {item.Name} ({string.Join(", ", effects)})!";
    return true;
  }
}
