using System.Linq;
using BaldursNet.Application.Interfaces;
using BaldursNet.Application.Services;
using BaldursNet.Domain.Entities;
using BaldursNet.Presentation.ConsoleUI;

namespace BaldursNet.State;

public class LootState : IGameState
{
  private readonly Container _container;
  private readonly LootUI _ui = new();
  private LootUI.LootAction _action;
  private Item? _targetItem;
  private string? _statusMessage;

  public LootState(Container container)
  {
    _container = container;
  }

  public void Render(GameEngine engine)
  {
    var result = _ui.Render(_container, engine.Player, _statusMessage);
    _action = result.Action;
    _targetItem = result.TargetItem;
    _statusMessage = null;
  }

  public void Update(GameEngine engine)
  {
    var player = engine.Player;

    switch (_action)
    {
      case LootUI.LootAction.TakeItem:
        if (_targetItem != null)
        {
          if (_container.Inventory.TransferTo(player.Inventory, _targetItem, out var msg))
          {
            _statusMessage = msg;
            engine.AddMessage($"Pegou {_targetItem.Name} de {_container.Name}.");
          }
          else
          {
            _statusMessage = msg;
          }
        }
        break;

      case LootUI.LootAction.StoreItem:
        if (_targetItem != null)
        {
          if (player.Inventory.TransferTo(_container.Inventory, _targetItem, out var msg))
          {
            _statusMessage = msg;
            engine.AddMessage($"Guardou {_targetItem.Name} em {_container.Name}.");
          }
          else
          {
            _statusMessage = msg;
          }
        }
        break;

      case LootUI.LootAction.TakeAll:
        int takenCount = 0;
        var itemsToTake = _container.Inventory.Items.ToList();
        foreach (var item in itemsToTake)
        {
          if (_container.Inventory.TransferTo(player.Inventory, item, out _))
          {
            takenCount++;
          }
        }
        _statusMessage = takenCount > 0
          ? $"Você transferiu {takenCount} itens para sua mochila!"
          : "Não foi possível transferir mais itens (mochila cheia).";
        break;

      case LootUI.LootAction.StoreAll:
        int storedCount = 0;
        var itemsToStore = player.Inventory.Items.ToList();
        foreach (var item in itemsToStore)
        {
          if (player.Inventory.TransferTo(_container.Inventory, item, out _))
          {
            storedCount++;
          }
        }
        _statusMessage = storedCount > 0
          ? $"Você guardou {storedCount} itens no recipiente!"
          : "Não foi possível guardar mais itens (recipiente cheio).";
        break;

      case LootUI.LootAction.Close:
        engine.PopState();
        break;
    }

    _action = LootUI.LootAction.None;
    _targetItem = null;
  }
}
