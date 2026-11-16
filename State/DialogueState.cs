using BaldursNet.Application.Interfaces;
using BaldursNet.Application.Services;
using BaldursNet.Domain.Entities;
using BaldursNet.Presentation.ConsoleUI;

namespace BaldursNet.State;

public class DialogueState : IGameState
{
  private readonly Npc _npc;
  private readonly DialogueUI _ui = new();
  private DialogueUI.DialogueAction _action;

  public DialogueState(Npc npc)
  {
    _npc = npc;
  }

  public void Render(GameEngine engine)
  {
    _action = _ui.Render(_npc, engine.Player);
  }

  public void Update(GameEngine engine)
  {
    var p = engine.Player;

    switch (_action)
    {
      case DialogueUI.DialogueAction.OpenShop:
        if (_npc.MerchantStock != null)
        {
          engine.PushState(new MerchantState(_npc));
        }
        break;

      case DialogueUI.DialogueAction.Rest:
        if (p.SpendGold(10))
        {
          p.CurrentHp = p.TotalMaxHp;
          p.CurrentMp = p.TotalMaxMp;
          engine.AddMessage("Você descansou em uma cama confortável na Taverna. Seus PV e PM foram totalmente recuperados!");
          engine.PopState();
        }
        else
        {
          engine.AddMessage("Você não tem ouro suficiente para pagar uma estadia (Custa 10 moedas).");
        }
        break;

      case DialogueUI.DialogueAction.Leave:
        engine.PopState();
        break;
    }

    _action = DialogueUI.DialogueAction.None;
  }
}
