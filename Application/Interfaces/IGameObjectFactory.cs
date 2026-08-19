using BaldursNet.Application.Dtos;
using BaldursNet.Domain.Entities;

namespace BaldursNet.Application.Interfaces;

public interface IGameObjectFactory
{
  GameObject Create(GameObjectDto dto);
}