// Libs
import JQuery from 'jquery'
import $ from 'jquery'

import 'bootstrap/dist/css/bootstrap.min.css'
import 'bootstrap'

// Core
import { AuthService } from '../../js/core/index.js'

import '../../js/main.js'
import { Menu } from '../../components/menu/menu.js'

import {
  documentosDataTable,
  configurarFormularioModalListaClausula,
  configurarFormularioModalEditarClausula,
  iaDocumentoSindicalContext,
  configurarModal,
  configurarFormularioFiltros
} from '../../js/modules/scrap'
import { useUsuarioService } from '../../js/modules/index.js'

const usuarioAdmSerivce = useUsuarioService()

JQuery(async function () {
  new Menu()

  await AuthService.initialize()

  const permissoesUsuario = (await usuarioAdmSerivce.obterPermissoes()).value

  if (permissoesUsuario instanceof Array) {
    const { datas } = iaDocumentoSindicalContext

    const [permissoesDocumentoSindicalIa] = permissoesUsuario.filter(permissao => permissao.modulo_id == '70')
    datas.permissoesDocumentoSindicalIa = permissoesDocumentoSindicalIa

    if (datas.permissoesDocumentoSindicalIa && datas.permissoesDocumentoSindicalIa?.criar != '1') {
      $("#clausulaBtn")
        .addClass('disabled')
        .attr('disabled', true)
        .removeAttr('id')
        .removeAttr('data-target')
        .removeAttr('data-toggle')
    }

    if (datas.permissoesDocumentoSindicalIa && datas.permissoesDocumentoSindicalIa?.aprovar != '1') {
      $("#aprovar_documento_btn")
        .addClass('disabled')
        .attr('disabled', true)
    }
  }

  configurarModal(iaDocumentoSindicalContext)
  configurarFormularioModalEditarClausula(iaDocumentoSindicalContext)
  configurarFormularioModalListaClausula(iaDocumentoSindicalContext)
  configurarFormularioFiltros(iaDocumentoSindicalContext)
  await documentosDataTable(iaDocumentoSindicalContext)
})


