using BaldursNet.Application.Dtos;
using BaldursNet.Application.Interfaces;
using BaldursNet.Domain.Entities;

namespace BaldursNet.Application.Factories;

public class RoomFactory(IGameObjectFactory gameObjectFactory) : IRoomFactory
{
  private readonly IGameObjectFactory GameObjectFactory = gameObjectFactory;
  public Room CreateRoom(RoomDto dto)
  {
    var room = new Room(dto.Name, dto.Description);

    if (dto.Objects != null)
    {
      foreach (var objDto in dto.Objects)
      {
        var gameObject = GameObjectFactory.Create(objDto);
        room.AddObject(gameObject);
      }
    }

    return room;
  }
}