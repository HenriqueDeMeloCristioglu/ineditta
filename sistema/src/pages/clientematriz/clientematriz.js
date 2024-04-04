import JQuery from 'jquery';
import $ from 'jquery';

import 'datatables.net-bs5/css/dataTables.bootstrap5.css';
import 'datatables.net-responsive-bs5/css/responsive.bootstrap5.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap';
import 'datatables.net-responsive-bs5';

import '../../js/utils/masks/jquery-mask-extensions';

import '../../js/main.js'
import { Menu } from '../../components/menu/menu.js'

import Masker from '../../js/utils/masks/masker.js';
import SelectWrapper from '../../js/utils/selects/select-wrapper.js';
import { closeModal, renderizarModal } from '../../js/utils/modals/modal-wrapper.js';
import DataTableWrapper from '../../js/utils/datatables/datatable-wrapper.js';
import DatepickerWrapper from '../../js/utils/datepicker/datepicker-wrapper.js';
import DateFormatter from '../../js/utils/date/date-formatter.js';

import NotificationService from '../../js/utils/notifications/notification.service';
import { GrupoEconomicoService, MatrizService, ModuloService, TipoDocService, UsuarioAdmService } from '../../js/services';
import { ApiService, AuthService } from '../../js/core/index.js';

const apiService = new ApiService();
const usuarioAdmService = new UsuarioAdmService(apiService);
const matrizService = new MatrizService(apiService);
const grupoEconomicoService = new GrupoEconomicoService(apiService);
const tipoDocumentoService = new TipoDocService(apiService);
const moduloService = new ModuloService(apiService);

let matrizesTb = null;
let modulosTb = null;

let clienteMatrizSelecionado = null;
let update = false;

let grupoEconomicoSelect = null;
let tipoDocumentoSelect = null;
let slaPrioridadeSelect = null;
let tipoProcessamentoSelect = null;

let dataInativacaoDatePicker = null;

let id_modulos = '';

const MODULO_CLIENTE_MATRIZ_ID = 17

JQuery(async function () {
  new Menu();

  await AuthService.initialize();

  const permissoesUsuario = (await usuarioAdmService.obterPermissoes()).value;

  const [permissoesModulo] = permissoesUsuario.filter(p => p.modulo_id == MODULO_CLIENTE_MATRIZ_ID);

  console.log(permissoesUsuario);
  console.log(permissoesModulo);

  await carregarPermissoesUsuario();

  await carregarDatatableMatrizes();

  configurarModal();

  await configurarFormularios();

  if (permissoesModulo.criar == 1) $("#btn_novo").show();
  else $("#btn_novo").hide();

  $("#btn_novo").on("click", () => {
    clienteMatrizSelecionado = null;
  });

  $('.horizontal-nav').removeClass('hide');

  $("#upsert-form").on('submit', async (e) => {
    e.preventDefault();
    await upsertClienteMatriz();
  });

  $("#btn-upsert").on('click', () => {
    $("#upsert-form").trigger('submit');
  });

  $("#btn-inativar-ativar").on('click', async () => {
    await inativarAtivarToggle(clienteMatrizSelecionado?.id ?? 0);
  });
})

async function upsertClienteMatriz(){
  const arquivoInput = document.querySelector('input[name="logotipoMatriz"]');
  const arquivo = arquivoInput.files[0];

  const params = {
    grupoEconomicoId: Number(grupoEconomicoSelect?.getValue()),
    nome: $("#nome-input").val(),
    codigo: $("#cod-input").val() ? $("#cod-input").val() : null,
    aberturaNegociacao: Number($("#an-input").val()),
    dataCorteForpag: Number($("#corte-input").val()),
    slaPrioridade: slaPrioridadeSelect?.getValue(),
    tiposDocumentos: tipoDocumentoSelect?.getValue(),
    tipoProcessamento: Number(tipoProcessamentoSelect?.getValue()),
    modulosIds: id_modulos.split(" ").filter(valor => !!valor).map(v => Number(v)),
    logo: arquivo
  }

  let resultUpsert;

  if (clienteMatrizSelecionado) {
    params.id = clienteMatrizSelecionado.id;
    resultUpsert = await matrizService.atualizar(params);
  } else {
    resultUpsert = await matrizService.inserir(params);
  }

  if (resultUpsert.isFailure()){
    NotificationService.error({title: "Não foi possível realizar o cadastro ou atualização do cliente matriz.", message: resultUpsert.error});
    return;
  }

  NotificationService.success({title: "Cadastro/atualização realizado(a) com sucesso"});

  if (!clienteMatrizSelecionado){
    limparFormularioUpsert();
    $("#novoClientMatrizModal_close_btn").trigger('click');
  }

  await carregarDatatableModulos();
  await carregarDatatableMatrizes();
}

