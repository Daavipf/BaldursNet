using System;
using BaldursNet.Application.Interfaces;
using BaldursNet.Application.Services;

namespace BaldursNet.State;

public class LoadMenuState : IGameState
{
  public void Render(GameEngine engine)
  {
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("╔══════════════════════════════════════════════════════════════════════════════╗");
    Console.WriteLine("║                        GUIA DE SISTEMAS & MECÂNICAS                          ║");
    Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════════╝");
    Console.ResetColor();

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("\n► 1. EXPLORAÇÃO EM GRAFO E NAVEGAÇÃO POR ABAS:");
    Console.ResetColor();
    Console.WriteLine("  * O mapa é um grafo de salas conectadas por saídas.");
    Console.WriteLine("  * Use as setas [← / →] para alternar entre as abas na tela principal:");
    Console.WriteLine("    - Saídas: portas e passagens (algumas trancadas que exigem chaves!).");
    Console.WriteLine("    - Inimigos: monstros na sala prontos para o combate.");
    Console.WriteLine("    - Personagens: NPCs para conversar e negociar mercadorias.");
    Console.WriteLine("    - Recipientes: baús, armários e sarcófagos para saquear e guardar itens.");
    Console.WriteLine("    - Itens no Chão: itens soltos que podem ser coletados diretamente.");
    Console.WriteLine("    - Herói & Mochila: status completo, ficha de RPG e equipamentos.");

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("\n► 2. INVENTÁRIO, CONTAINERS E EQUIPAMENTOS:");
    Console.ResetColor();
    Console.WriteLine("  * Cada inventário tem uma capacidade máxima de carga em kg.");
    Console.WriteLine("  * Cada item ocupa um espaço de peso proporcional.");
    Console.WriteLine("  * O jogador possui espaços dedicados para equipamentos especiais:");
    Console.WriteLine("    - Arma (aumenta o dano de ataque).");
    Console.WriteLine("    - Armadura, Escudo e Elmo (aumentam a defesa e mitigam dano recebido).");
    Console.WriteLine("    - Acessório (concede bônus de vida e mana).");
    Console.WriteLine("    - Atalhos de Consumíveis (para uso rápido em combate e exploração).");

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("\n► 3. SISTEMA DE COMBATE POR TURNOS:");
    Console.ResetColor();
    Console.WriteLine("  * Ao enfrentar inimigos, o combate tático por turnos é ativado.");
    Console.WriteLine("  * Escolha entre atacar com sua arma, lançar magias (consumindo PM),");
    Console.WriteLine("    usar itens consumíveis, assumir postura defensiva ou tentar fugir.");
    Console.WriteLine("  * Vencer monstros concede Experiência (XP), Ouro e espólios (drops).");
    Console.WriteLine("  * Ao subir de nível, todos os seus atributos aumentam e PV/PM são curados!");

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("\n► 4. ARQUITETURA ABERTA BASEADA EM JSON:");
    Console.ResetColor();
    Console.WriteLine("  * Todo o conteúdo do jogo está salvo na pasta 'Data/':");
    Console.WriteLine("    - Data/items.json: catálogo de armas, armaduras, consumíveis e chaves.");
    Console.WriteLine("    - Data/enemies.json: lista de monstros com stats, magias e drops.");
    Console.WriteLine("    - Data/world.json: definição de nós de salas, recipientes, NPCs e conexões.");

    Console.ForegroundColor = ConsoleColor.DarkYellow;
    Console.WriteLine("\n" + new string('─', 78));
    Console.WriteLine("[Pressione qualquer tecla para retornar ao menu]");
    Console.ResetColor();
    Console.ReadKey(true);
  }

  public void Update(GameEngine engine)
  {
    engine.PopState();
  }
}
