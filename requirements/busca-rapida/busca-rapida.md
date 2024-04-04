# Gerar Resumo IA:

- Front-end:

  - Ao abrir o modal do documento:
    - Habilitar o botão de `Gerar Resumo Cláusulas` se: ✔✔
      - O documento possuir alguma empresa que tenha acesso ao o módulo(modulo_cliente) `Gerar Resumo Cláusulas` ✔✔
      - O documento possuir alguma cláusulas com o campo `resumir` igual a verdadeiro ✔✔
    - Apresentar na grid somente as cláusulas que tem o campo `consta_no_documento` como verdadeiro ✔✔
    - Incluir no Grid o campo `Resumível` ✔✔
    - Incluir no Grid o campo `Ver Resumo` ✔✔
    - Ao clicar no botão `Gerar Resumo Cláusulas`: ✔✔
      - Disparar a ação de gerar os resumos das cláusulas ✔✔
    - Ao clicar no botão `Ver Resumo`: ✔✔
      - Abri modal Resumo: ✔✔
        - Botão Salvar: ✔✔
          - Salvar alterações do resumo clicando no botão `Salvar` ✔✔
          - Ao salvar, fechar o modal e voltar para o de cláusulas ✔✔
        - Mostrar: ✔✔
          - Apresentará o ID do Documento ✔✔
          - Apresentará o Nome da cláusula ✔✔
          - Apresentará a descrição do Sinônimo ✔✔
          - Apresentará o Número da cláusula ✔✔
          - Apresentará a Vigência inicial ✔✔
          - Apresentará a Vigência Final ✔✔
          - Apresentará o texto original da clausula em uma área de texto a esquerda ✔✔
          - Apresentará o texto da clausula resumida pela IA em uma área de texto a direita ✔✔
            - O campo estará desabilitado e só habilitar ao clicar no botão `Editar Resumo` ✔✔
  - Módulo Resumo Cláusulas:

    - O menu deve aparecer depois de `formulário aplicação` dentro de `Mapa Sindical` ✔✔
    - Fazer filtros:
      - Grupo Econômico ✔✔
      - Empresa ✔✔
      - Estabeleciento ✔✔
      - Localidade do Estabelecimento ✔✔
        - Puxar localizações de acordo com os estabelecimentos do usuário ✔✔
      - Atividade Econômica ✔✔
      - Sindicato Laboral ✔✔
      - Sindicato Patronal ✔✔
      - Data-base/Ano
      - Nome Documento ✔✔
      - Grupo Cláusulas ✔✔
      - Seleção de Cláusulas: ✔✔
        - Puxar cláusulas que não constam no documento ✔✔
        - Puxar somente cláusulas que tem o campo `resumir` como sim ✔✔
      - Data Processamento (Da cláusula) ✔✔
    - Carregar na Grid:
      - Regras: ✔✔
        - Ser Cláusula Resumível ✔✔
        - Trazer Cláusulas que não constam no documento ✔✔
        - Trazer Cláusulas que tem a `data_processamento_documento` ✔✔
        - Não repetir clausulas com o mesma estrutura e resumo ✔✔
      - Campos: ✔✔
        - Nome ✔✔
        - Documento ✔✔
        - Sindicato Laboral ✔✔
        - Sindicato Patronal ✔✔
        - Texto Resumido ✔✔
        - Data-base ✔✔
        - Validade Final ✔✔
        - Data Processamento ✔✔
    - Modal de Cláusulas:
      - Adicionar ao cabeçalho das cláusulas: ✔✔
        - Data assinatura/registro ✔✔
      - Adicionar Botão `Adicionar regra empresa`: ✔✔
        - Regras: ✔✔
          - Só pode ver quem tem acesso a comentar ✔✔
        - Ao clicar, abrir modal clausulas-cliente: ✔✔
          - Puxar clausula selecionada ✔✔
          - Puxar clusula do cliente por Grupo Econômico se houver ✔✔
          - Abrir modal Cláusula Cliente ✔✔
        - Adicionar Painel Clausula Cliente: ✔✔
          - Mostrar `texto` do cliente ✔✔
      - Modal Cláusula Cliente - mostrar: ✔✔
        - Nome da Cláusula ✔✔
        - Texto Resumido do cliente ✔✔
        - Botão de `Salvar`: ✔✔
          - Ao clicar, Criar ou Atualizar `texto` da clausula_cliente selecionada ✔✔
          - Após salvar, fechar modal ✔✔
    - Gerar Pdf:
      - Mostrar: ✔✔
        - No lugar do texto: Resumo da cláusula ✔✔
        - No lugar dos comentários: Regra empresa ✔✔
      - Regra: ✔✔
        - Agrupadas por documento ✔✔
        - Ordenadar por A-Z (UF/Município) ✔✔
        - Ordenadar por A-Z (Nome Cláusula) ✔✔
        - Filtra por Período ✔✔
    - Gerar Relatório excel: ✔✔
      - Seguir template ✔✔

- Back-end:

  - Gerar Resumo Cláusulas: ✔✔

    - Regras:
      - Receber o ID ✔✔
      - Puxar o Documento selecionado pelo ID ✔✔
      - Puxar o Documento se houver alguma empresa que tenha acesso ao o módulo(modulo_cliente) `Gerar Resumo Cláusulas` ✔✔
      - Puxar o Documento se houver alguma cláusula com o campo `resumir` igual a verdadeiro ✔✔
      - Ordenar por `estrutura_clausula` ✔✔
      - Agrupar os texto das cláusulas que são da mesma estrutura e resumir elas ✔✔
      - Salvar cada resumo em sua respectiva cláusulas ✔✔
      - Abrir novas cláusulas como não constam ✔✔

  - Abrir novas cláusulas como não constam ✔✔

    - Receber ID ✔✔
    - Puxar todas as estruturas a que não existem no documento ✔✔
    - Adicionar o campo `consta_no_documento` na Clausula ✔✔
    - Gerar novas cláusulas ✔✔

  - Liberar Documento ✔✔

    - Adicionar campo `data_processamento_documento` ✔✔
    - Preencher campo `data_processamento_documento` com a data atual ✔✔

  - Atualizar Resumo Cláusula ✔✔

    - Obter todos as cláusulas do documento de acordo com as `estruturas_id`
    - Editar campo `texto_resumido` ✔✔

  - Restringir no sistema para trazer somente cláusulas que constam no documento ✔✔
