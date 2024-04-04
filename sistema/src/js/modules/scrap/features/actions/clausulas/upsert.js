import { incluirClausula } from "./incluir-clausula"
import { atualizarClausula } from "./atualizar-clausula"

export async function upsert({
  id,
  texto,
  documentoSindicalId,
  estruturaClausulaId,
  numero,
  sinonimoId
}) {
  return id ? await atualizarClausula({
    id,
    texto,
    documentoSindicalId,
    estruturaClausulaId,
    numero,
    sinonimoId
  }) : await incluirClausula({
    texto,
    documentoSindicalId,
    estruturaClausulaId,
    numero,
    sinonimoId
  })
}