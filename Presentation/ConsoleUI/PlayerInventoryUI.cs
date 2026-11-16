using System;
using System.Collections.Generic;
using System.Linq;
using BaldursNet.Domain.Entities;
using BaldursNet.Domain.Enums;
using BaldursNet.Presentation.UIComponents;

namespace BaldursNet.Presentation.ConsoleUI;

public class PlayerInventoryUI
{
  public enum InventoryAction
  {
    None,
    EquipItem,
    UnequipSlot,
    UseConsumable,
    DropItem,
    Close
  }

  public (InventoryAction Action, object? Target) Render(Player player, Room currentRoom)
  {
    int currentSection = 0; // 0 = Mochila, 1 = Equipamentos
    int bagIndex = 0;
    int equipIndex = 0;

    var equipSlots = new List<(string Label, EquipmentSlot Slot)>
    {
      ("Arma Principal", EquipmentSlot.Weapon),
      ("Armadura de Tronco", EquipmentSlot.Armor),
      ("Escudo / Mão Secundária", EquipmentSlot.Shield),
      ("Elmo de Proteção", EquipmentSlot.Helmet),
      ("Acessório Especial", EquipmentSlot.Accessory),
      ("Consumível Rápido 1", EquipmentSlot.QuickSlot1),
      ("Consumível Rápido 2", EquipmentSlot.QuickSlot2)
    };

    while (true)
    {
      Console.Clear();

      RenderHeader(player);

      Console.WriteLine();
      Console.ForegroundColor = ConsoleColor.Cyan;
      Console.Write(currentSection == 0 ? " [ MOCHILA DE ITENS ] " : "   Mochila de Itens   ");
      Console.ResetColor();
      Console.Write("    ");
      Console.ForegroundColor = ConsoleColor.Cyan;
      Console.WriteLine(currentSection == 1 ? " [ ESPAÇOS DE EQUIPAMENTO ] " : "   Espaços de Equipamento   ");
      Console.ResetColor();
      Console.WriteLine(new string('─', 78));

      if (currentSection == 0)
      {
        // Renderiza itens da mochila
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Carga Total: {player.Inventory.CurrentWeight} / {player.Inventory.MaxWeight} kg  ({player.Inventory.Items.Count} itens)");
        Console.ResetColor();
        Console.WriteLine();

        if (player.Inventory.Items.Count == 0)
        {
          Console.ForegroundColor = ConsoleColor.DarkGray;
          Console.WriteLine("  [Sua mochila está vazia]");
          Console.ResetColor();
        }
        else
        {
          for (int i = 0; i < player.Inventory.Items.Count; i++)
          {
            var item = player.Inventory.Items[i];
            string selector = (i == bagIndex) ? " ► " : "   ";
            if (i == bagIndex)
            {
              Console.ForegroundColor = ConsoleColor.Green;
              Console.WriteLine($"{selector}{item.Name} - {item.Weight} kg {item.GetStatSummary()}");
              Console.ResetColor();
              Console.ForegroundColor = ConsoleColor.DarkGray;
              Console.WriteLine($"       └ {item.Description}");
              Console.ResetColor();
            }
            else
            {
              Console.WriteLine($"{selector}{item.Name} - {item.Weight} kg {item.GetStatSummary()}");
            }
          }
        }
      }
      else
      {
        // Renderiza equipamentos
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Bônus de Equipamentos: +{player.Equipment.TotalAttackBonus} ATQ  |  +{player.Equipment.TotalDefenseBonus} DEF");
        Console.ResetColor();
        Console.WriteLine();

        for (int i = 0; i < equipSlots.Count; i++)
        {
          var slotInfo = equipSlots[i];
          var equippedItem = player.Equipment.GetItem(slotInfo.Slot);
          string selector = (i == equipIndex) ? " ► " : "   ";
          string itemStr = equippedItem != null
            ? $"{equippedItem.Name} {equippedItem.GetStatSummary()}"
            : "[Vazio]";

          if (i == equipIndex)
          {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{selector}[{slotInfo.Label}]: {itemStr}");
            Console.ResetColor();
          }
          else
          {
            Console.WriteLine($"{selector}[{slotInfo.Label}]: {itemStr}");
          }
        }
      }

      Console.ForegroundColor = ConsoleColor.DarkYellow;
      Console.WriteLine("\n" + new string('─', 78));
      Console.WriteLine("[←/→] Alternar Mochila / Equipamentos  |  [↑/↓] Navegar  |  [Enter] Ações  |  [ESC] Voltar");
      Console.ResetColor();

      var key = Console.ReadKey(intercept: true).Key;

      if (key == ConsoleKey.LeftArrow || key == ConsoleKey.RightArrow)
      {
        currentSection = (currentSection == 0) ? 1 : 0;
      }
      else if (key == ConsoleKey.UpArrow)
      {
        if (currentSection == 0 && player.Inventory.Items.Count > 0)
        {
          bagIndex = (bagIndex <= 0) ? player.Inventory.Items.Count - 1 : bagIndex - 1;
        }
        else if (currentSection == 1)
        {
          equipIndex = (equipIndex <= 0) ? equipSlots.Count - 1 : equipIndex - 1;
        }
      }
      else if (key == ConsoleKey.DownArrow)
      {
        if (currentSection == 0 && player.Inventory.Items.Count > 0)
        {
          bagIndex = (bagIndex >= player.Inventory.Items.Count - 1) ? 0 : bagIndex + 1;
        }
        else if (currentSection == 1)
        {
          equipIndex = (equipIndex >= equipSlots.Count - 1) ? 0 : equipIndex + 1;
        }
      }
      else if (key == ConsoleKey.Escape)
      {
        return (InventoryAction.Close, null);
      }
      else if (key == ConsoleKey.Enter)
      {
        if (currentSection == 0 && player.Inventory.Items.Count > 0 && bagIndex < player.Inventory.Items.Count)
        {
          var item = player.Inventory.Items[bagIndex];
          var action = ShowItemSubMenu(item);
          if (action != InventoryAction.None)
          {
            return (action, item);
          }
        }
        else if (currentSection == 1)
        {
          var slot = equipSlots[equipIndex].Slot;
          var equipped = player.Equipment.GetItem(slot);
          if (equipped != null)
          {
            return (InventoryAction.UnequipSlot, slot);
          }
        }
      }
    }
  }

