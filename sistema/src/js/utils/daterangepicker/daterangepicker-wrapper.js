import 'daterangepicker';
import $ from 'jquery';

import 'daterangepicker/daterangepicker.css';

export default class DatepickerrangeWrapper {
  constructor(selector) {
    this.selector = selector;
    this.beginDate = null;
    this.endDate = null;
    this.init();
  }

  init() {
    $(this.selector).daterangepicker({
      "locale": {
        "format": "DD/MM/YYYY",
        "separator": " - ",
        "applyLabel": "Aplicar",
        "cancelLabel": "Cancelar",
        "fromLabel": "De",
        "toLabel": "Até",
        "customRangeLabel": "Custom",
        "daysOfWeek": [
          "Dom",
          "Seg",
          "Ter",
          "Qua",
          "Qui",
          "Sex",
          "Sáb"
        ],
        "monthNames": [
          "Janeiro",
          "Fevereiro",
          "Março",
          "Abril",
          "Maio",
          "Junho",
          "Julho",
          "Agosto",
          "Setembro",
          "Outubro",
          "Novembro",
          "Dezembro"
        ],
      },
      autoUpdateInput: false,
    });

    const self = this;

    $(this.selector).on('apply.daterangepicker', function (_, picker) {
      $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
      self.beginDate = picker.startDate?.toDate();
      self.endDate = picker.endDate?.toDate();
    });

    /*$(this.selector).on('cancel.daterangepicker', function () {
      $(this).val('');
      self.beginDate = null;
      self.endDate = null;
    });*/

    $(this.selector).on('change', function () {
      if($(this).val() === '') {
        self.clear();
        console.log("Fui limpo!");
      }
    });

    $(this.selector).on('keypress', function (e) {
      if (e.key === 'Enter') {
        $(".applyBtn.btn.btn-sm.btn-primary").trigger('click');
      }
    });
  }

  getBeginDate() {
    return this.beginDate;
  }

  getEndDate() {
    return this.endDate;
  }
  
  clear(){
    $(this.selector).val('');
    this.beginDate = null;
    this.endDate = null;
  }

  hasValue(){
    return this.beginDate && this.endDate;
  }
}