using System.Linq;
using BaldursNet.Application.Interfaces;
using BaldursNet.Application.Services;
using BaldursNet.Domain.Entities;
using BaldursNet.Presentation.UIComponents;

namespace BaldursNet.State;

public class ExplorationState : IGameState
{
  private readonly IUserInterface _ui;
  private string? _selectedTab;
  private int _selectedIndex = -1;

  public ExplorationState(IUserInterface ui)
  {
    _ui = ui;
  }

  public void Update(GameEngine engine)
  {
    if (_selectedIndex == -1 || string.IsNullOrEmpty(_selectedTab))
    {
      // Pressionou ESC: volta para o menu principal
      engine.PushState(new MainMenuState(engine.MainMenuUI));
      return;
    }

    if (_selectedTab.StartsWith("Saídas"))
    {
      var exits = engine.CurrentRoom.GetAvailableExits();
      if (_selectedIndex >= 0 && _selectedIndex < exits.Count)
      {
        var exit = exits[_selectedIndex];

        if (exit.IsLocked)
        {
          if (!string.IsNullOrEmpty(exit.RequiredKeyId) && engine.Player.HasKey(exit.RequiredKeyId))
          {
            exit.IsLocked = false;
            engine.AddMessage($"Você usou a chave e destrancou {exit.DisplayName}!");
            if (exit.TargetRoom != null)
            {
              engine.CurrentRoom = exit.TargetRoom;
            }
          }
          else
          {
            engine.AddMessage(exit.LockedDescription ?? "Esta passagem está trancada! Você precisa da chave.");
          }
        }
        else if (exit.TargetRoom != null)
        {
          engine.CurrentRoom = exit.TargetRoom;
          engine.AddMessage($"Você se deslocou até {engine.CurrentRoom.Name}.");
        }
      }
    }
    else if (_selectedTab.StartsWith("Inimigos"))
    {
      var enemies = engine.CurrentRoom.GetAliveEnemies();
      if (_selectedIndex >= 0 && _selectedIndex < enemies.Count)
      {
        var enemy = enemies[_selectedIndex];
        engine.PushState(new CombatState(enemy));
      }
    }
    else if (_selectedTab.StartsWith("Personagens"))
    {
      var npcs = engine.CurrentRoom.GetNpcs();
      if (_selectedIndex >= 0 && _selectedIndex < npcs.Count)
      {
        var npc = npcs[_selectedIndex];
        engine.PushState(new DialogueState(npc));
      }
    }
    else if (_selectedTab.StartsWith("Recipientes"))
    {
      var containers = engine.CurrentRoom.GetContainers();
      if (_selectedIndex >= 0 && _selectedIndex < containers.Count)
      {
        var container = containers[_selectedIndex];

        if (container.IsLocked)
        {
          if (!string.IsNullOrEmpty(container.RequiredKeyId) && engine.Player.HasKey(container.RequiredKeyId))
          {
            container.IsLocked = false;
            engine.AddMessage($"Você usou a chave e destrancou {container.Name}!");
            engine.PushState(new LootState(container));
          }
          else
          {
            engine.AddMessage($"{container.Name} está trancado! Requer a chave correta.");
          }
        }
        else
        {
          engine.PushState(new LootState(container));
        }
      }
    }
    else if (_selectedTab.StartsWith("Itens no Chão"))
    {
      var worldItems = engine.CurrentRoom.GetWorldItems();
      if (_selectedIndex >= 0 && _selectedIndex < worldItems.Count)
      {
        var wi = worldItems[_selectedIndex];
        if (engine.Player.Inventory.CanAddItem(wi.Item))
        {
          engine.Player.Inventory.AddItem(wi.Item);
          engine.CurrentRoom.RemoveObject(wi);
          engine.AddMessage($"Você pegou {wi.Item.Name} ({wi.Item.Weight} kg) do chão.");
        }
        else
        {
          engine.AddMessage($"Mochila pesada demais! Capacidade: {engine.Player.Inventory.CurrentWeight}/{engine.Player.Inventory.MaxWeight} kg.");
        }
      }
    }
    else if (_selectedTab.StartsWith("Herói"))
    {
      engine.PushState(new PlayerInventoryState());
    }

    _selectedIndex = -1;
    _selectedTab = null;
  }

  public void Render(GameEngine engine)
  {
    var result = _ui.RenderScreen(engine);

    if (result == null)
    {
      _selectedIndex = -1;
      _selectedTab = null;
    }
    else
    {
      _selectedTab = result.Value.Tab;
      _selectedIndex = result.Value.Index;
    }
  }
}
