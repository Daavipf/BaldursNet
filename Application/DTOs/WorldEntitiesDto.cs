namespace BaldursNet.Application.Dtos;

public class ContainerDto
{
  public string Id { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public int Capacity { get; set; } = 20;
  public bool IsLocked { get; set; }
  public string? RequiredKeyId { get; set; }
  public List<string> Items { get; set; } = [];
}

public class NpcDto
{
  public string Id { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public List<string> Dialogue { get; set; } = [];
  public bool IsMerchant { get; set; }
  public List<string> MerchantStock { get; set; } = [];
}

public class RoomExitDto
{
  public string TargetRoomId { get; set; } = string.Empty;
  public string DisplayName { get; set; } = string.Empty;
  public bool IsLocked { get; set; }
  public string? RequiredKeyId { get; set; }
  public string? LockedDescription { get; set; }
}

public class EnemyInstanceDto
{
  public string EnemyId { get; set; } = string.Empty;
}
