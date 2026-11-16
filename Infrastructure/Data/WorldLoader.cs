using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using BaldursNet.Application.Dtos;
using BaldursNet.Application.Interfaces;
using BaldursNet.Domain.Entities;
using BaldursNet.Domain.Enums;

namespace BaldursNet.Infrastructure.Data;

public class WorldLoader : IWorldLoader
{
  private readonly string _filePath;
  private readonly ItemRepository _itemRepository;
  private readonly EnemyRepository _enemyRepository;
  private Dictionary<string, Room>? _cachedRooms;

  public WorldLoader(string? filePath = null, ItemRepository? itemRepo = null, EnemyRepository? enemyRepo = null)
  {
    _itemRepository = itemRepo ?? new ItemRepository();
    _enemyRepository = enemyRepo ?? new EnemyRepository(_itemRepository);
    _filePath = ResolvePath(filePath ?? "Data/world.json", "world.json");
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

  public Room GetStartingRoom(string startingRoomId)
  {
    var domainRooms = LoadWorld();

    if (!domainRooms.TryGetValue(startingRoomId, out var startRoom))
    {
      if (domainRooms.Count > 0)
        return domainRooms.Values.First();

      throw new KeyNotFoundException($"Sala inicial '{startingRoomId}' não encontrada no mapa.");
    }

    return startRoom;
  }

  public Dictionary<string, Room> LoadWorld()
  {
    if (_cachedRooms != null)
      return _cachedRooms;

    if (!File.Exists(_filePath))
      throw new FileNotFoundException($"O arquivo de mapa não foi encontrado: {_filePath}");

    string json = File.ReadAllText(_filePath);
    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    var roomDtos = JsonSerializer.Deserialize<List<RoomDto>>(json, options) ?? [];

    if (roomDtos.Count == 0)
      throw new InvalidDataException("O arquivo de mapa está vazio ou inválido.");

    var domainRooms = new Dictionary<string, Room>(StringComparer.OrdinalIgnoreCase);

    // 1. Instancia as salas
    foreach (var dto in roomDtos)
    {
      var room = new Room(dto.Id, dto.Name, dto.Description);

      // Containers
      foreach (var containerDto in dto.Containers)
      {
        var container = new Container(
          containerDto.Capacity,
          containerDto.Name,
          containerDto.Description,
          new Position(0, 0, 0),
          containerDto.IsLocked,
          containerDto.RequiredKeyId
        )
        {
          Id = containerDto.Id
        };

        foreach (var itemId in containerDto.Items)
        {
          var item = _itemRepository.CreateItem(itemId);
          container.Inventory.AddItem(item);
        }

        room.AddObject(container);
      }

      // NPCs
      foreach (var npcDto in dto.Npcs)
      {
        var npc = new Npc(npcDto.Id, npcDto.Name, npcDto.Description, new Position(0, 0, 0))
        {
          DialogueLines = npcDto.Dialogue,
          IsMerchant = npcDto.IsMerchant
        };

        if (npc.IsMerchant)
        {
          npc.MerchantStock = new Inventory(500);
          foreach (var itemId in npcDto.MerchantStock)
          {
            var item = _itemRepository.CreateItem(itemId);
            npc.MerchantStock.AddItem(item);
          }
        }

        room.AddObject(npc);
      }

      // Inimigos
      foreach (var enemyInst in dto.Enemies)
      {
        var enemy = _enemyRepository.CreateEnemy(enemyInst.EnemyId);
        room.AddObject(enemy);
      }

      // Itens soltos no chão
      foreach (var itemId in dto.Items)
      {
        var item = _itemRepository.CreateItem(itemId);
        room.AddObject(new WorldItem(item));
      }

      // Retrocompatibilidade se tiver Objects legados
      if (dto.Objects != null)
      {
        foreach (var obj in dto.Objects)
        {
          var pos = new Position(obj.Position.X, obj.Position.Y, obj.Position.Z);
          if (obj.Type == "Character")
          {
            room.AddObject(new Character(obj.Life, obj.Name, obj.Description, pos));
          }
          else if (obj.Type == "Container")
          {
            room.AddObject(new Container(obj.Capacity, obj.Name, obj.Description, pos));
          }
        }
      }

      domainRooms[room.Id] = room;
    }

    // 2. Conecta as saídas (nós do grafo)
    foreach (var dto in roomDtos)
    {
      if (!domainRooms.TryGetValue(dto.Id, out var currentRoom)) continue;

      foreach (var exitDto in dto.Exits)
      {
        if (domainRooms.TryGetValue(exitDto.TargetRoomId, out var targetRoom))
        {
          currentRoom.AddExit(new RoomExit
          {
            TargetRoomId = exitDto.TargetRoomId,
            DisplayName = string.IsNullOrEmpty(exitDto.DisplayName) ? targetRoom.Name : exitDto.DisplayName,
            TargetRoom = targetRoom,
            IsLocked = exitDto.IsLocked,
            RequiredKeyId = exitDto.RequiredKeyId,
            LockedDescription = exitDto.LockedDescription
          });
        }
      }
    }

    _cachedRooms = domainRooms;
    return domainRooms;
  }
}
