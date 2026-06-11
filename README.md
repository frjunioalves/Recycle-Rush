# ♻️ Recycle Rush

Jogo 2D de triagem, crafting e economia baseado em reciclagem. Separe o lixo na esteira, descarte os não recicláveis no triturador, e bata a meta financeira antes que o tempo acabe.

---

## Estado do Projeto

> Desenvolvido em Unity 2D com URP e o novo Input System.

### Implementado
- Esteira com spawn contínuo e movimento automático dos itens
- Jogador com movimento livre e inventário de mão única (1 item por vez)
- Lixeiras categorizadas com armazenamento em pilha (LIFO)
- Penalidade ao depositar item na lixeira errada
- Penalidade ao deixar item cair no fim da esteira
- Triturador que processa lixo não reciclável em lote e gera Cubo de Sucata
- GameManager com cronômetro regressivo, saldo de dinheiro e sistema de vidas
- UI de HUD: dinheiro atual / meta, timer no formato MM:SS

### Em desenvolvimento / Incompleto
- Tela de game over e tela de vitória (condições checadas, sem transição de cena)
- Ganho de dinheiro ao vender Cubo de Sucata pelo Triturador (não conectado)
- Sistema de Crafting (Bancada)
- Máquina de Vendas
- Escalonamento dinâmico de dificuldade
- Guia/Livro de consulta in-game
- Itens das categorias Vidro e Orgânico (ScriptableObjects não criados)
- Animações do jogador (suporte preparado, inativo)
- Indicador visual de vidas na HUD

---

## Mecânicas Implementadas

### Esteira (`EsteiraSpawner.cs` / `MoverNaEsteira.cs`)
Um ponto de spawn no início da esteira instancia itens aleatórios em intervalo configurável. O movimento dos itens agora é gerenciado pela própria esteira (`MoverNaEsteira.cs`) através de um gatilho (Trigger), empurrando qualquer objeto com a tag "Item" que esteja sobre ela. Você pode escolher a direção (Direita, Esquerda, Cima ou Baixo) diretamente no Inspector. Se chegar ao fim da esteira sem ser recolhido, é destruído e o jogador perde uma vida.

### Jogador (`MovePlayer.cs` / `PlayerInteract.cs`)
Movimento 2D via novo Input System com velocidade padrão de 7. O sprite é espelhado conforme a direção do movimento.

**Interação (tecla `E`):**
- Mão vazia → verifica raio de 1,5u ao redor:
  1. Tenta retirar item da Lixeira mais próxima
  2. Tenta pegar item solto na esteira
- Segurando item → verifica raio de 1,5u ao redor:
  1. Tenta depositar na Lixeira mais próxima
  2. Tenta alimentar o Triturador
  3. Solta o item no chão

### Lixeiras (`Lixeira.cs`)
Cada lixeira aceita apenas uma categoria. O armazenamento é uma pilha LIFO com capacidade máxima de 20 itens. Um TextMeshPro flutuante exibe `atual/máximo` em tempo real.

**Ao depositar item incorreto:** chama `GameManager.AplicarPenalidade()`.

| Lixeira | Cor | Categoria |
|---|---|---|
| Papel | Azul | `CategoriaItem.Papel` |
| Plástico | Vermelho | `CategoriaItem.Plastico` |
| Metal | Amarelo | `CategoriaItem.Metal` |
| Vidro | Verde | `CategoriaItem.Vidro` |
| Orgânico | Marrom | `CategoriaItem.Organico` |

### Triturador (`Triturador.cs`)
Aceita apenas itens não recicláveis. Processa em lote de **10 itens**, com **2 segundos por item**. Ao completar o lote:
- Gera um **Cubo de Sucata** no ponto de drop
- Dispara animação, partículas e áudio durante o processamento
- Exibe contagem regressiva em tela

### GameManager (`GameManager.cs`)
Singleton. Controla o estado global da partida:

| Variável | Valor padrão |
|---|---|
| Dinheiro inicial | R$ 0 |
| Meta financeira | R$ 500 |
| Tempo da rodada | 105 segundos (1:45) |
| Vidas | 3 |

- **`AdicionarDinheiro(int valor)`** — soma ao saldo e verifica meta
- **`AplicarPenalidade()`** — decrementa vidas; `vidas <= 0` loga "Game Over"
- **`AtualizarUITempo()`** — formata `float` segundos para `MM:SS`