async function inativarAtivarToggle(id){
  const result = await matrizService.inativarAtivarToggle(id);

  if (result.isFailure()) {
    NotificationService.error({title: 'Não foi possível realizar a inativação ou ativação do cliente matriz'});
    return;
  }

  NotificationService.success({title: 'Ativação/Inativação feita com sucesso.'});
  
  if(dataInativacaoDatePicker?.getValue()) {
    $("#btn-inativar-ativar").html("Inativar");
    dataInativacaoDatePicker?.clear();
  }
  else {
    $("#btn-inativar-ativar").html("Ativar");
    dataInativacaoDatePicker?.setValue(new Date());
  }
  
  await carregarDatatableModulos();
  await carregarDatatableMatrizes();
}

async function carregarPermissoesUsuario() {
  const result = await usuarioAdmService.obterPermissoes();

  if (result.isFailure()) {
    return NotificationService.error({ title: 'Erro ao obter permições do usuário', message: result.error })
  }

  const data = result.value
}

async function carregarDatatableMatrizes() {
  if (matrizesTb) {
    matrizesTb.reload();
    return;
  }

  matrizesTb = new DataTableWrapper('#matrizesTb', {
    columns: [
      { "data": "id", },
      { "data": "grupoEconomico", title: "Grupo Econômico" },
      { "data": "nome", title: "nome" },
      { "data": "dataInclusao", title: "Data de Inclusão", render: (data) =>  DateFormatter.dayMonthYear(data)},
      { "data": "dataInativacao", title: "Data de Inativação", render: (data) => data ? DateFormatter.dayMonthYear(data) : null }
    ],
    ajax: async (requestData) => {
      requestData.SortColumn = 'id';
      requestData.columns = 'id,grupoEconomico,nome,dataInclusao,dataInativacao';
      return await matrizService.obterDataTable(requestData);
    },

    rowCallback: function (row, data) {
      const icon = $("<i>").addClass("fa fa-file-text")

      let button = $("<a>")
        .attr("data-id", data?.id)
        .addClass("btn-update")
        .html(icon)

      button.on("click", async function () {
        const matriz = await matrizService.obterPorId(data?.id);
        if (matriz.isFailure()) {
          NotificationService({title: "Erro ao tentar obter o cliente matriz"});
          return;
        }

        clienteMatrizSelecionado = matriz.value;
        
        await preencherInformacoesParaAtualizacao();

        update = true
        $('#btn_open_novo_cliente_matriz_modal').trigger('click')
      })

      $("td:eq(0)", row).html(button)
    }
  });

  await matrizesTb.initialize();
}

