using System;

namespace BaldursNet.Domain.Entities;

public class Character : GameObject
{
  public int Level { get; set; } = 1;
  public int MaxHp { get; set; } = 100;
  public int CurrentHp { get; set; } = 100;
  public int MaxMp { get; set; } = 30;
  public int CurrentMp { get; set; } = 30;
  public int BaseAttack { get; set; } = 5;
  public int BaseDefense { get; set; } = 2;

  public bool IsAlive => CurrentHp > 0;

  public Character() { }

  public Character(int life, string name, string description, Position pos)
    : base(name, description, pos)
  {
    MaxHp = life;
    CurrentHp = life;
  }

  public virtual void TakeDamage(int damage)
  {
    CurrentHp = Math.Clamp(CurrentHp - damage, 0, MaxHp);
  }

  public virtual void Heal(int amount)
  {
    CurrentHp = Math.Clamp(CurrentHp + amount, 0, MaxHp);
  }

  public virtual void RestoreMp(int amount)
  {
    CurrentMp = Math.Clamp(CurrentMp + amount, 0, MaxMp);
  }

  public virtual bool SpendMp(int amount)
  {
    if (CurrentMp < amount) return false;
    CurrentMp -= amount;
    return true;
  }
}
