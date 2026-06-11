# ♻️ Recycle Rush - Documento de Design e Projeto (GDD)

**Recycle Rush** é um simulador de triagem e reciclagem frenético com **estilo visual 2.5D**. Você assume o papel de um operário em uma usina de processamento de resíduos e sua missão é transformar o lixo da cidade em lucro, batendo metas financeiras agressivas antes que o turno acabe. O jogo combina a simplicidade e precisão dos controles 2D com a profundidade visual e perspectiva de um ambiente tridimensional.

> **Estado atual:** Em desenvolvimento ativo na Unity (URP) utilizando o novo Input System. A lógica de movimentação e física roda em 2D (eixos X e Y), enquanto os sprites e a câmera proporcionam a ilusão de profundidade (2.5D).

---

## 🕹️ Visão Geral do Gameplay (Core Loops)

### 1. Triagem Ágil e Pressão de Tempo
O coração do jogo é a separação. Itens variados chegam sem parar pela esteira. O jogador deve gerenciar seu tempo (turnos cronometrados) correndo entre a esteira e os pontos de descarte. Se o tempo acabar e a meta financeira não for atingida, é Game Over.

### 2. Dificuldade Progressiva
Para manter a tensão, o jogo apresenta dificuldade dinâmica. Conforme o turno avança, a velocidade da esteira aumenta, exigindo reflexos afiados e priorização rápida. Itens que caem da esteira sem serem recolhidos geram perda de vidas.

### 3. Ciclo do Lixo ao Luxo (Triturador e Crafting)
Nem tudo vai para a lixeira.
*   **Triturador:** Lixos não recicláveis (ex: fraldas) são colocados aqui. Após processar um lote (ex: 10 itens), gera um "Cubo de Sucata" de alto valor.
*   **Bancada de Crafting (Em Breve):** O jogador pegará itens recicláveis simples (garrafas, papelão) e os combinará em receitas complexas (ex: Drone de Sucata) para maximizar os lucros e bater as metas.

### 4. Sistema de Penalidades
A precisão é fundamental. Colocar um item na lixeira errada resulta em perda de dinheiro imediata, afetando a economia do turno. Deixar itens caírem da esteira resulta em perda de vidas.

---

## ⚙️ Sistemas Técnicos e Mecânicas Implementadas

### O Jogador (`MovePlayer.cs` e `PlayerInteract.cs`)
*   **Movimento:** Implementado com o novo Unity Input System via Rigidbody2D.
*   **Sprint:** O jogador possui uma velocidade base (7) e uma velocidade de corrida (12) ativada ao segurar `Shift`.
*   **Interação (Tecla E):** Utiliza `Physics2D.OverlapCircleAll` com raio de 1.5u.
    *   *Mão vazia:* Prioriza objetos fixos (ex: puxar alavanca, retirar item do topo de uma lixeira) e, secundariamente, pega itens do chão/esteira.
    *   *Segurando item (Inventário de 1 slot):* Tenta depositar na lixeira, alimentar o triturador, usar a alavanca (sem largar o item) ou, em último caso, solta o item no chão instanciando um prefab genérico.

### Esteira e Logística (`EsteiraSpawner.cs`, `MoverNaEsteira.cs` e `AlavancaEsteira.cs`)
*   **Spawner:** Instancia um "prefab casca" e injeta dados de um `ScriptableObject` aleatório a cada *X* segundos (controlado pelo GameManager).
*   **Movimento Físico:** A esteira empurra qualquer objeto com a tag "Item" via `OnTriggerStay2D`. Suporta direções configuráveis (Direita, Esquerda, Cima, Baixo).
*   **Alavanca:** Um objeto interativo que permite ao jogador pausar/retomar toda a lógica da esteira (spawn e movimento) a qualquer momento.

### Sistema de Lixeiras (`Lixeira.cs`)
*   **Estrutura de Dados:** Utiliza uma Pilha (`Stack<ItemData>`) operando em formato LIFO (Last In, First Out).
*   **Validação:** Cada lixeira aceita apenas um `CategoriaItem` específico (Papel, Plástico, Metal, Vidro, Orgânico).
*   **UI Dinâmica:** Um `TextMeshPro` anexado ao prefab exibe a capacidade em tempo real (ex: `5/20`).
*   **Penalidade:** O depósito de uma categoria incorreta aciona a penalidade via GameManager.

