import DataTableWrapper from "../../../../utils/datatables/datatable-wrapper";
import NotificationService from "../../../../utils/notifications/notification.service";
import Masker from "../../../../utils/masks/masker";

export class EmpresaTb {
  constructor(pageContext, clienteUnidadeService) {
    const { empresaTb } = pageContext.formulario.datatables;

    this.pageContext = pageContext;
    this.clienteUnidadeService = clienteUnidadeService;

    this.carregarAsync = this.carregarAsync.bind(this);
    empresaTb.carregarAsync = this.carregarAsync.bind(this);
  }

  async carregarAsync() {
    const { empresaTb } = this.pageContext.formulario.datatables;

    if (empresaTb.content) return;

    $("#seleciona_todas_empresas").on("click", (event) => {
      if (event.currentTarget.checked) {
        $(".empresa").prop("checked", true);
        $(".empresa").trigger("change");
      } else {
        $(".empresa").prop("checked", false);
        $(".empresa").trigger("change");
      }
    });

    empresaTb.content = new DataTableWrapper("#empresaTb", {
      columns: [
        { data: "id", orderable: false },
        { data: "grupo", title: "Grupo Econômico" },
        { data: "filial", title: "Empresa" },
        { data: "nome", title: "Estabelecimento" },
        { data: "cnpj", title: "Cnpj", render: (data) => Masker.CNPJ(data) },
      ],
      ajax: async (requestData) => {
        $("#seleciona_todas_empresas").val(false).prop("checked", false);
        requestData.Columns = "id,grupo,filial,nome,cnpj";
        requestData.SortColumn = "id";
        requestData.grupoUsuario = true;
        return await this.clienteUnidadeService.obterDatatable(requestData);
      },

      rowCallback: (row, data) => {
        const checkbox = $("<input>")
          .attr("type", "checkbox")
          .attr("data-id", data?.id)
          .addClass("empresa");

        $("td:eq(0)", row).html(checkbox);

        if (empresaTb.checkboxsSelecionados) {
          const ids = empresaTb.checkboxsSelecionados.split(" ");
          const isChecked = ids.indexOf("" + data?.id);
          if (isChecked >= 0) {
            $(row).find(".empresa").prop("checked", true);
          }
        }

        this._handlerCheckboxChange(row);
      },
    });

    await empresaTb.content.initialize();
  }

  _handlerCheckboxChange(row) {
    const { empresaTb } = this.pageContext.formulario.datatables;

    $(row).find('.empresa').on('change', function () {
      const dataId = $(this).data("id")
  
      if ($(this).is(":checked")) {
        if (empresaTb.checkboxsSelecionados.split(' ').indexOf(dataId + '') === -1) {
          empresaTb.checkboxsSelecionados += " " + dataId
        }
  
        return NotificationService.success({ title: 'Empresa selecionada!' })
      }
  
      empresaTb.checkboxsSelecionados = (empresaTb.checkboxsSelecionados + "").replace(' ' + dataId, '')
  
      NotificationService.success({ title: 'Empresa desselecionada!' })
    })
  }
}