async function preencherInformacoesParaAtualizacao() {
  if(clienteMatrizSelecionado?.grupoEconomicoId && clienteMatrizSelecionado?.grupoEconomicoId != "[]") {
    grupoEconomicoSelect?.disable();
    let grupo = clienteMatrizSelecionado.grupoEconomicoId;
    let gruposOpcoes = await grupoEconomicoSelect?.loadOptions();
    let gruposSelecionados = gruposOpcoes?.filter(sl => sl.id == grupo)
    gruposSelecionados = gruposSelecionados ? [...gruposSelecionados] : gruposSelecionados;
    
    grupoEconomicoSelect?.clear();
    grupoEconomicoSelect.setCurrentValue(gruposSelecionados);
    grupoEconomicoSelect?.enable();
  }

  if(clienteMatrizSelecionado?.nome) {
    $("#nome-input").val(clienteMatrizSelecionado.nome);
  }

  if(clienteMatrizSelecionado?.codigo) {
    $("#cod-input").val(clienteMatrizSelecionado.codigo);
  }

  if(clienteMatrizSelecionado?.aberturaNegociacao) {
    $("#an-input").val(clienteMatrizSelecionado.aberturaNegociacao);
  }

  if(clienteMatrizSelecionado?.dataCorteFopag) {
    $("#corte-input").val(clienteMatrizSelecionado.dataCorteFopag);
  }

  if(clienteMatrizSelecionado?.dataInativacao) {
    dataInativacaoDatePicker?.setValue(clienteMatrizSelecionado?.dataInativacao);
  }

  if(clienteMatrizSelecionado?.tiposDocumentosIds && clienteMatrizSelecionado?.tiposDocumentosIds instanceof Array && clienteMatrizSelecionado?.tiposDocumentosIds?.length > 0) {
    tipoDocumentoSelect?.disable();
    let tiposDocumentosIds = clienteMatrizSelecionado.tiposDocumentosIds;
    let tiposDocumentosOpcoes = await tipoDocumentoSelect?.loadOptions();
    let tiposDocumentosSelecionados = tiposDocumentosOpcoes?.filter(sl => tiposDocumentosIds.find(v => v == sl.id))
    tiposDocumentosSelecionados = tiposDocumentosSelecionados ? [...tiposDocumentosSelecionados] : tiposDocumentosSelecionados;
    
    tipoDocumentoSelect?.clear();
    tipoDocumentoSelect.setCurrentValue(tiposDocumentosSelecionados);
    tipoDocumentoSelect?.enable();
  }

  if (clienteMatrizSelecionado?.tipoProcessamento) {
    tipoProcessamentoSelect?.disable();

    let tipoProcessamentoOpcoes = await tipoProcessamentoSelect.loadOptions();
    let tipoProcessamentoSelecionado = tipoProcessamentoOpcoes?.filter(sl => sl.id == clienteMatrizSelecionado?.tipoProcessamento);

    tipoProcessamentoSelect?.clear();
    tipoProcessamentoSelect?.setCurrentValue(tipoProcessamentoSelecionado);
    
    tipoProcessamentoSelect?.enable();
  }

  if (clienteMatrizSelecionado?.slaPrioridade) {
    slaPrioridadeSelect?.disable();

    let slaPrioridadeOpcoes = await slaPrioridadeSelect.loadOptions();
    let slaPrioridadeSelecionada = slaPrioridadeOpcoes?.filter(sl => sl.id == clienteMatrizSelecionado?.slaPrioridade);

    slaPrioridadeSelect?.clear();
    slaPrioridadeSelect?.setCurrentValue(slaPrioridadeSelecionada);
    
    slaPrioridadeSelect?.enable();
  }

  id_modulos = " " + clienteMatrizSelecionado?.modulosIds?.join(" ");
}

async function carregarDatatableModulos() {
  if (modulosTb) {
    modulosTb.reload();
    return;
  }

  $("#seleciona_todos_modulos").on("click", (event) => {
    if (event.currentTarget.checked) {
      $('.modulo').prop('checked', true);
      $('.modulo').trigger('change');
    } else {
      $('.modulo').prop('checked', false);
      $('.modulo').trigger('change');
    }
  });

  modulosTb = new DataTableWrapper('#modulosTb', {
    columns: [
      { "data": "id", orderable: false },
      { "data": "nome", title: "Módulo" },
    ],
    ajax: async (requestData) => {
      $('#seleciona_todos_modulos').val(false).prop('checked', false);
      requestData.SortColumn = 'id';
      requestData.Columns = 'id,nome';
      return await moduloService.obterComercialDatatable(requestData);
    },

    rowCallback: function (row, data) {
      const checkbox = $("<input>").attr("type", "checkbox").attr("data-id", data?.id).addClass('modulo')

      $("td:eq(0)", row).html(checkbox);

      if (id_modulos) {
        const ids = id_modulos.split(" ");
        const isChecked = ids.indexOf('' + data?.id);
        if (isChecked >= 0) {
          $(row).find('.modulo').prop('checked', true);
        }
      }

      handleSelectModulo(row);
    }
  });

  await modulosTb.initialize();
}

function configurarModal() {
  const pageCtn = document.getElementById('pageCtn');

  const novoClientMatrizModalHidden = document.getElementById('novoClientMatrizHidden');
  const novoClientMatrizModalHiddenContent = document.getElementById('novoClientMatrizContent');

  const modalsConfig = [
    {
      id: 'novoClientMatrizModal',
      modal_hidden: novoClientMatrizModalHidden,
      content: novoClientMatrizModalHiddenContent,
      btnsConfigs: [],
      onOpen: async () => {
        await carregarDatatableModulos()
        if(clienteMatrizSelecionado) {
          $("#btn-inativar-ativar").show();
          $("#btn-inativar-ativar").html(clienteMatrizSelecionado?.dataInativacao ? "Ativar" : "Inativar");
        }
        else {
          $("#btn-inativar-ativar").hide();
        }
        // configCollapsePanels()

        // if (update) {
        //   $('#cnaes_selecionados').show()
        //   await carregarCnaesSelecionados()
        // }

        // await carregarCnaes()
      },
      onClose: () => {
        limparFormularioUpsert();
        clienteMatrizSelecionado = null;
        // update = false
      }
    }
  ];

  renderizarModal(pageCtn, modalsConfig);
}

