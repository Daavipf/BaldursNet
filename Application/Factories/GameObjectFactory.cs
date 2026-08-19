using BaldursNet.Application.Dtos;
using BaldursNet.Application.Interfaces;
using BaldursNet.Domain.Entities;

namespace BaldursNet.Application.Factories;

public class GameObjectFactory : IGameObjectFactory
{
  public GameObject Create(GameObjectDto dto)
  {
    var position = new Position(dto.Position.X, dto.Position.Y, dto.Position.Z);

    return dto.Type switch
    {
      "Character" => new Character(dto.Life, dto.Name, dto.Description, position),
      "Container" => new Container(dto.Capacity, dto.Name, dto.Description, position),
      _ => throw new ArgumentException($"Tipo de GameObject desconhecido no JSON: {dto.Type}")
    };
  }
}