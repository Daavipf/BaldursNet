namespace BaldursNet.Application.Dtos;

public class SkillDto
{
  public string Id { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public int ManaCost { get; set; }
  public int Power { get; set; }
  public bool IsHealing { get; set; }
  public string Element { get; set; } = "Físico";
}

public class LootDropDto
{
  public string ItemId { get; set; } = string.Empty;
  public double DropChance { get; set; } = 1.0;
}

public class EnemyDto
{
  public string Id { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public string EnemyType { get; set; } = "Monstro";
  public int Level { get; set; } = 1;
  public int MaxHp { get; set; } = 30;
  public int MaxMp { get; set; } = 10;
  public int BaseAttack { get; set; } = 5;
  public int BaseDefense { get; set; } = 1;
  public int XpReward { get; set; } = 20;
  public int GoldReward { get; set; } = 10;
  public List<LootDropDto> LootDrops { get; set; } = [];
  public List<SkillDto> Skills { get; set; } = [];
}
