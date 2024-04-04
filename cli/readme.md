# Ineditta - CLI

## Pré Reqisitos

- Node.Js

## Instalação

- Entre na pasta `cli`
- Rode o comando `npm i`

## O que posso fazer?

- No Front-end
  - Criar arquivos de ações de crud dentro da pasta `modules`
  - Criar uma nova `service` com as operações de incluir, editar, obter por id e obter data table
- No Back-end
  - Criar casos de uso em `Application`

## Como usar?

- No Front-end: rodar o comando `npm run generate:frontend --` mais a opção que desejar:
  - `-m ou --moudule` que é o nome do módulo que deseja gerar
  - `-cd ou --crud` que é uma flag para selecionar todas as opções de crud para ser gerado (opcional)
  - `-ic ou --incluir` que é uma flag para selecionar somente o incluir do crud para ser gerado (opcional)
  - `-at ou --atualizar` que é uma flag para selecionar somente o atualizar do crud para ser gerado (opcional)
  - `-ob ou --obterPorId` que é uma flag para selecionar somente o obter por id do crud para ser gerado (opcional)
  - `-sv ou --service` que é uma flag para criar uma service nova para ser gerada (opcional)
- No Back-end: rodar o comando `npm run generate:backend --` mais a opção que desejar:

  - `-m ou --moudule` que é o nome do módulo que deseja gerar
  - `-sm ou --submodule` que é o nome do sub módulo que deseja gerar (opcional)
  - `-ac ou --action` que é o nome da ação do caso de uso que deseja gerar