function limparFormularioUpsert() {
  grupoEconomicoSelect?.clear();
  tipoDocumentoSelect?.clear();
  tipoProcessamentoSelect?.clear();
  slaPrioridadeSelect?.clear();
  dataInativacaoDatePicker?.clear();
  $("#nome-input").val(null);
  $("#cod-input").val(null);
  $("#an-input").val(null);
  $("#corte-input").val(null);
  $("#cod-input").val(null);
  id_modulos = '';
}

function atualizarClienteMatriz(){

}

function addClienteMatriz(){

}

async function configurarFormularios() {
  grupoEconomicoSelect = new SelectWrapper("#ge-input", {onOpened: async () => (await grupoEconomicoService.obterSelect()).value});
  tipoDocumentoSelect = new SelectWrapper("#td-input",  {
    options: { placeholder: 'Selecione', multiple: true },
    onOpened: async () => {
      return (await tipoDocumentoService.obterProcessados()).value;
    }
  });
  slaPrioridadeSelect = new SelectWrapper("#pri-input", {
    options: { placeholder: 'Selecione' },
    onOpened: () => obterSlaPrioridade()
  });
  tipoProcessamentoSelect = new SelectWrapper("#proc-input", {
    options: { placeholder: 'Selecione' },
    onOpened: () => obterTipoProcessamento()
  });
  dataInativacaoDatePicker = new DatepickerWrapper('#dataina-input');

  function obterSlaPrioridade() {
    return [
      {id: "Documento Divulgado", description: "Documento Divulgado"},
      {id: "Processa com Comparativo de Cláusulas", description: "Processa com Comparativo de Cláusulas"},
      {id: "Processa com Formulário", description: "Processa com Formulário"},
      {id: "Processa com Mapa Sindical", description: "Processa com Mapa Sindical"},
    ]
  }

  function obterTipoProcessamento() {
    return [
      {id: 1, description: "assinado ou registrado"},
      {id: 2, description: "sem assinatura"},
      {id: 3, description: "somente registrado"}
    ]
  }
}

function handleSelectModulo(row) {
  $(row).find('.modulo').on('change', function () {
    const dataId = $(this).data("id")

    if ($(this).is(":checked")) {
      if (id_modulos.split(' ').indexOf(dataId + '') === -1) {
        id_modulos += " " + dataId
        console.log(id_modulos);
      }

      return NotificationService.success({ title: 'Módulo selecionado!' })
    }

    id_modulos = (id_modulos + "").replace(' ' + dataId, '')
    console.log(id_modulos);
    NotificationService.success({ title: 'Módulo desselecionado!' })
  })
}

// // KEYCLOACK////////////////////////////////////////////////////////////

// function initKeycloak() {
//   const keycloak = new Keycloak();
//   keycloak
//     .init({
//       onLoad: "login-required",
//       config: {
//         url: "http://localhost:8000/auth/",
//         realm: "Ineditta-prod",
//         clientId: "logineditta",
//       },
//       initOptions: {


//         redirectUri: "http://localhost:8000/clientematriz.php", //a url da pagina que está implementando,

//         checkLoginIframe: false,
//       },
//     })
//     .then(function (authenticated) {
//       //alert(
//       //	authenticated ?
//       //	"authenticated | TOKEN: " + keycloak.token :
//       //	"not authenticated"
//       //);// Garantia de acesso, comentar se confirmar a autenticação e testes não forem mais necessários
//       setAccessToken(keycloak.token);
//       setKeyUserId(keycloak.idTokenParsed.sub);
//       $("#keyusername").html(keycloak.idTokenParsed.name);
//       keycloak.loadUserProfile()
//         .then(function (profile) {
//           // alert(JSON.stringify(profile, null, "  "));

//           console.log(profile);
//           console.log(sessionStorage.getItem(profile.id));

//           $('body').css("display", "");

//           if (!sessionStorage.getItem(profile.id)) {
//             window.location.replace("http://localhost:8000/index.php");
//           }




