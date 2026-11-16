using System;
using System.Collections.Generic;
using BaldursNet.Application.Interfaces;
using BaldursNet.Domain.Entities;
using BaldursNet.Domain.Enums;
using BaldursNet.Presentation.ConsoleUI;
using BaldursNet.State;

namespace BaldursNet.Application.Services;

public class GameEngine : IGameEngine
{
  private readonly IWorldLoader _worldLoader;
  private readonly Stack<IGameState> _stateStack = new();
  private readonly List<string> _messageLog = [];

  public Room CurrentRoom { get; set; }
  public Player Player { get; set; }
  public IReadOnlyList<string> MessageLog => _messageLog;

  public IUserInterface MainMenuUI { get; set; } = new MainMenuUI();
  public IUserInterface ExplorationUI { get; set; } = new ExplorationUI();

  public IGameState? CurrentState => _stateStack.Count > 0 ? _stateStack.Peek() : null;

  public GameEngine(IWorldLoader worldLoader)
  {
    _worldLoader = worldLoader;
    CurrentRoom = _worldLoader.GetStartingRoom("tavern");
    Player = CreateDefaultPlayer();
  }

  private static Player CreateDefaultPlayer()
  {
    var player = new Player("Aventureiro");
    // Item inicial na mão
    var sword = new Item
    {
      Id = "starter_sword",
      Name = "Espada Curta Desgastada",
      Description = "Uma espada antiga que te acompanhou até aqui.",
      Type = ItemType.Weapon,
      Rarity = ItemRarity.Common,
      Weight = 2,
      Value = 10,
      AttackBonus = 5,
      TargetSlot = EquipmentSlot.Weapon
    };
    player.Equipment.Weapon = sword;

    // Consumível inicial no bolso
    var potion = new Item
    {
      Id = "starter_potion",
      Name = "Poção de Vida",
      Description = "Restaura 30 PV.",
      Type = ItemType.Consumable,
      Rarity = ItemRarity.Common,
      Weight = 1,
      Value = 15,
      HealAmount = 30,
      TargetSlot = EquipmentSlot.QuickSlot1
    };
    player.Inventory.AddItem(potion);
    player.Equipment.QuickSlot1 = potion;

    return player;
  }

  public void RestartGame()
  {
    Player = CreateDefaultPlayer();
    CurrentRoom = _worldLoader.GetStartingRoom("tavern");
    _messageLog.Clear();
    _stateStack.Clear();
    PushState(new ExplorationState(ExplorationUI));
  }

  public void PushState(IGameState state)
  {
    _stateStack.Push(state);
  }

  public void PopState()
  {
    if (_stateStack.Count > 0)
    {
      _stateStack.Pop();
    }
  }

  public void ChangeState(IGameState? newState)
  {
    if (newState == null)
    {
      _stateStack.Clear();
      return;
    }

    if (_stateStack.Count > 0)
    {
      _stateStack.Pop();
    }
    _stateStack.Push(newState);
  }

  public void AddMessage(string message)
  {
    _messageLog.Add(message);
    if (_messageLog.Count > 6)
    {
      _messageLog.RemoveAt(0);
    }
  }

  public void Start()
  {
    ChangeState(new MainMenuState(MainMenuUI));
    RunLoop();
  }

  private void RunLoop()
  {
    while (CurrentState != null)
    {
      var state = CurrentState;
      state.Render(this);
      state.Update(this);
    }
  }
}
