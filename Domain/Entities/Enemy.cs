using System;
using System.Collections.Generic;

namespace BaldursNet.Domain.Entities;

public class LootDropEntry
{
  public Item Item { get; set; } = new();
  public double DropChance { get; set; } = 1.0;
}

public class Enemy : Character
{
  public string EnemyType { get; set; } = "Monstro";
  public int XpReward { get; set; } = 20;
  public int GoldReward { get; set; } = 10;
  public List<LootDropEntry> LootDrops { get; } = [];
  public List<Skill> Skills { get; } = [];

  public Enemy() { }

  public Enemy(string id, string name, string description, int hp, int mp, int attack, int defense, int level)
    : base(hp, name, description, new Position(0, 0, 0))
  {
    Id = id;
    MaxMp = mp;
    CurrentMp = mp;
    BaseAttack = attack;
    BaseDefense = defense;
    Level = level;
  }

  public List<Item> GenerateLoot()
  {
    var loot = new List<Item>();
    var random = new Random();

    foreach (var drop in LootDrops)
    {
      if (random.NextDouble() <= drop.DropChance)
      {
        loot.Add(drop.Item.Clone());
      }
    }

    return loot;
  }

  public Enemy Clone()
  {
    var clone = new Enemy(Id, Name, Description, MaxHp, MaxMp, BaseAttack, BaseDefense, Level)
    {
      EnemyType = EnemyType,
      XpReward = XpReward,
      GoldReward = GoldReward
    };

    foreach (var drop in LootDrops)
    {
      clone.LootDrops.Add(new LootDropEntry
      {
        Item = drop.Item.Clone(),
        DropChance = drop.DropChance
      });
    }

    foreach (var skill in Skills)
    {
      clone.Skills.Add(skill.Clone());
    }

    return clone;
  }
}
