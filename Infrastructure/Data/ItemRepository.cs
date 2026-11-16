using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using BaldursNet.Application.Dtos;
using BaldursNet.Domain.Entities;
using BaldursNet.Domain.Enums;

namespace BaldursNet.Infrastructure.Data;

public class ItemRepository
{
  private readonly Dictionary<string, Item> _items = new(StringComparer.OrdinalIgnoreCase);

  public ItemRepository(string? filePath = null)
  {
    string path = ResolvePath(filePath ?? "Data/items.json", "items.json");
    if (File.Exists(path))
    {
      LoadItems(path);
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

  private void LoadItems(string path)
  {
    string json = File.ReadAllText(path);
    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    var dtos = JsonSerializer.Deserialize<List<ItemDto>>(json, options) ?? [];

    foreach (var dto in dtos)
    {
      Enum.TryParse<ItemType>(dto.Type, true, out var itemType);
      Enum.TryParse<ItemRarity>(dto.Rarity, true, out var rarity);

      EquipmentSlot? targetSlot = null;
      if (!string.IsNullOrEmpty(dto.TargetSlot) && Enum.TryParse<EquipmentSlot>(dto.TargetSlot, true, out var slot))
      {
        targetSlot = slot;
      }

      var item = new Item
      {
        Id = dto.Id,
        Name = dto.Name,
        Description = dto.Description,
        Type = itemType,
        Rarity = rarity,
        Weight = dto.Weight,
        Value = dto.Value,
        AttackBonus = dto.AttackBonus,
        DefenseBonus = dto.DefenseBonus,
        HpBonus = dto.HpBonus,
        MpBonus = dto.MpBonus,
        TargetSlot = targetSlot,
        HealAmount = dto.HealAmount,
        ManaAmount = dto.ManaAmount,
        KeyId = dto.KeyId
      };

      _items[item.Id] = item;
    }
  }

  public Item CreateItem(string id)
  {
    if (_items.TryGetValue(id, out var template))
    {
      return template.Clone();
    }

    // Item fallback
    return new Item
    {
      Id = id,
      Name = $"Item desconhecido ({id})",
      Description = "Um objeto misterioso sem detalhes registrados.",
      Weight = 1
    };
  }

  public bool TryGetItem(string id, out Item item)
  {
    if (_items.TryGetValue(id, out var template))
    {
      item = template.Clone();
      return true;
    }
    item = null!;
    return false;
  }

  public IReadOnlyDictionary<string, Item> GetAllItems() => _items;
}
