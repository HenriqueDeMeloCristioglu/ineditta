import $ from 'jquery'

import ModalContainer from "./script";

$.fn.modal.Constructor.prototype.enforceFocus = function () { };

export function renderizarModal(pageCtn, modalsConfig) {
  modalsConfig.map(({
    id,
    modal_hidden,
    content,
    btnsConfigs,
    onOpen,
    onClose,
    isInIndex,
    styles
  }) => {
    renderModal(pageCtn, modal_hidden, content, id, btnsConfigs, onOpen, onClose, isInIndex, styles)
  })
}

function renderModal(pageCtn, modal_hidden, content, modalId, btnsConfigs, onOpened, onClosed, isInIndex, styles) {
  const modalContainerElement = configurarModal(content, modalId, styles)
  pageCtn.appendChild(modalContainerElement)

  if (btnsConfigs.length > 0) {
    btnsConfigs.map(btnItem => {
      const button = modalContainerElement.querySelector(`#${btnItem.id}`)

      button.addEventListener('click', () => btnItem.onClick(btnItem.data ? btnItem.data : null, modalContainerElement))
    })
  }

  $(modalContainerElement).on('show.bs.modal', () => {
    if (onOpened) onOpened(modalContainerElement)
  })
  $(modalContainerElement).on('hidden.bs.modal', () => {
    if (isInIndex) $('body').addClass('modal-open')
    if (onClosed) onClosed(modalContainerElement)
  })

  modal_hidden.remove()
}

function configurarModal(content, modalId, styles) {
  const modalContainer = new ModalContainer(modalId)

  return modalContainer.injectContent(content, styles)
}

export function closeModal(modal) {
  $(`#${modal.id}_close_btn`).trigger('click')
}