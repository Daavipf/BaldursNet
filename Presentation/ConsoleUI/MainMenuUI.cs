using System;
using System.Collections.Generic;
using BaldursNet.Application.Interfaces;
using BaldursNet.Application.Services;
using BaldursNet.Presentation.UIComponents;

namespace BaldursNet.Presentation.ConsoleUI;

public class MainMenuUI : IUserInterface
{
  public (string? Tab, int Index)? RenderScreen(GameEngine engine)
  {
    bool inGame = engine.CurrentRoom != null;
    var items = inGame
      ? new List<string> { "Continuar Aventura", "Guia & Mecânicas", "Reiniciar Novo Jogo", "Sair" }
      : new List<string> { "Iniciar Nova Aventura", "Guia & Mecânicas", "Sobre o Jogo", "Sair" };

    SelectionMenuParams<string> menuParams = new(
      title: "BALDUR'S NET - RPG DE TERMINAL",
      items: items,
      canCancel: false,
      prompt: "\n[↑/↓] Selecionar  |  [Enter] Confirmar"
    );

    int selectedIndex = 0;

    while (true)
    {
      Console.Clear();

      Console.ForegroundColor = ConsoleColor.Cyan;
      Console.WriteLine(@"
  ____            _       _                 _   _      _   
 |  _ \          | |     | |               | \ | |    | |  
 | |_) | __ _  __| | _   | | _ __  ___     |  \| | ___| |_ 
 |  _ < / _` |/ _` || | | | || '__/ __|    | . ` |/ _ \ __|
 | |_) | (_| | (_| || |_| | || |  \__ \    | |\  |  __/ |_ 
 |____/ \__,_|\__,_| \__,_|_||_|  |___/    |_| \_|\___|\__|
                                                           ");
      Console.ResetColor();

      Console.ForegroundColor = ConsoleColor.DarkGray;
      Console.WriteLine("    Um RPG Tático em Texto no Terminal com Grafo de Ambientes e Inventário\n");
      Console.ResetColor();

      SelectibleMenu.ShowMenu(menuParams, selectedIndex);

      var key = Console.ReadKey(intercept: true).Key;

      if (key == ConsoleKey.UpArrow)
      {
        selectedIndex = (selectedIndex <= 0) ? items.Count - 1 : selectedIndex - 1;
      }
      else if (key == ConsoleKey.DownArrow)
      {
        selectedIndex = (selectedIndex >= items.Count - 1) ? 0 : selectedIndex + 1;
      }
      else if (key == ConsoleKey.Enter)
      {
        return (null, selectedIndex);
      }
    }
  }

  public void ShowMessage(string message)
  {
    Console.WriteLine(message);
    Console.WriteLine("\n[Pressione qualquer tecla para continuar]");
    Console.ReadKey(true);
  }
}