//         }).catch(function () {
//           alert('Failed to load user profile');
//         });

//       //Código para encapsular o token se necessario abaixo:

//       //Manter nesse bloco:

//       //setAccessToken(keycloak.token); 

//       //Copiar para o arquivo de interesse:

//       //let access_token = "";

//       //function setAccessToken(val) {
//       //  access_token = val;
//       //}


//     })
//     .catch(function (err) {
//       alert("failed to initialize" + JSON.stringify(err));

//     });
// }

// let access_token = "";

// function setAccessToken(val) {
//   access_token = val;
// }

// let key_user_id = "";

// function setKeyUserId(val) {
//   key_user_id = val;
// }


// function logout() {
//   sessionStorage.clear();
//   var settings = {
//     "url": "http://localhost:8000/auth/admin/realms/Ineditta-prod/users/" + key_user_id + "/logout",
//     "method": "POST",
//     "timeout": 0,
//     "headers": {
//       "Authorization": "Bearer " + access_token,
//     },
//   };

//   $.ajax(settings).done(function (response) {
//     console.log(response);
//     document.location.href = 'http://localhost:8000/exit.php'

//     document.location.reload(true);
//   });

// }




// $(document).ready(function () {
//   $.fn.datepicker.dates['pt-BR'] = {
//     format: 'dd/mm/yyyy',
//     days: ["Domingo", "Segunda", "Terรงa", "Quarta", "Quinta", "Sexta", "Sรกbado", "Domingo"],
//     daysShort: ["Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "Sรกb", "Dom"],
//     daysMin: ["Do", "Se", "Te", "Qu", "Qu", "Se", "Sa", "Do"],
//     months: ["Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho", "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"],
//     monthsShort: ["Jan", "Fev", "Mar", "Abr", "Mai", "Jun", "Jul", "Ago", "Set", "Out", "Nov", "Dez"],
//     today: "Hoje",
//     suffix: [],
//     meridiem: []
//   };
//   $(".datepicker").datepicker({
//     autoclose: true,
//     todayHighlight: true,
//     toggleActive: true,
//     format: 'dd/mm/yyyy',
//     language: 'pt-BR'
//   }).on('changeDate', function (selected) {
//     updateDate($(this).closest('form').find('.datepicker'), selected);
//   });


//   $("#dataina-input").mask('00/00/0000');
//   $("#dataina-inputu").mask('00/00/0000');
//   $("#dataclu-input").mask('00/00/0000');
//   $("#dataclu-inputu").mask('00/00/0000');


//   $("#cnpj-inputu").mask('00.000.000/0000-00');
//   $(".cnpj_format").mask('00.000.000/0000-00');

//   $("#cep-input").mask('00000-000');
//   $("#cep-inputu").mask('00000-0s00');

//   $("#an-input").mask('00');
//   $("#an-inputu").mask('00');


// });

// //Click botão adicionar novo registro
// $("#btn_novo").on("click", () => {
//   setTimeout(() => {
//     $(".select2").select2()
//   }, 200)

//   $("#cnpj-input").mask('00.000.000/0000-00');
// })



// function selecionarTodos() {
//   var check = document.querySelectorAll(".check_modulo")
//   console.log(check)

//   $(check).prop("checked", true)

//   var listaId = []
//   check.forEach(element => {
//     var id = element.getAttribute("data-id")

//     listaId.push(id)
//   });

//   //console.log(listaId)
//   $("#modulos-input").val(listaId)

//   console.log($("#modulos-input").val())
// }

// function limparSelecao() {
//   var check = document.querySelectorAll(".check_modulo")
//   console.log(check)

//   $(check).prop("checked", false)


//   $("#modulos-input").val('')

//   console.log($("#modulos-input").val())

//   $("#select_all").prop("disabled", false)
// }

// function updateDate(inputs, selected) {
//   var minDate = new Date(selected.date.valueOf());
//   $(inputs[1]).datepicker('setStartDate', minDate);
//   $(inputs[0]).datepicker('setEndDate', minDate);
// }

// async function addClienteMatriz() {
//   var imgInBase64 = new Promise(function (resolve, reject) {
//     var fileInput = document.querySelector('#logo-input');
//     var reader = new FileReader();
//     if (fileInput.files.length > 0 && fileInput.files[0].size < (1048576 * 0.55)) {
//       reader.readAsDataURL(fileInput.files[0]);
//       reader.onload = function () {
//         resolve(reader.result);//base64encoded string
//       };
//       reader.onerror = function (error) {
//         reject(error);
//       };
//     } else {
//       resolve("");
//     }

