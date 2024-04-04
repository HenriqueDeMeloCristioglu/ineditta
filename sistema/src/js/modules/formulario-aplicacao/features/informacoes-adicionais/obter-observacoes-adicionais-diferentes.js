export function obterObservacoesAdicionaisDiferentes(informacoesAdicionais) {
  const observacoes = []

  for (const informacaoAdicional of informacoesAdicionais) {
    if (!informacaoAdicional.observacoesAdicionais) {
      continue
    }

    for (const observacaoAdicional of informacaoAdicional.observacoesAdicionais) {
      const { clausulaId, element, dado, tipo } = observacaoAdicional
      const data = element.val()

      if (dado != data) {
        observacoes.push({
          id: clausulaId,
          valor: data,
          tipo
        })
      }
    }
  }

  return observacoes
}