  private static InventoryAction ShowItemSubMenu(Item item)
  {
    var options = new List<(string Label, InventoryAction Action)>();

    if (item.TargetSlot.HasValue && item.Type != ItemType.Consumable)
    {
      options.Add(($"Equipar em {item.TargetSlot.Value}", InventoryAction.EquipItem));
    }

    if (item.Type == ItemType.Consumable)
    {
      options.Add(("Usar / Consumir agora", InventoryAction.UseConsumable));
      if (item.TargetSlot.HasValue)
      {
        options.Add(($"Colocar no atalho {item.TargetSlot.Value}", InventoryAction.EquipItem));
      }
    }

    options.Add(("Largar no chão da sala", InventoryAction.DropItem));
    options.Add(("Cancelar", InventoryAction.None));

    int selected = 0;
    while (true)
    {
      Console.Clear();
      Console.ForegroundColor = ConsoleColor.Cyan;
      Console.WriteLine($"=== ITEM: {item.Name.ToUpper()} ===");
      Console.ResetColor();
      Console.WriteLine($"{item.Description}\n");
      Console.WriteLine($"Tipo: {item.Type} | Peso: {item.Weight} kg | Valor: {item.Value} ouro {item.GetStatSummary()}");
      Console.WriteLine(new string('─', 50));

      for (int i = 0; i < options.Count; i++)
      {
        if (i == selected)
        {
          Console.ForegroundColor = ConsoleColor.Green;
          Console.WriteLine($" ► {options[i].Label}");
          Console.ResetColor();
        }
        else
        {
          Console.WriteLine($"   {options[i].Label}");
        }
      }

      var key = Console.ReadKey(intercept: true).Key;
      if (key == ConsoleKey.UpArrow)
      {
        selected = (selected <= 0) ? options.Count - 1 : selected - 1;
      }
      else if (key == ConsoleKey.DownArrow)
      {
        selected = (selected >= options.Count - 1) ? 0 : selected + 1;
      }
      else if (key == ConsoleKey.Enter)
      {
        return options[selected].Action;
      }
      else if (key == ConsoleKey.Escape)
      {
        return InventoryAction.None;
      }
    }
  }

  private static void RenderHeader(Player player)
  {
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"╔═══════════════════════════ FICHA DO HERÓI: {player.Name.ToUpper()} ═══════════════════════════╗");
    Console.ResetColor();
    Console.WriteLine($"  Nível: {player.Level}  |  XP: {player.CurrentXp}/{player.XpToNextLevel}  |  Ouro: {player.Gold} 🪙");
    Console.WriteLine($"  Pontos de Vida (PV): {player.CurrentHp} / {player.TotalMaxHp}  |  Pontos de Mana (PM): {player.CurrentMp} / {player.TotalMaxMp}");
    Console.WriteLine($"  Força: {player.Strength}  |  Destreza: {player.Agility}  |  Inteligência: {player.Intelligence}  |  Vitalidade: {player.Vitality}");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"  Ataque Total: {player.TotalAttack}  |  Defesa Total: {player.TotalDefense}");
    Console.ResetColor();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"╚{new string('═', 76)}╝");
    Console.ResetColor();
  }
}
