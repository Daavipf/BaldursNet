using System.Collections.Generic;
using System.Linq;

namespace BaldursNet.Domain.Entities;

public class Room
{
  public string Id { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public List<RoomExit> Exits { get; } = [];
  public List<GameObject> Objects { get; } = [];

  public Room(string id, string name, string description)
  {
    Id = id;
    Name = name;
    Description = description;
  }

  public Room(string name, string description) : this(string.Empty, name, description) { }

  public void AddExit(RoomExit exit)
  {
    Exits.Add(exit);
  }

  public void AddExit(Room targetRoom, string? displayName = null)
  {
    Exits.Add(new RoomExit
    {
      TargetRoomId = targetRoom.Id,
      DisplayName = displayName ?? targetRoom.Name,
      TargetRoom = targetRoom
    });
  }

  public List<RoomExit> GetAvailableExits() => Exits;

  public List<T> GetGameObjects<T>() where T : GameObject
  {
    return Objects.OfType<T>().Where(o => o.IsActive).ToList();
  }

  public List<Enemy> GetAliveEnemies()
  {
    return GetGameObjects<Enemy>().Where(e => e.IsAlive).ToList();
  }

  public List<Npc> GetNpcs() => GetGameObjects<Npc>();

  public List<Container> GetContainers() => GetGameObjects<Container>();

  public List<WorldItem> GetWorldItems() => GetGameObjects<WorldItem>();

  public void AddObject(GameObject obj)
  {
    Objects.Add(obj);
  }

  public void RemoveObject(GameObject obj)
  {
    Objects.Remove(obj);
  }
}
