namespace BaldursNet;

public interface IWorldLoader
{
  Room GetStartingRoom(string startingRoomId);
}