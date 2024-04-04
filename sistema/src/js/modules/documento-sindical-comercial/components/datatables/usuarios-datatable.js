import DataTableWrapper from "../../../../utils/datatables/datatable-wrapper";
import NotificationService from "../../../../utils/notifications/notification.service";

export class UsuarioTb {
    constructor(
        pageContext,
        usuarioAdmService
    ) {
        const { usuariosTb } = pageContext.formulario.datatables;

        this.pageContext = pageContext;
        this.usuarioAdmService = usuarioAdmService;

        this.carregarAsync = this.carregarAsync.bind(this);
        usuariosTb.carregarAsync = this.carregarAsync.bind(this);
    }

    async carregarAsync() {
        const { usuariosTb, empresaFiltroUsuarioTb } = this.pageContext.formulario.datatables;
        const { sindicatoLaboralSelect, sindPatronalSelect } = this.pageContext.formulario.selects;

        if (usuariosTb.content) {
          usuariosTb.content.reload();
          return;
        }
      
        $("#seleciona_todos_usuario").on("click", (event) => {
          if (event.currentTarget.checked) {
            $('.usuario-adm').prop('checked', true);
            $('.usuario-adm').trigger('change');
          } else {
            $('.usuario-adm').prop('checked', false);
            $('.usuario-adm').trigger('change');
          }
        });
      
        usuariosTb.content = new DataTableWrapper('#usuariosTb', {
          columns: [
            { "data": "id", orderable: false },
            { "data": "nome", title: "Nome do Usuário" },
            { "data": "departamento", title: "Departamento" },
            { "data": "nivel", title: "Nível" }
          ],
          ajax: async (requestData) => { 
            $('#seleciona_todos_usuario').val(false).prop('checked', false);
            console.log(empresaFiltroUsuarioTb.checkboxsSelecionados.split(" ").filter(v => !!v));
            requestData.estabelecimentosIds = empresaFiltroUsuarioTb.checkboxsSelecionados.split(" ").filter(v => !!v);
            requestData.FiltrarUsuariosAceitamNotificacaoEmail = true;
            requestData.SindicatosLaboraisIds = sindicatoLaboralSelect?.getValue()?.map(v => Number(v));
            requestData.SindicatosPatronaisIds = sindPatronalSelect?.getValue()?.map(v => Number(v));
            return await this.usuarioAdmService.obterDatatablePorGrupoEconomico(requestData)
          },
          rowCallback: (row, data) => {
            const checkbox = $("<input>").attr("type", "checkbox").attr("data-id", data?.id).addClass('usuario-adm')
      
            $("td:eq(0)", row).html(checkbox);
      
            if (usuariosTb.checkboxsSelecionados) {
              const ids = usuariosTb.checkboxsSelecionados.split(" ");
              const isChecked = ids.indexOf('' + data?.id);
              if (isChecked >= 0) {
                $(row).find('.usuario-adm').prop('checked', true);
              }
            }
      
            this._handlerCheckboxChange(row);
          }
        });
      
        await usuariosTb.content.initialize();
    }

    _handlerCheckboxChange(row) {
        const { usuariosTb } = this.pageContext.formulario.datatables;

        $(row).find('.usuario-adm').on('change', function () {
          const dataId = $(this).data("id")
      
          if ($(this).is(":checked")) {
            if (!usuariosTb.checkboxsSelecionados.split(" ").some(u => u == dataId)) {
              usuariosTb.checkboxsSelecionados += " " + dataId
            }
            console.log(usuariosTb.checkboxsSelecionados);
            return NotificationService.success({ title: 'Usuário selecionado!' })
          }
      
          usuariosTb.checkboxsSelecionados = (usuariosTb.checkboxsSelecionados + "").replace(' ' + dataId, '')
          console.log(usuariosTb.checkboxsSelecionados);
      
          NotificationService.success({ title: 'Usuário desselecionado!' })
        })
    }
}