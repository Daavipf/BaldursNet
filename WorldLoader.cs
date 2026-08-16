using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace BaldursNet;

public class JsonWorldLoader(string filePath) : IWorldLoader
{
  private readonly string FilePath = filePath;

  public Room GetStartingRoom(string startingRoomId)
  {
    Dictionary<string, Room> domainRooms = LoadWorld(FilePath);

    if (!domainRooms.TryGetValue(startingRoomId, out var startRoom))
      throw new KeyNotFoundException($"Sala inicial '{startingRoomId}' não encontrada no mapa.");

    return startRoom;
  }

  private Dictionary<string, Room> LoadWorld(string filePath)
  {
    List<RoomDto> roomDtos = ReadWorldFile(filePath);

    Dictionary<string, Room> domainRooms = MapDtoToDomainRooms(roomDtos);

    ConnectRoomsNodes(roomDtos, domainRooms);

    return domainRooms;
  }

  private List<RoomDto> ReadWorldFile(string filePath)
  {
    if (!File.Exists(filePath))
      throw new FileNotFoundException($"O arquivo de mapa não foi encontrado: {filePath}");

    string json = File.ReadAllText(filePath);

    var roomDtos = JsonSerializer.Deserialize<List<RoomDto>>(json);

    if (roomDtos == null || roomDtos.Count == 0)
      throw new InvalidDataException("O arquivo de mapa está vazio ou inválido.");

    return roomDtos;
  }

  private Dictionary<string, Room> MapDtoToDomainRooms(List<RoomDto> roomDtos)
  {
    var domainRooms = new Dictionary<string, Room>();
    foreach (var dto in roomDtos)
    {
      domainRooms[dto.Id] = new Room(dto.Name, dto.Description);
    }

    return domainRooms;
  }

  private void ConnectRoomsNodes(List<RoomDto> roomDtos, Dictionary<string, Room> domainRooms)
  {
    foreach (var dto in roomDtos)
    {
      var currentRoom = domainRooms[dto.Id];

      foreach (var exit in dto.Exits)
      {

        if (domainRooms.TryGetValue(exit, out var targetRoom))
        {
          currentRoom.AddExit(targetRoom);
        }
      }
    }
  }
}