// Libs
import jQuery from 'jquery'
import $ from 'jquery'

// Temp
import 'datatables.net-bs5/css/dataTables.bootstrap5.css'
import 'datatables.net-responsive-bs5/css/responsive.bootstrap5.css'
import 'datatables.net-bs5'
import 'datatables.net-responsive-bs5'

// Css libs
import 'bootstrap/dist/css/bootstrap.min.css'
import 'bootstrap'

// Core
import { AuthService, ApiService } from '../../js/core/index.js'
import '../../js/main.js'

// Services
import { EmailCaixaDeSaidaService, EmailStorageManagerService } from '../../js/services/index.js'

import { gerarRelatorio, reenviarEmails } from '../../js/modules/emails-enviados'

import DateFormater from '../../js/utils/date/date-formatter.js'
import DataTableWrapper from '../../js/utils/datatables/datatable-wrapper.js'
import { Menu } from '../../components/menu/menu.js'

// Services
const apiService = new ApiService()
const emailStorageManagerService = new EmailStorageManagerService(apiService)
const emailCaixaDeSaidaService = new EmailCaixaDeSaidaService(apiService)

let emailsEnviadosTb = null
let emailsCaixaDeSaidaTb = null

jQuery(async () => {
  new Menu()

  await AuthService.initialize()

  await carregarDatatableEnviados()
  await carregarDatatableCaixaDeSaida()

  $("#reenviarEmailsBtn").on('click', async () => await reenviarEmails())
  $("#gerarRelatorioBtn").on('click', async () => await gerarRelatorio())
})

async function carregarDatatableEnviados() {
  emailsEnviadosTb = new DataTableWrapper('#emailsEnviadosTb', {
    columns: [
      { "data": "from", title: "De" },
      { "data": "to", title: "Para" },
      { "data": "assunto", title: "Assunto" },
      { "data": "enviado", title: "E-mail recebido" },
      { "data": "dataInclusao", title: "Data do envio", render: data => DateFormater.dateTime(data) }
    ],
    ajax: async (requestData) => {
      requestData.Columns = 'from,to,assunto,enviado'
      return await emailStorageManagerService.obterDatatable(requestData)
    }
  })

  await emailsEnviadosTb.initialize()
}

async function carregarDatatableCaixaDeSaida() {
  emailsCaixaDeSaidaTb = new DataTableWrapper('#emailsCaixaDeSaidaTb', {
    columns: [
      { "data": "email", title: "Email" },
      { "data": "assunto", title: "Assunto" },
      { "data": "dataInclusao", title: "Data do envio", render: data => DateFormater.dayMonthYear(data) }
    ],
    ajax: async (requestData) => {
      return await emailCaixaDeSaidaService.obterDatatable(requestData)
    }
  })

  await emailsCaixaDeSaidaTb.initialize()
}
