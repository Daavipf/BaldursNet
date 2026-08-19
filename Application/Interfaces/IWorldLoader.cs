using BaldursNet.Domain.Entities;

namespace BaldursNet.Application.Interfaces;

public interface IWorldLoader
{
  Room GetStartingRoom(string startingRoomId);
}