### Triturador (`Triturador.cs`)
*   **Processamento em Lote:** Mantém uma contagem interna. Exige exatos 10 itens não recicláveis.
*   **Timer e Feedback:** Cada item leva 2 segundos para ser processado. Utiliza corotinas (`IEnumerator`) para simular o tempo, emitindo partículas e atualizando um texto de progresso na tela.
*   **Recompensa:** Ao fim do ciclo, instancia um Cubo de Sucata no ponto de drop especificado.

### Gestão Global (`GameManager.cs`)
*   **Padrão Singleton:** Acesso global facilitado via `GameManager.Instance`.
*   **Economia e Tempo:** Controla o dinheiro atual, a meta do turno e formata o tempo flutuante para a UI (MM:SS).
*   **Configurações Globais:** Centraliza a velocidade da esteira, intervalo de spawn e escala visual dos itens gerados, permitindo balanceamento rápido.

---

## 📦 Arquitetura de Dados (ScriptableObjects)

A base de dados do jogo utiliza `ScriptableObjects` (`ItemData.cs`), permitindo a criação de centenas de itens sem alterar código. Cada item guarda: `nome`, `categoria`, `icone` e `valor monetário`.

### Itens Implementados

| Item | Categoria | Valor (R$) |
| :--- | :--- | :--- |
| Folha Amassada | Papel (Azul) | 1 |
| Caixa de Papelão | Papel (Azul) | 2 |
| Pote de Iogurte | Plástico (Vermelho) | 1 |
| Garrafa PET | Plástico (Vermelho) | 2 |
| Lata de Refrigerante | Metal (Amarelo) | 3 |
| Fralda Suja | Não Reciclável (Triturador) | 0 |
| Esponja Velha | Não Reciclável (Triturador) | 0 |
| Cubo de Sucata | Não Reciclável (Venda Direta) | 5 |

### Itens Planejados (Para Crafting)

| Item Base / Produto | Lixeira / Bancada | Valor Estimado (R$) |
| :--- | :--- | :--- |
| Pedaço de Fio de Cobre | Metal (Amarelo) | 5 |
| Garrafa de Cerveja | Vidro (Verde) | 3 |
| Casca de Banana | Orgânico (Marrom) | 1 |
| **Caderno Reciclado** | *Crafting (Papel + Papel)* | 15 |
| **Drone de Sucata** | *Crafting (Metal + PET + Fio)* | 100 |

---

## 🏗️ Estrutura do Projeto (Unity)

### Prefabs Principais
*   `item.prefab`: O esqueleto vazio. O Spawner ou o Jogador o instanciam no mundo e injetam um `ItemData` nele para definir sua aparência e comportamento.
*   `lixo_prefab.prefab`: A estrutura base das Lixeiras, contendo colisores e os textos flutuantes.
*   `Esteira.prefab`: O conjunto completo de automação logística (Spawner + Visual + Triggers de movimento e destruição).

### Hierarquia de Scripts
```text
Assets/Scripts/
├── Dados/
│   └── ItemData.cs          (ScriptableObject de definição)
├── Entidades/
│   ├── ItemColetavel.cs     (Renderização e dados em cena)
│   ├── MovePlayer.cs        (Física e Input)
│   └── PlayerInteract.cs    (Lógica de Interação e Inventário)
├── Mecanicas/
│   ├── AlavancaEsteira.cs   (Controle lógico da esteira)
│   ├── EsteiraSpawner.cs    (Gerador dinâmico)
│   ├── FimDaEsteira.cs      (Trigger de penalidade de vida)
│   ├── Lixeira.cs           (Armazenamento LIFO)
│   ├── MoverNaEsteira.cs    (Física de translação da esteira)
│   └── Triturador.cs        (Máquina de processamento em lote)
├── Core/
│   └── GameManager.cs       (Singleton de estados e UI)
└── Visuais/
    └── DestaqueObjeto.cs    (Feedback de proximidade)
```

---

## 🚀 Próximos Passos (Roadmap)

1.  **Módulo de Crafting:** Implementar a `Bancada.cs` onde o jogador deposita itens específicos para gerar produtos de alto valor.
2.  **Sistema de Vendas:** Criar o "Caminhão de Coleta" ou "Máquina de Vendas" para o jogador despachar os produtos e receber o dinheiro.
3.  **Transições de Jogo:** Criar as telas e lógicas de fim de turno (Vitória/Bateu Meta vs. Derrota/Falência).
4.  **Escalonamento Dinâmico:** Implementar a lógica no `GameManager` que aumenta gradativamente o `intervaloSpawnGlobal` e a `velocidadeGlobalEsteira` com base no tempo restante.
5.  **Animações:** Ligar o Animator do personagem para ações de andar, correr e carregar itens.