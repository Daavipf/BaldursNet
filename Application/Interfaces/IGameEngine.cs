using BaldursNet.Domain.Entities;

namespace BaldursNet.Application.Interfaces;

public interface IGameEngine
{
  Room CurrentRoom { get; set; }
  Player Player { get; set; }
  void Start();
  void ChangeState(IGameState? newState);
  void PushState(IGameState newState);
  void PopState();
  void AddMessage(string message);
}