//   });
//   img = await imgInBase64;

//   //Campos obrigatórios
//   if (
//     $("#cod-input").val() == "" ||
//     $("#cnpj-input").val() == "" ||
//     $("#ge-input").val() == "" ||
//     $("#nome-input").val() == "" ||
//     $("#rs-input").val() == "" ||
//     $("#end-input").val() == "" ||
//     $("#bairro-input").val() == "" ||
//     $("#cep-input").val() == "" ||
//     $("#cid-input").val() == "" ||
//     $("#an-input").val() == ""
//   ) {
//     Swal.fire({
//       position: 'top-end',
//       icon: 'warning',
//       title: 'Preencha todos os campos obrigatórios!',
//       showConfirmButton: false,
//       timer: 2500
//     })

//     let input = document.querySelectorAll('.required');
//     $(input).addClass("has-warning")

//     return;
//   }
//   $.ajax(
//     {
//       url: "includes/php/ajax.php"
//       , type: "post"
//       , dataType: "json"
//       , data: {
//         "module": "clientematriz",
//         "action": "addClienteMatriz",
//         "cod-input": $("#cod-input").val(),
//         "cnpj-input": $("#cnpj-input").val(),
//         "ge-input": $("#ge-input").val(),
//         "nome-input": $("#nome-input").val(),
//         "rs-input": $("#rs-input").val(),
//         "end-input": $("#end-input").val(),
//         "ent-input": $("#ent-input").val(),
//         "corte-input": $("#corte-input").val(),
//         "proc-input": $("#proc-input").val(),
//         "pri-input": $("#pri-input").val(),
//         "bairro-input": $("#bairro-input").val(),
//         "cep-input": $("#cep-input").val(),
//         "cid-input": $("#cid-input").val(),
//         "uf-input": $("#uf-input").val(),
//         "dataina-input": $("#dataina-input").val(),
//         "an-input": $("#an-input").val(),
//         "dataclu-input": $("#dataclu-input").val(),
//         "td-input": $("#td-input").val(),
//         "cla-input": $("#cla-input").val(),
//         "modulos-input": $("#modulos-input").val(),
//         "logo-input": img

//       }
//       , beforeSend: function (xhr) {
//         $("#mensagem_sucesso").hide();
//       }
//       , complete: function (xhr, textStatus) {
//         $("#preload").hide();
//       }
//       , success: function (data) {

//         Swal.fire({
//           position: 'top-end',
//           icon: 'success',
//           title: 'Cadastro realizado com sucesso!',
//           showConfirmButton: false,
//           timer: 2000
//         })

//         //  document.querySelectorAll('input[type=checkbox]').forEach(el => el.checked = false);

//       }
//       , error: function (jqXHR, textStatus, errorThrown) {

//         Swal.fire({
//           position: 'top-end',
//           icon: 'error',
//           title: 'Não foi posssível realizar o cadastro' + textStatus,
//           showConfirmButton: false,
//           timer: 2000
//         })


//         // document.querySelectorAll('input[type=checkbox]').forEach(el => el.checked = isAllCheck);

//         //window.alert("ERRO: " + errorThrown + "    STATUS: " + textStatus);
//       }
//     }
//   )
// }

// function addMod(id_modulos) {
//   check = null;
//   if (document.getElementById('inicheck' + id_modulos + '').checked) {
//     $("#modulos-input").val($("#modulos-input").val() + " " + id_modulos);
//     // $("#select_all").prop("disabled", true)


//   } else {
//     $("#modulos-input").val(($("#modulos-input").val() + "").replace(' ' + id_modulos, ''));

//   }

//   console.log($("#modulos-input").val());


//   var campo = $("#modulos-input").val()

//   if (!campo) {
//     $("#select_all").prop("disabled", false)
//     console.log('ola' + campo + 'oi');
//   } else {
//     $("#select_all").prop("disabled", true)
//     console.log('ola' + campo + 'oi');
//   }




// }

// function saveModuleChange(id_empresa, id_modulos) {
//   // check = null;
//   // if (document.getElementById('check'+id_modulos+'').checked) 
//   // {
//   // 	check = 1;
//   // } else 
//   // {
//   // 	check = 0;
//   // }		

