using BaldursNet.Application.Dtos;
using BaldursNet.Domain.Entities;

namespace BaldursNet.Application.Interfaces;

public interface IRoomFactory
{
  Room CreateRoom(RoomDto dto);
}