import { IAClausulaStatus } from "../../../../application/scrap/clausulas/ia-clausula-status";
import { div, tbody, td, tr } from "../../../../utils/components/elements";

export function criarBody({
  classificacaoClausula,
  sinonimo,
  numero,
  status
}) {
  const statusItem = div({
    className: `circle ${IAClausulaStatus.Consistente == status ? 'circle-green' : 'circle-danger'}`
  })

  const row = tr({})
                .append(td({ content: classificacaoClausula }))
                .append(td({ content: sinonimo }))
                .append(td({ content: numero }))
                .append(td({
                  content: statusItem,
                  style: 'display: flex; justify-content: center; align-items: center;',
                  className: 'status'
                }))

  return tbody({ content: row })
}