//   if (document.getElementById('check' + id_modulos + '').checked) {
//     $("#modulos_update").val($("#modulos_update").val() + " " + id_modulos);


//   } else {
//     $("#modulos_update").val(($("#modulos_update").val() + "").replace(' ' + id_modulos, ''));

//   }

//   console.log($("#modulos_update").val());



//   // $.ajax(
//   // 	{
//   // 		 url: "includes/php/ajax.php"
//   // 		,type: "post"
//   // 		,dataType: "json"
//   // 		,data: {
//   // 			"module": "clientematriz", 
//   // 			"action": "saveModuleChange",
//   // 			"id_empresa": id_empresa,
//   // 			"id_modulos": id_modulos,
//   // 			"check": check

//   // 		}
//   // 		,beforeSend: function(xhr){
//   // 			//$("#mensagem_sucesso").hide();
//   // 		}
//   // 		,complete: function( xhr, textStatus ) {
//   // 			$("#preload").hide();
//   // 		}
//   // 		,success: function (data) {

//   // 			Swal.fire({
//   // 				position: 'top-end',
//   // 				icon: 'success',
//   // 				title: 'Cadastro realizado com sucesso!',
//   // 				showConfirmButton: false,
//   // 				timer: 2000
//   // 			})


//   // 		}
//   // 		,error: function (jqXHR, textStatus, errorThrown) {

//   // 			Swal.fire({
//   // 				position: 'top-end',
//   // 				icon: 'error',
//   // 				title: 'Não foi posssível realizar o cadastro' + textStatus,
//   // 				showConfirmButton: false,
//   // 				timer: 2000
//   // 			})


//   // 			//window.alert("ERRO: " + errorThrown + "    STATUS: " + textStatus);
//   // 		}
//   // 	}
//   // )
// }

// async function updateClienteMatriz(id_empresa) {
//   var imgInBase64 = new Promise(function (resolve, reject) {
//     var fileInput = document.querySelector('#logo-inputu');
//     var reader = new FileReader();
//     if (fileInput.files.length > 0 && fileInput.files[0].size < (1048576 * 0.55)) {
//       reader.readAsDataURL(fileInput.files[0]);
//       reader.onload = function () {
//         resolve(reader.result);//base64encoded string
//       };
//       reader.onerror = function (error) {
//         reject(error);
//       };
//     } else {
//       resolve($("#logo-update").prop("src"));
//     }

//   });
//   img = await imgInBase64;
//   $("#logo-update").prop("src", img);

//   //OBTEM TODOS OS IDS DOS MODULOS MARCADOS
//   var check = document.querySelectorAll('[data-mod]');
//   var listaModulos = []
//   var listaModulosDesmarcados = []
//   console.log(check);
//   check.forEach(element => {
//     if (element.checked) {
//       listaModulos.push(element.getAttribute("data-mod"))
//     } else {
//       listaModulosDesmarcados.push(element.getAttribute("data-mod"))
//     }

//   });

//   console.log(listaModulos);
//   console.log(listaModulosDesmarcados);


//   $.ajax(
//     {
//       url: "includes/php/ajax.php"
//       , type: "post"
//       , dataType: "json"
//       , data: {
//         "module": "clientematriz",
//         "action": "updateClienteMatriz",
//         "cod-input": $("#cod-inputu").val(),
//         "cnpj-input": $("#cnpj-inputu").val(),
//         "ge-input": $("#ge-inputu").val(),
//         "nome-input": $("#nome-inputu").val(),
//         "rs-input": $("#rs-inputu").val(),
//         "end-input": $("#end-inputu").val(),
//         "bairro-input": $("#bairro-inputu").val(),
//         "cep-input": $("#cep-inputu").val(),
//         "cid-input": $("#cid-inputu").val(),
//         "uf-input": $("#uf-inputu").val(),
//         "dataina-input": $("#dataina-inputu").val(),
//         "an-input": $("#an-inputu").val(),
//         "dataclu-input": $("#dataclu-inputu").val(),
//         "td-input": $("#td-inputu").val(),
//         "ent-input": $("#ent-inputu").val(),
//         "corte-input": $("#corte-inputu").val(),
//         "proc-input": $("#proc-inputu").val(),
//         "pri-input": $("#pri-inputu").val(),
//         "cla-input": $("#cla-inputu").val(),
//         "logo-input": img,
//         "id_empresa": id_empresa

//       }
//       , success: function (data) {

