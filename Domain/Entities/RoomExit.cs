namespace BaldursNet.Domain.Entities;

public class RoomExit
{
  public string TargetRoomId { get; set; } = string.Empty;
  public string DisplayName { get; set; } = string.Empty;
  public Room? TargetRoom { get; set; }
  public bool IsLocked { get; set; }
  public string? RequiredKeyId { get; set; }
  public string? LockedDescription { get; set; }

  public bool Unlock(string keyId)
  {
    if (!IsLocked) return true;
    if (RequiredKeyId == keyId)
    {
      IsLocked = false;
      return true;
    }
    return false;
  }
}
