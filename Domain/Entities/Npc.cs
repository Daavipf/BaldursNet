using System.Collections.Generic;

namespace BaldursNet.Domain.Entities;

public class Npc : Character
{
  public List<string> DialogueLines { get; set; } = [];
  public bool IsMerchant { get; set; }
  public Inventory? MerchantStock { get; set; }

  public Npc(string id, string name, string description, Position pos)
    : base(100, name, description, pos)
  {
    Id = id;
  }
}