//         Swal.fire({
//           position: 'top-end',
//           icon: 'success',
//           title: 'Cadastro realizado com sucesso!',
//           showConfirmButton: false,
//           timer: 2000
//         })

//       }
//       , error: function (jqXHR, textStatus, errorThrown) {

//         Swal.fire({
//           position: 'top-end',
//           icon: 'error',
//           title: 'Não foi posssível realizar o cadastro' + textStatus,
//           showConfirmButton: false,
//           timer: 2000
//         })
//       }
//     }
//   )

//   //UPDATE DE MÓDULOS
//   $.ajax(
//     {
//       url: "includes/php/ajax.php"
//       , type: "post"
//       , dataType: "json"
//       , data: {
//         "module": "clientematriz",
//         "action": "saveModuleChange",
//         "modulos": listaModulos,
//         "desmarcados": listaModulosDesmarcados,
//         "empresa": id_empresa

//       }
//       , success: function (data) {

//       }
//       , error: function (jqXHR, textStatus, errorThrown) {

//         Swal.fire({
//           position: 'top-end',
//           icon: 'error',
//           title: 'Não foi posssível atualizar os módulos' + textStatus,
//           showConfirmButton: false,
//           timer: 2500
//         })

//       }
//     }
//   )
// }

// $('#btn-atualizar').on('click', function () {
//   var id = $('#id-inputu').val();
//   updateClienteMatriz(id);
// });

// function selectGrupo(idGrupo) {
//   $("#ge-input").val(idGrupo);
//   $("#ge-input").prop("disabled", true);
//   $("#ge-inputu").val(idGrupo);
//   $("#ge-inputu").prop("disabled", true);
// }


// $('.btn_cancelar').click(function () {

//   document.location.reload(true);

// });

// function getByIdClienteMatriz(id_empresa) {

//   $.ajax(
//     {
//       url: "includes/php/ajax.php"
//       , type: "post"
//       , dataType: "json"
//       , data: {
//         "module": "clientematriz",
//         "action": "getByIdClienteMatriz",
//         "id_empresa": id_empresa
//       }
//       , beforeSend: function (xhr) {
//         //$('#preload').show();
//       }
//       , complete: function (xhr, textStatus) {
//         //$("#preload").hide();
//       }
//       , success: function (data) {

//         $("#id-inputu").val(data.response_data.id_empresa);
//         $("#cod-inputu").val(data.response_data.codigo_empresa);
//         $("#cnpj-inputu").val(data.response_data.cnpj_empresa);
//         $("#ge-inputu").val(data.response_data.cliente_grupo_id_grupo_economico);
//         $("#nome-inputu").val(data.response_data.nome_empresa);
//         $("#rs-inputu").val(data.response_data.razao_social);
//         $("#end-inputu").val(data.response_data.logradouro_empresa);
//         $("#bairro-inputu").val(data.response_data.bairro);
//         $("#cep-inputu").val(data.response_data.cep);
//         $("#cid-inputu").val(data.response_data.cidade);
//         $("#uf-inputu").val(data.response_data.uf);
//         $("#dataina-inputu").val(data.response_data.data_inativacao);
//         $("#an-inputu").val(data.response_data.abri_neg);
//         $("#dataclu-inputu").val(data.response_data.data_inclusao);
//         $("#td-inputu").val(data.response_data.tip_doc);
//         $("#logo-update").prop("src", data.response_data.logo_empresa);
//         $("#corte-inputu").val(data.response_data.data_cortefopag);
//         $("#ent-inputu").val(data.response_data.sla_entrega);
//         $("#proc-inputu").val(data.response_data.tipo_processamento);
//         $("#pri-inputu").val(data.response_data.sla_prioridade);
//         $("#cla-inputu").val(data.response_data.classe_doc);


//         $("#teste-tabela").html(data.response_data.listaMod2);
//         $("#tabela-historico").html(data.response_data.listaHistorico);
//       }
//       , error: function (jqXHR, textStatus, errorThrown) {
//         window.alert("ERRO: " + errorThrown + "    STATUS: " + textStatus);
//         /* bootbox.alert({
//           message: ( 'Ocorreu um erro inesperado ao processar sua solicitação.' ),
//           size: 'small'
//         }); */
//       }
//     }
//   )
// }


// $('#btn-cancelar').click(function () {

//   document.location.reload(true);

// });

// $('#btn-cancelar2').click(function () {

//   document.location.reload(true);

// });
