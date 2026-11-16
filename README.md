# Baldur's Net 10.0 — RPG de Terminal em Texto

**Baldur's Net** é um jogo completo de RPG baseado em terminal e texto, minimalista na apresentação, mas com profundidade em seus sistemas e mecânicas: exploração de masmorras em grafo interconectado, inventário com peso e limite de capacidade, transferência com recipientes (baús/armários), espaços dedicados para equipamentos, NPCs com diálogos e comércio, sistema de combate tático por turnos com IA e magias, e arquitetura de dados 100% aberta baseada em arquivos JSON.

---

## 🎮 Como Executar

### 1. Iniciar o Jogo
Para compilar e jogar:
```bash
dotnet run
```

### 2. Executar a Suíte de Testes Automatizados
O projeto conta com uma verificação automatizada completa que testa todas as regras de negócio (JSON loading, grafo, portas trancadas, limite de carga, equipamentos, consumíveis e combate):
```bash
dotnet run -- --test
```

---

## 🗺️ 1. Estrutura do Mapa em Grafo e Navegação por Abas

O mundo é estruturado como um **grafo direcionado**, onde cada nó é um ambiente (`Room`) interligado por conexões/arestas (`RoomExit`). Cada sala possui itens soltos, personagens, monstros hostis e recipientes.

### Controles de Navegação:
- `[← / →]` : Alterna entre as abas da sala no topo da tela.
- `[↑ / ↓]` : Navega entre as opções e entidades da aba ativa.
- `[Enter]` : Interage com o elemento selecionado.
- `[I]` : Atalho instantâneo para abrir a Mochila & Equipamentos.
- `[ESC]` : Abre o Menu Principal e opções.

### Abas da Tela de Exploração:
- **`[ Saídas ]`** : Lista as passagens conectadas no grafo. Portas podem estar abertas ou trancadas (🔒). Se o jogador possuir a chave necessária na mochila, ela é destrancada automaticamente ao tentar passar!
- **`[ Inimigos ]`** : Criaturas hostis no ambiente. Interagir inicia o combate por turnos.
- **`[ Personagens ]`** : NPCs amigáveis para conversar, descansar ou comerciar mercadorias.
- **`[ Recipientes ]`** : Baús, armários, urnas e caixas. Abre a interface de saque e transferência de itens.
- **`[ Itens no Chão ]`** : Objetos largados no chão que podem ser coletados diretamente para a mochila.
- **`[ Herói & Mochila ]`** : Visualização direta da ficha de personagem, equipamentos e itens.

---

## 🎒 2. Sistema de Inventário, Containers e Equipamentos

### Capacidade Máxima e Peso
- Cada inventário possui uma capacidade máxima de carga em quilogramas (`MaxWeight`).
- Cada item possui seu peso individual (`Weight`) e valor em ouro (`Value`).
- Se uma transferência ou coleta for exceder a carga máxima, a ação é bloqueada com mensagem de aviso.

### Transferência entre Containers (Baús / Armários)
Ao interagir com qualquer recipiente no mundo:
- A interface exibe os itens do recipiente e os itens da mochila do jogador, junto com a carga em tempo real de ambos (ex: `8/25 kg` vs `14/37 kg`).
- `[← / →]` alternam entre o container e a mochila.
- `[Enter]` transfere o item selecionado para o outro lado (verificando espaço).
- `[T]` transfere todos os itens possíveis do recipiente para o jogador.
- `[G]` guarda todos os itens possíveis da mochila no recipiente.

### Espaços Especiais de Equipamento
O inventário do herói possui slots específicos para cada categoria de item:
- **Arma Principal (`Weapon`)**: concede bônus de Ataque (`+ATQ`).
- **Armadura de Tronco (`Armor`)**: concede bônus de Defesa (`+DEF`).
- **Escudo / Mão Secundária (`Shield`)**: concede Defesa adicional.
- **Elmo (`Helmet`)**: proteção de cabeça com Defesa extra.
- **Acessório (`Accessory`)**: anéis e amuletos que aumentam PV e PM máximos.
- **Atalhos Rápidos de Consumíveis (`QuickSlot1`, `QuickSlot2`)**: poções de vida e mana para uso rápido.

