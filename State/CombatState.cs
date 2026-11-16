using System;
using System.Collections.Generic;
using BaldursNet.Application.Interfaces;
using BaldursNet.Application.Services;
using BaldursNet.Domain.Entities;
using BaldursNet.Domain.Enums;
using BaldursNet.Presentation.ConsoleUI;

namespace BaldursNet.State;

public class CombatState : IGameState
{
  private readonly Enemy _enemy;
  private readonly CombatUI _ui = new();
  private readonly List<string> _combatLog = [];
  private readonly Random _random = new();

  private bool _isDefending = false;
  private bool _combatFinished = false;

  public CombatState(Enemy enemy)
  {
    _enemy = enemy;
    _combatLog.Add($"Um confronto temível começou contra {_enemy.Name}!");
  }

  public void Render(GameEngine engine)
  {
    if (_combatFinished) return;

    var result = _ui.RenderCombatTurn(engine.Player, _enemy, _combatLog);
    ExecutePlayerTurn(engine, result.Action, result.Extra);
  }

  public void Update(GameEngine engine)
  {
    if (_combatFinished) return;

    // Checa se o inimigo morreu
    if (!_enemy.IsAlive)
    {
      _combatFinished = true;
      HandleVictory(engine);
      return;
    }

    // Turno do Inimigo
    ExecuteEnemyTurn(engine);

    // Checa se o jogador morreu
    if (!engine.Player.IsAlive)
    {
      _combatFinished = true;
      HandleDefeat(engine);
    }
  }

  private void ExecutePlayerTurn(GameEngine engine, CombatUI.CombatAction action, object? extra)
  {
    var p = engine.Player;
    _isDefending = false;

    switch (action)
    {
      case CombatUI.CombatAction.Attack:
        int baseDmg = p.TotalAttack;
        double variance = (_random.NextDouble() * 0.4) + 0.8; // 80% a 120%
        int dmg = Math.Max(1, (int)(baseDmg * variance) - _enemy.BaseDefense);

        bool isCrit = _random.Next(100) < (p.Agility * 2);
        if (isCrit)
        {
          dmg = (int)(dmg * 1.8);
          _combatLog.Add($"★ ACERTO CRÍTICO! Você atingiu {_enemy.Name} causando {dmg} de dano!");
        }
        else
        {
          _combatLog.Add($"Você atacou {_enemy.Name} desferindo {dmg} de dano.");
        }

        _enemy.TakeDamage(dmg);
        break;

      case CombatUI.CombatAction.Skill:
        if (extra is Skill skill)
        {
          p.SpendMp(skill.ManaCost);

          if (skill.IsHealing)
          {
            p.Heal(skill.Power);
            _combatLog.Add($"Você conjurou {skill.Name} e regenerou {skill.Power} PV!");
          }
          else
          {
            int skillDmg = Math.Max(2, skill.Power + (p.Intelligence / 2) - (_enemy.BaseDefense / 2));
            _enemy.TakeDamage(skillDmg);
            _combatLog.Add($"Você conjurou {skill.Name} contra {_enemy.Name} causando {skillDmg} de dano ({skill.Element})!");
          }
        }
        break;

      case CombatUI.CombatAction.UseItem:
        if (extra is Item consumable)
        {
          if (p.UseConsumable(consumable, out var msg))
          {
            _combatLog.Add(msg);
          }
        }
        break;

      case CombatUI.CombatAction.Defend:
        _isDefending = true;
        p.RestoreMp(5);
        _combatLog.Add("Você assumiu postura defensiva (+50% defesa no próximo ataque e recuperou 5 PM).");
        break;

      case CombatUI.CombatAction.Flee:
        int fleeChance = 40 + (p.Agility * 3) - (_enemy.Level * 5);
        if (_random.Next(100) < fleeChance)
        {
          _combatLog.Add("Você conseguiu escapar do combate com agilidade!");
          engine.AddMessage($"Você fugiu do combate contra {_enemy.Name}!");
          _combatFinished = true;
          engine.PopState();
          return;
        }
        else
        {
          _combatLog.Add($"Você tentou fugir, mas {_enemy.Name} bloqueou seu caminho!");
        }
        break;
    }
  }

  private void ExecuteEnemyTurn(GameEngine engine)
  {
    var p = engine.Player;

    // Se tiver habilidades com mana, pode tentar usar
    Skill? chosenSkill = null;
    foreach (var sk in _enemy.Skills)
    {
      if (_enemy.CurrentMp >= sk.ManaCost && _random.Next(100) < 40)
      {
        chosenSkill = sk;
        break;
      }
    }

    if (chosenSkill != null)
    {
      _enemy.SpendMp(chosenSkill.ManaCost);
      int rawDmg = chosenSkill.Power;
      int def = p.TotalDefense;
      int dmg = Math.Max(1, rawDmg - (def / 2));
      if (_isDefending) dmg = Math.Max(1, dmg / 2);

      p.TakeDamage(dmg);
      _combatLog.Add($"⚡ {_enemy.Name} usou {chosenSkill.Name} atingindo você em {dmg} de dano!");
    }
    else
    {
      int rawDmg = _enemy.BaseAttack + _random.Next(-2, 3);
      int def = p.TotalDefense;
      int dmg = Math.Max(1, rawDmg - (def / 2));
      if (_isDefending) dmg = Math.Max(1, dmg / 2);

      p.TakeDamage(dmg);
      _combatLog.Add($"⚔ {_enemy.Name} atacou você causando {dmg} de dano!");
    }
  }

  private void HandleVictory(GameEngine engine)
  {
    var p = engine.Player;
    var drops = _enemy.GenerateLoot();
    int xp = _enemy.XpReward;
    int gold = _enemy.GoldReward;

    p.GainGold(gold);
    bool leveledUp = p.GainXp(xp, out _);

    // Adiciona drops ao inventário ou ao chão da sala se cheio
    foreach (var drop in drops)
    {
      if (!p.Inventory.AddItem(drop))
      {
        engine.CurrentRoom.AddObject(new WorldItem(drop));
        engine.AddMessage($"Sua mochila estava cheia; {drop.Name} caiu no chão da sala.");
      }
    }

    // Remove inimigo da sala
    engine.CurrentRoom.RemoveObject(_enemy);
    engine.AddMessage($"Você derrotou {_enemy.Name} e ganhou {xp} XP e {gold} moedas de ouro!");

    _ui.ShowVictoryScreen(p, _enemy, drops, xp, gold, leveledUp);
    engine.PopState();
  }

  private void HandleDefeat(GameEngine engine)
  {
    _ui.ShowDefeatScreen(engine.Player, _enemy);
    // Reinicia jogo
    engine.RestartGame();
  }
}
