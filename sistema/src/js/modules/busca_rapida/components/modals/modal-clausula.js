import { renderizarModal } from "../../../../utils/modals/modal-wrapper"
import { handleClickGerarPdf } from "../../features"
import { handleCloseClausulaModal, handleOpenClausulaModal } from "../../features/handlers/modals/clausula"

export class ModalClausula {
  constructor({ pageContainer }) {
    this.pageContainer = pageContainer
  }

  render(context) {
    const modal_hidden = document.getElementById("clausulaModalHidden")
    const content = document.getElementById("clausulaModalHiddenContent")

    renderizarModal(this.pageContainer, [
      {
        id: "clausulaModal",
        modal_hidden,
        content,
        btnsConfigs: [
          {
            id: "gerarPDF",
            onClick: async () => await handleClickGerarPdf(context),
            data: null,
          },
        ],
        onOpen: async () => await handleOpenClausulaModal(context),
        onClose: () => handleCloseClausulaModal(context),
        styles: {
          container: {
            paddingRight: "30px",
            paddingLeft: "30px",
          },
          modal: {
            maxWidth: "1800px",
            width: "100%",
          },
        },
      }
    ])
  }
}

