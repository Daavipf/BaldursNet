using System;
using BaldursNet.Application.Interfaces;
using BaldursNet.Application.Services;
using BaldursNet.Domain.Entities;
using BaldursNet.Presentation.ConsoleUI;

namespace BaldursNet.State;

public class MerchantState : IGameState
{
  private readonly Npc _merchant;
  private readonly MerchantUI _ui = new();
  private MerchantUI.ShopAction _action;
  private Item? _item;
  private string? _message;

  public MerchantState(Npc merchant)
  {
    _merchant = merchant;
  }

  public void Render(GameEngine engine)
  {
    var result = _ui.Render(_merchant, engine.Player, _message);
    _action = result.Action;
    _item = result.Item;
    _message = null;
  }

  public void Update(GameEngine engine)
  {
    var p = engine.Player;
    var stock = _merchant.MerchantStock;

    switch (_action)
    {
      case MerchantUI.ShopAction.Buy:
        if (_item != null && stock != null)
        {
          if (p.Gold < _item.Value)
          {
            _message = "Ouro insuficiente para comprar este item!";
          }
          else if (!p.Inventory.CanAddItem(_item))
          {
            _message = "Sua mochila está muito cheia para carregar este item!";
          }
          else
          {
            p.SpendGold(_item.Value);
            stock.RemoveItem(_item);
            p.Inventory.AddItem(_item);
            _message = $"Você comprou {_item.Name} por {_item.Value} moedas de ouro!";
            engine.AddMessage($"Comprou {_item.Name}.");
          }
        }
        break;

      case MerchantUI.ShopAction.Sell:
        if (_item != null && stock != null)
        {
          int sellPrice = Math.Max(1, _item.Value / 2);
          p.Inventory.RemoveItem(_item);
          p.GainGold(sellPrice);
          stock.AddItem(_item);
          _message = $"Você vendeu {_item.Name} por {sellPrice} moedas de ouro!";
          engine.AddMessage($"Vendeu {_item.Name}.");
        }
        break;

      case MerchantUI.ShopAction.Close:
        engine.PopState();
        break;
    }

    _action = MerchantUI.ShopAction.None;
    _item = null;
  }
}
