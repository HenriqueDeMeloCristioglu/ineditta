import { th, thead, tr } from "../../../../utils/components/elements";

export function criarHeader({
  onClickOrdenarPorStatus,
  context
}) {
  const header = thead({
    content: tr({})
    .append(th({ content: 'Classificação cláusula' }))
    .append(th({ content: 'Sinônimo' }))
    .append(th({ content: 'N da cláusula' }))
    .append(
      th({ content: 'Status', style: 'cursor: pointer;' })
      .on('click', () => onClickOrdenarPorStatus(context))
    )
  })

  return header
}