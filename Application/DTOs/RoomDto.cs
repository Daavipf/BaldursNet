using System.Text.Json.Serialization;

namespace BaldursNet.Application.Dtos;

public class RoomDto
{
  public string Id { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;

  // Saídas avançadas
  public List<RoomExitDto> Exits { get; set; } = [];

  // Entidades declaradas de forma limpa e modular
  public List<EnemyInstanceDto> Enemies { get; set; } = [];
  public List<NpcDto> Npcs { get; set; } = [];
  public List<ContainerDto> Containers { get; set; } = [];
  public List<string> Items { get; set; } = [];

  // Retrocompatibilidade opcional com o formato original
  public List<GameObjectDto>? Objects { get; set; }
}
