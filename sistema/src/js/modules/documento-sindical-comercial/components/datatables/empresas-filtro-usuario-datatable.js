import DataTableWrapper from "../../../../utils/datatables/datatable-wrapper";
import NotificationService from "../../../../utils/notifications/notification.service";
import Masker from "../../../../utils/masks/masker";

export class EmpresasParaFiltrarUsuariosTb {
    constructor(
        pageContext,
        clienteUnidadeService
    ) {
        const { empresaFiltroUsuarioTb } = pageContext.formulario.datatables;

        this.pageContext = pageContext;
        this.clienteUnidadeService = clienteUnidadeService;

        this.carregarAsync = this.carregarAsync.bind(this);
        empresaFiltroUsuarioTb.carregarAsync = this.carregarAsync.bind(this);
    }

    async carregarAsync() {
        const { empresaFiltroUsuarioTb } = this.pageContext.formulario.datatables;

        if (empresaFiltroUsuarioTb.content) {
          empresaFiltroUsuarioTb.content.reload();
          return;
        }
      
        $("#seleciona_todas_empresas_filtro_usuario").on("click", (event) => {
          if (event.currentTarget.checked) {
            $('.empresa-filtro-usuario').prop('checked', true);
            $('.empresa-filtro-usuario').trigger('change');
          } else {
            $('.empresa-filtro-usuario').prop('checked', false);
            $('.empresa-filtro-usuario').trigger('change');
          }
        });
      
        empresaFiltroUsuarioTb.content = new DataTableWrapper('#empresaFiltroUsuarioTb', {
          columns: [
            { "data": "id", orderable: false },
            { "data": "grupo", title: "Grupo Econômico" },
            { "data": "filial", title: "Empresa" },
            { "data": "nome", title: "Estabelecimento" },
            { "data": "cnpj", title: "Cnpj", render: (data) => Masker.CNPJ(data) }
          ],
          ajax: async (requestData) => {
            $('#seleciona_todas_empresas_filtro_usuario').val(false).prop('checked', false);
            requestData.SortColumn = 'id';
            requestData.grupoUsuario = true;
            requestData.Columns = 'id,grupo,filial,nome,cnpj';
            return await this.clienteUnidadeService.obterDatatable(requestData)
          },
      
          rowCallback: (row, data) => {
            const checkbox = $("<input>").attr("type", "checkbox").attr("data-id", data?.id).addClass('empresa-filtro-usuario')
      
            $("td:eq(0)", row).html(checkbox);
      
            if (empresaFiltroUsuarioTb.checkboxsSelecionados) {
              const ids = empresaFiltroUsuarioTb.checkboxsSelecionados.split(" ");
              const isChecked = ids.indexOf('' + data?.id);
              if (isChecked >= 0) {
                $(row).find('.empresa-filtro-usuario').prop('checked', true);
              }
            }
      
            this._handlerCheckboxChange(row);
          }
        });
      
        await empresaFiltroUsuarioTb.content.initialize();
    }

    _handlerCheckboxChange(row) {
        const { empresaFiltroUsuarioTb } = this.pageContext.formulario.datatables;

        $(row).find('.empresa-filtro-usuario').on('change', function () {
          const dataId = $(this).data("id")
      
          if ($(this).is(":checked")) {
            if (empresaFiltroUsuarioTb.checkboxsSelecionados.split(' ').indexOf(dataId + '') === -1) {
              empresaFiltroUsuarioTb.checkboxsSelecionados += " " + dataId
            }
      
            return NotificationService.success({ title: 'Empresa selecionada!' })
          }
      
          empresaFiltroUsuarioTb.checkboxsSelecionados = (empresaFiltroUsuarioTb.checkboxsSelecionados + "").replace(' ' + dataId, '')
      
          NotificationService.success({ title: 'Empresa desselecionada!' })
        })
      }
}