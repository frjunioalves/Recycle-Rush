# ♻️ Recycle Rush

Jogo 2D de triagem, crafting e economia baseado em reciclagem. Separe o lixo na esteira, descarte os não recicláveis no triturador, e bata a meta financeira antes que o tempo acabe.

---

## Estado do Projeto

> Desenvolvido em Unity 2D com URP e o novo Input System.

### Implementado
- Esteira com spawn contínuo e movimento automático dos itens
- **Alavanca de Controle**: Permite ligar/desligar a esteira a qualquer momento
- Jogador com movimento livre, **sistema de corrida (Shift)** e inventário de mão única
- Lixeiras categorizadas com armazenamento em pilha (LIFO)
- Penalidade ao depositar item na lixeira errada
- Penalidade ao deixar item cair no fim da esteira
- Triturador que processa lixo não reciclável em lote e gera Cubo de Sucata
- GameManager com cronômetro regressivo, saldo de dinheiro, vidas e **estado global da esteira**
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

### Esteira (`EsteiraSpawner.cs` / `MoverNaEsteira.cs` / `AlavancaEsteira.cs`)
Um ponto de spawn no início da esteira instancia itens aleatórios em intervalo configurável. O movimento dos itens é gerenciado pela própria esteira (`MoverNaEsteira.cs`) através de um gatilho (Trigger).
- **Alavanca**: O jogador pode interagir com uma alavanca (tecla `E`) para ligar ou desligar o movimento e o spawn da esteira globalmente.
- **Direções**: Suporte a movimento para Direita, Esquerda, Cima ou Baixo.
- **Destaque**: Itens e objetos interativos possuem feedback visual de destaque.

### Jogador (`MovePlayer.cs` / `PlayerInteract.cs`)
Movimento 2D via novo Input System com velocidade configurável.
- **Velocidade**: 7 (Normal) / 12 (Correndo com `Shift`).
- **Interação (tecla `E`)**:
  - Mão vazia → Retira item da Lixeira, usa Alavanca ou pega item do chão.
  - Segurando item → Deposita na Lixeira, alimenta Triturador, usa Alavanca ou solta no chão.

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
Singleton. Controla o estado global da partida e configurações globais:

| Variável | Valor padrão |
|---|---|
| Meta financeira | R$ 500 |
| Tempo da rodada | 105 segundos |
| Vidas | 3 |
| **Velocidade Esteira** | 2.0 |
| **Intervalo Spawn** | 3.0 |
| **Escala Global Itens** | 1.0 |

- **`esteiraAtiva`**: Boolean que trava/destrava todo o sistema de logística.
- **`AdicionarDinheiro(int valor)`** — soma ao saldo e verifica meta.
- **`AplicarPenalidade()`** — decrementa vidas; `vidas <= 0` loga "Game Over".

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
├── ItemColetavel.cs     — Componente do item; aplica sprite e escala global
├── MovePlayer.cs        — Movimento 2D e sistema de Sprint (Shift)
├── PlayerInteract.cs    — Lógica de interação universal (E)
├── AlavancaEsteira.cs   — Controle visual e lógico da esteira
├── DestaqueObjeto.cs    — Feedback visual ao chegar perto de objetos
├── Lixeira.cs           — Bin de triagem com pilha LIFO
├── MoverNaEsteira.cs    — Movimento físico sobre a esteira via Trigger
├── EsteiraSpawner.cs    — Instancia itens respeitando estado global
├── FimDaEsteira.cs      — Trigger de perda de item
├── Triturador.cs        — Processador de sucata
└── GameManager.cs       — Centralizador de regras, economia e estados
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