---

## ⚔️ 3. Sistema de Combate Tático por Turnos

Quando o herói enfrenta um monstro:
- **Atacar**: desfere golpe físico com a arma equipada, calculando variação, acerto crítico baseado em Destreza (`Agility`) e mitigação da defesa do monstro.
- **Habilidade / Magia**: usa técnicas especiais consumindo Pontos de Mana (PM), como *Golpe Feroz*, *Centelha Arcana* e *Bênção Menor* (cura).
- **Usar Item**: consome poções diretamente da mochila.
- **Postura Defensiva**: reduz em 50% o dano recebido no turno seguinte e recupera PM.
- **Fugir**: teste de fuga dinâmico baseado na agilidade do herói versus o nível do monstro.
- **Vitória**: concede Experiência (XP), Moedas de Ouro e gera espólios (drops) que vão para a mochila ou caem na sala.
- **Progressão (Level Up)**: ao acumular XP suficiente, o herói sobe de nível, aumentando Força, Destreza, Vitalidade, Inteligência e restaurando toda a vida e mana.

---

## 📂 4. Arquitetura Aberta e Conteúdo via JSON

Todo o conteúdo do jogo é desacoplado do código e carregado de arquivos JSON na pasta `Data/`:

1. **`Data/items.json`**:
   - Catálogo com armas, armaduras, escudos, elmos, anéis, poções, chaves e tesouros.
   - Propriedades: `Id`, `Name`, `Description`, `Type`, `Rarity`, `Weight`, `Value`, `AttackBonus`, `DefenseBonus`, `HpBonus`, `MpBonus`, `TargetSlot`, `HealAmount`, `ManaAmount`, `KeyId`.

2. **`Data/enemies.json`**:
   - Catálogo de monstros e chefes.
   - Propriedades: `Id`, `Name`, `EnemyType`, `Level`, `MaxHp`, `MaxMp`, `BaseAttack`, `BaseDefense`, `XpReward`, `GoldReward`, `Skills`, `LootDrops` com chances percentuais.

3. **`Data/world.json`**:
   - Grafo de salas do mundo.
   - Cada sala define:
     - `Id`, `Name`, `Description`
     - `Exits`: lista de saídas com `TargetRoomId`, `DisplayName`, `IsLocked`, `RequiredKeyId`, `LockedDescription`.
     - `Containers`: recipientes com `Capacity`, `IsLocked`, `RequiredKeyId` e lista de `Items`.
     - `Npcs`: personagens com diálogos, `IsMerchant` e `MerchantStock`.
     - `Enemies`: monstros presentes na sala.
     - `Items`: itens caídos no chão.

---

## 🏛️ 5. Arquitetura do Código

```
BaldursNet/
├── Application/
│   ├── DTOs/               # Modelos de transferência para serialização JSON
│   ├── Factories/          # Fábricas de salas e game objects
│   ├── Interfaces/         # IGameEngine, IWorldLoader, IUserInterface, IGameState
│   └── Services/           # GameEngine (loop principal, máquina de estados com Stack)
├── Domain/
│   ├── Entities/           # Player, Enemy, Npc, Container, Room, Item, Inventory...
│   └── Enums/              # ItemType, EquipmentSlot, ItemRarity
├── Infrastructure/
│   └── Data/               # WorldLoader, ItemRepository, EnemyRepository
├── Presentation/
│   ├── ConsoleUI/          # ExplorationUI, CombatUI, LootUI, PlayerInventoryUI, DialogueUI...
│   └── UIComponents/       # RoomTabs, SelectibleMenu
├── State/                  # ExplorationState, CombatState, LootState, PlayerInventoryState...
├── Data/                   # items.json, enemies.json, world.json
├── Tests/                  # Suíte de verificação automatizada
└── Program.cs              # Ponto de entrada da aplicação
```
