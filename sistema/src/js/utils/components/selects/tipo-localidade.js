export async function obterTipoLocalizacao() {
  return await new Promise((resolve) => {
    return resolve([
      {
        id: 'uf',
        description: 'Uf'
      },
      {
        id: 'regiao',
        description: 'Região'
      },
      {
        id: 'municipio',
        description: 'Municipio'
      }
    ])
  })
}