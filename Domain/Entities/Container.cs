namespace BaldursNet.Domain.Entities;

public class Container : GameObject
{
  public int Capacity
  {
    get => Inventory.MaxWeight;
    set => Inventory.MaxWeight = value;
  }

  public Inventory Inventory { get; }
  public bool IsLocked { get; set; }
  public string? RequiredKeyId { get; set; }

  public Container(int cap, string name, string description, Position pos, bool isLocked = false, string? requiredKeyId = null)
    : base(name, description, pos)
  {
    Inventory = new Inventory(cap);
    IsLocked = isLocked;
    RequiredKeyId = requiredKeyId;
  }

  public bool Unlock(string keyId)
  {
    if (!IsLocked) return true;
    if (RequiredKeyId == keyId)
    {
      IsLocked = false;
      return true;
    }
    return false;
  }
}
