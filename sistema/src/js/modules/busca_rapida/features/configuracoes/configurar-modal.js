import { ModalClausula, ModalClausulaCliente, ModalInformacoesSindicatos } from "../../components"

export function configurarModal(context) {
  const pageCtn = document.getElementById("pageCtn")
  
  const modalClausulaCliente = new ModalClausulaCliente({ pageContainer: pageCtn })
  modalClausulaCliente.render(context)
  
  const modalInformacoesSindicatos = new ModalInformacoesSindicatos({ pageContainer: pageCtn })
  modalInformacoesSindicatos.render(context)
  
  const modalClausula = new ModalClausula({ pageContainer: pageCtn })
  modalClausula.render(context)
}
