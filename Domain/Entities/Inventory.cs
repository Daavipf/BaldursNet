using System;
using System.Collections.Generic;
using System.Linq;

namespace BaldursNet.Domain.Entities;

public class Inventory
{
  public int MaxWeight { get; set; }
  public List<Item> Items { get; } = [];

  public int CurrentWeight => Items.Sum(i => i.Weight);
  public int RemainingCapacity => Math.Max(0, MaxWeight - CurrentWeight);
  public bool IsFull => CurrentWeight >= MaxWeight;

  public Inventory(int maxWeight)
  {
    MaxWeight = maxWeight;
  }

  public bool CanAddItem(Item item)
  {
    return CurrentWeight + item.Weight <= MaxWeight;
  }

  public bool AddItem(Item item)
  {
    if (!CanAddItem(item))
      return false;

    Items.Add(item);
    return true;
  }

  public bool RemoveItem(Item item)
  {
    return Items.Remove(item);
  }

  public bool TransferTo(Inventory target, Item item, out string message)
  {
    if (!Items.Contains(item))
    {
      message = "O item não foi encontrado no inventário de origem.";
      return false;
    }

    if (!target.CanAddItem(item))
    {
      message = $"Capacidade insuficiente no destino! ({target.CurrentWeight}/{target.MaxWeight} kg - Requer {item.Weight} kg)";
      return false;
    }

    Items.Remove(item);
    target.AddItem(item);
    message = $"{item.Name} transferido com sucesso!";
    return true;
  }

  public void Clear()
  {
    Items.Clear();
  }
}
