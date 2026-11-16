using BaldursNet.Application.Interfaces;
using BaldursNet.Application.Services;
using BaldursNet.Domain.Entities;
using BaldursNet.Domain.Enums;
using BaldursNet.Presentation.ConsoleUI;

namespace BaldursNet.State;

public class PlayerInventoryState : IGameState
{
  private readonly PlayerInventoryUI _ui = new();
  private PlayerInventoryUI.InventoryAction _action;
  private object? _actionTarget;

  public void Render(GameEngine engine)
  {
    var result = _ui.Render(engine.Player, engine.CurrentRoom);
    _action = result.Action;
    _actionTarget = result.Target;
  }

  public void Update(GameEngine engine)
  {
    var p = engine.Player;

    switch (_action)
    {
      case PlayerInventoryUI.InventoryAction.EquipItem:
        if (_actionTarget is Item itemToEquip)
        {
          if (p.Equip(itemToEquip, out var msg))
          {
            engine.AddMessage(msg);
          }
          else
          {
            engine.AddMessage(msg);
          }
        }
        break;

      case PlayerInventoryUI.InventoryAction.UnequipSlot:
        if (_actionTarget is EquipmentSlot slot)
        {
          if (p.Unequip(slot, out var msg))
          {
            engine.AddMessage(msg);
          }
          else
          {
            engine.AddMessage(msg);
          }
        }
        break;

      case PlayerInventoryUI.InventoryAction.UseConsumable:
        if (_actionTarget is Item itemToUse)
        {
          if (p.UseConsumable(itemToUse, out var msg))
          {
            engine.AddMessage(msg);
          }
          else
          {
            engine.AddMessage(msg);
          }
        }
        break;

      case PlayerInventoryUI.InventoryAction.DropItem:
        if (_actionTarget is Item itemToDrop)
        {
          p.Inventory.RemoveItem(itemToDrop);
          engine.CurrentRoom.AddObject(new WorldItem(itemToDrop));
          engine.AddMessage($"Você largou {itemToDrop.Name} no chão da sala.");
        }
        break;

      case PlayerInventoryUI.InventoryAction.Close:
        engine.PopState();
        break;
    }

    _action = PlayerInventoryUI.InventoryAction.None;
    _actionTarget = null;
  }
}
