import $ from 'jquery'

import modalTemplate from './index.html'

export default class ModalContainer {
  modalContainer
  id

  constructor(id) {
    this.modalContainer = this.stringToHTML(modalTemplate)
    this.id = id

    this.setNewId(id)
  }

  setNewId(id) {
    return this.modalContainer.id = id
  }

  injectContent(content, styles) {
    const contentModalId = `${this.modalContainer.id}-content`
    const closeButton = createCloseButton(this.id)


    if (styles) {
      const { modal, container } = styles

      if (container) {
        Object.keys(container).map(style => {
          this.modalContainer.style[style] = container[style]
        })
      }

      if (modal) {
        Object.keys(modal).map(style => {
          this.modalContainer.querySelector("#my-modal-dialog").style[style] = modal[style]
        })
      }
    }

    this.modalContainer.querySelector('#myModalContent').id = contentModalId
    this.modalContainer.querySelector(`#${contentModalId}`).innerHTML = content.innerHTML
    this.modalContainer.querySelector(`#${contentModalId}`).append(closeButton)

    return this.modalContainer
  }

  stringToHTML(str, selector = 'div') {
    const parser = new DOMParser();
    const { body } = parser.parseFromString(str, 'text/html');
    const element = body.querySelector(selector)

    return element;
  }
}

function createCloseButton(modalId) {
  return (
    $('<button>', {
      class: 'close hidden',
      id: `${modalId}_close_btn`
    })
      .attr('data-dismiss', 'modal')
  )[0]
}