---

## Itens Definidos (ScriptableObjects)

| Item | Categoria | Valor |
|---|---|---|
| Folha Amassada | Papel | R$ 1 |
| Caixa de Papelão | Papel | R$ 2 |
| Garrafa PET | Plástico | R$ 2 |
| Pote de Iogurte | Plástico | R$ 1 |
| Lata de Refrigerante | Metal | R$ 3 |
| Fralda Suja | Não Reciclável | R$ 0 |
| Esponja Velha | Não Reciclável | R$ 0 |
| Cubo de Sucata *(output do Triturador)* | Não Reciclável | R$ 5 |

> Itens pendentes de criação: Pedaço de Fio de Cobre, Garrafa de Cerveja, Pote de Geleia, Casca de Banana.

---

## Itens Planejados (Não Implementados)

### Tabela completa da Esteira

| Item | Lixeira | Valor direto |
|---|---|---|
| Folha Amassada | Papel (Azul) | R$ 1 |
| Caixa de Papelão | Papel (Azul) | R$ 2 |
| Garrafa PET | Plástico (Vermelho) | R$ 2 |
| Pote de Iogurte | Plástico (Vermelho) | R$ 1 |
| Lata de Refrigerante | Metal (Amarelo) | R$ 3 |
| Pedaço de Fio de Cobre | Metal (Amarelo) | R$ 5 |
| Garrafa de Cerveja | Vidro (Verde) | R$ 3 |
| Pote de Geleia Vazio | Vidro (Verde) | R$ 2 |
| Casca de Banana | Orgânico (Marrom) | R$ 1 |
| Fralda Suja | Fornalha | R$ 0 |
| Esponja Velha | Fornalha | R$ 0 |

### Receitas de Crafting (Bancada)

| Produto | Receita | Venda |
|---|---|---|
| Caderno Reciclado | 3x Folha Amassada + 1x Caixa de Papelão | R$ 15 |
| Camiseta Ecológica | 2x Garrafa PET + 1x Caixa de Papelão | R$ 25 |
| Panela de Alumínio | 4x Lata de Refrigerante | R$ 35 |
| Vaso de Vidro Trabalhado | 3x Garrafa de Cerveja + 1x Pote de Geleia | R$ 40 |
| Fio de Cobre Tratado | 2x Fio de Cobre + 1x Garrafa PET | R$ 45 |
| Adubo Premium | 3x Casca de Banana | R$ 20 |
| Drone de Sucata *(Raro)* | 2x Lata + 2x Fio de Cobre + 1x Garrafa PET | R$ 100 |

---

## Estrutura de Scripts

```
Assets/Scripts/
├── ItemData.cs          — ScriptableObject: nome, categoria, ícone, valor
├── ItemColetavel.cs     — Componente do item em cena; aplica sprite do ItemData
├── MovePlayer.cs        — Movimento 2D do jogador (novo Input System)
├── PlayerInteract.cs    — Inventário (1 item) e lógica de pegar/soltar/depositar
├── Lixeira.cs           — Bin de triagem com pilha LIFO e validação de categoria
├── MoverNaEsteira.cs    — Gerencia o movimento dos itens sobre a esteira via Trigger
├── EsteiraSpawner.cs    — Instancia itens aleatórios no ponto de spawn por timer
├── FimDaEsteira.cs      — Trigger no fim da esteira; destrói item e aplica penalidade
├── Triturador.cs        — Processador em lote (10 itens); gera Cubo de Sucata
└── GameManager.cs       — Singleton: dinheiro, timer, vidas, HUD
```

---

## Prefabs

| Prefab | Uso |
|---|---|
| `item.prefab` | Template genérico instanciado pela esteira; dados injetados via `ConfigurarItem()` |
| `lixo_prefab.prefab` | Lixeira com BoxCollider2D e TextMeshPro de quantidade |
| `Esteira.prefab` | Conjunto completo: ponto de spawn, sprite visual e trigger de fim de esteira |

---

## Tecnologias

- **Unity** com **Universal Render Pipeline (URP)**
- **C#** — lógica de jogo
- **Unity Input System** (novo) — entrada do jogador
- **TextMesh Pro** — UI e textos flutuantes
- **ScriptableObjects** — dados dos itens desacoplados das cenas
