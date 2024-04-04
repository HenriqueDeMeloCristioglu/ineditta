import DataTableWrapper from "../../../../utils/datatables/datatable-wrapper";

export class AbrangenciasTb {
  constructor(pageContext, localizacaoService) {
    const { abrangenciasTb } = pageContext.formulario.datatables;

    this.pageContext = pageContext;
    this.localizacaoService = localizacaoService;

    this.carregarAsync = this.carregarAsync.bind(this);
    abrangenciasTb.carregarAsync = this.carregarAsync.bind(this);
  }

  async carregarAsync() {
    const { abrangenciasTb } = this.pageContext.formulario.datatables;

    if (abrangenciasTb.content !== null)
      return await abrangenciasTb.content.reload();

    $("#seleciona_todas_regioes").on("click", (event) => {
      if (event.currentTarget.checked) {
        $(".abrangencia").prop("checked", true);
        $(".abrangencia").trigger("change");
      } else {
        $(".abrangencia").prop("checked", false);
        $(".abrangencia").trigger("change");
      }
    });

    abrangenciasTb.content = new DataTableWrapper("#abrangenciaTb", {
      columns: [
        { data: "id", orderable: false, title: "Selecione" },
        { data: "municipio", title: "Municipio" },
        { data: "pais", title: "País" },
      ],
      ajax: async (requestData) => {
        $("#seleciona_todas_regioes").val(false).prop("checked", false);
        return await this.localizacaoService.obterDatatablePorUf(
          requestData,
          $("#uf_input").val()
        );
      },
      rowCallback: (row, data) => {
        const checkbox = $("<input>")
          .attr("type", "checkbox")
          .attr("data-id", data?.id)
          .addClass("abrangencia");

        $("td:eq(0)", row).html(checkbox);

        const dataId = $(row).find(".abrangencia").attr("data-id");

        if (abrangenciasTb.checkboxsSelecionados) {
          const ids = abrangenciasTb.checkboxsSelecionados.split(" ");
          const isChecked = ids.indexOf("" + dataId);
          if (isChecked >= 0) {
            $(row).find(".abrangencia").prop("checked", true);
          }
        }

        this._handlerCheckboxChange(row);
      },
    });

    await abrangenciasTb.content.initialize();
  }

  _handlerCheckboxChange(row) {
    const { abrangenciasTb } = this.pageContext.formulario.datatables;

    $(row)
      .find(".abrangencia")
      .on("change", function () {
        const dataId = $(this).data("id")

        if ($(row).find(".abrangencia").is(":checked")) {
          if (abrangenciasTb.checkboxsSelecionados.split(" ").indexOf(dataId + "") === -1) {
            abrangenciasTb.checkboxsSelecionados += " " + dataId;
          }
        } else {
          abrangenciasTb.checkboxsSelecionados = (abrangenciasTb.checkboxsSelecionados + "").replace(" " + dataId, "");
        }

        console.log(abrangenciasTb.checkboxsSelecionados);
    });
  }
}
