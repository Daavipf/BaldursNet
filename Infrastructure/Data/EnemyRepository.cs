using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using BaldursNet.Application.Dtos;
using BaldursNet.Domain.Entities;

namespace BaldursNet.Infrastructure.Data;

public class EnemyRepository
{
  private readonly Dictionary<string, Enemy> _enemyTemplates = new(StringComparer.OrdinalIgnoreCase);
  private readonly ItemRepository _itemRepository;

  public EnemyRepository(ItemRepository itemRepository, string? filePath = null)
  {
    _itemRepository = itemRepository;
    string path = ResolvePath(filePath ?? "Data/enemies.json", "enemies.json");
    if (File.Exists(path))
    {
      LoadEnemies(path);
    }
  }

  private static string ResolvePath(string primary, string fallback)
  {
    if (File.Exists(primary)) return primary;
    if (File.Exists(fallback)) return fallback;
    string appDir = AppDomain.CurrentDomain.BaseDirectory;
    string inAppDir = Path.Combine(appDir, primary);
    if (File.Exists(inAppDir)) return inAppDir;
    string inAppFallback = Path.Combine(appDir, fallback);
    if (File.Exists(inAppFallback)) return inAppFallback;
    return primary;
  }

  private void LoadEnemies(string path)
  {
    string json = File.ReadAllText(path);
    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    var dtos = JsonSerializer.Deserialize<List<EnemyDto>>(json, options) ?? [];

    foreach (var dto in dtos)
    {
      var enemy = new Enemy(dto.Id, dto.Name, dto.Description, dto.MaxHp, dto.MaxMp, dto.BaseAttack, dto.BaseDefense, dto.Level)
      {
        EnemyType = dto.EnemyType,
        XpReward = dto.XpReward,
        GoldReward = dto.GoldReward
      };

      foreach (var dropDto in dto.LootDrops)
      {
        var item = _itemRepository.CreateItem(dropDto.ItemId);
        enemy.LootDrops.Add(new LootDropEntry
        {
          Item = item,
          DropChance = dropDto.DropChance
        });
      }

      foreach (var skillDto in dto.Skills)
      {
        enemy.Skills.Add(new Skill
        {
          Id = skillDto.Id,
          Name = skillDto.Name,
          Description = skillDto.Description,
          ManaCost = skillDto.ManaCost,
          Power = skillDto.Power,
          IsHealing = skillDto.IsHealing,
          Element = skillDto.Element
        });
      }

      _enemyTemplates[enemy.Id] = enemy;
    }
  }

  public Enemy CreateEnemy(string id)
  {
    if (_enemyTemplates.TryGetValue(id, out var template))
    {
      return template.Clone();
    }

    return new Enemy(id, $"Inimigo ({id})", "Uma criatura hostil desconhecida.", 30, 10, 5, 1, 1);
  }

  public bool TryGetEnemy(string id, out Enemy enemy)
  {
    if (_enemyTemplates.TryGetValue(id, out var template))
    {
      enemy = template.Clone();
      return true;
    }
    enemy = null!;
    return false;
  }
}
