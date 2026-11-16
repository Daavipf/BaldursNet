namespace BaldursNet.Domain.Entities;

public class WorldItem : GameObject
{
  public Item Item { get; set; }

  public WorldItem(Item item, Position? pos = null)
    : base(item.Name, item.Description, pos ?? new Position(0, 0, 0))
  {
    Id = $"world_item_{item.Id}_{Guid.NewGuid().ToString()[..6]}";
    Item = item;
  }
}
