using BaldursNet.Domain.Entities;

namespace BaldursNet.Application.Interfaces;

public interface IGameEngine
{
  Room CurrentRoom { get; set; }
  void Start();
  void ChangeState(IGameState? newState);
}