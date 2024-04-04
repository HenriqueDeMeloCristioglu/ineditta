import { renderizarModal } from "../../../../utils/modals/modal-wrapper"
import {
  handleClickAdicionarClausulaCliente,
  handleCloseClausulaClienteModal,
  handleOpenClausulaClienteModal
} from "../../features"

export class ModalClausulaCliente {
  constructor({ pageContainer }) {
    this.pageContainer = pageContainer
  }

  render(context) {
    const clausulaClienteModalHidden = document.getElementById("clausulaClienteModalHidden")
    const clausulaClienteModalContent = document.getElementById("clausulaClienteModalContent")

    renderizarModal(this.pageContainer, [
      {
        id: "clausulaClienteModal",
        modal_hidden: clausulaClienteModalHidden,
        content: clausulaClienteModalContent,
        btnsConfigs: [
          {
            id: "btn_add_clausula_cliente",
            onClick: async (_, modalContainer) => await handleClickAdicionarClausulaCliente({ context, modalContainer })
          }
        ],
        onOpen: async () => handleOpenClausulaClienteModal(context),
        onClose: () => handleCloseClausulaClienteModal(context),
        isInIndex: true
      }
    ])
  }
}
