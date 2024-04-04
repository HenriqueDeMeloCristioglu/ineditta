import $ from 'jquery';
import 'bootstrap-datepicker';
import 'bootstrap-datepicker/dist/css/bootstrap-datepicker3.css';
import 'bootstrap-datepicker/dist/locales/bootstrap-datepicker.pt-BR.min.js';
import DateParser from '../date/date-parser.js';

export default class DatepickerWrapper {
  constructor(selector, changeEventHandler = null, format = null) {
    this.selector = selector;
    this.changeEventHandler = changeEventHandler === undefined ? null : changeEventHandler;
    this.format = format;
    this.options = {
      autoclose: true,
      todayHighlight: true,
      todayBtn: 'linked',
      language: 'pt-BR'
    };

    this.initialize();
  }

  initialize() {
    const changeEventHandler = this.changeEventHandler;
    const selector = this.selector;

    if (this.format === 'mes-ano') {
      this.options.format = 'MM/yyyy';
      this.options.minViewMode = 'months';
      this.options.maxViewMode = 'years';
    }

    $(this.selector)
      .datepicker({ ...this.options })
      .on('changeDate', function () {
        const value = $(selector).datepicker('getDate');
        if (changeEventHandler) {
          changeEventHandler($(selector), value);
        }
      })
      .on('show', (evt) => {
        evt.preventDefault();
        evt.stopPropagation();
      })
      .on('hide', (evt) => {
        evt.preventDefault();
        evt.stopPropagation();
      });
  }

  setValue(value) {
    if (value && typeof value === 'string') {
      value = DateParser.fromString(value);
    }

    if (!value) {
      $(this.selector)?.datepicker('clearDates');
      return;
    }

    $(this.selector)?.datepicker('setDate', value);
  }

  getValue() {
    return $(this.selector)?.datepicker('getDate');
  }

  clear() {
    this.setValue()
  }

  disable() {
    $(this.selector).prop("disabled", true)
  }

  enable() {
    $(this.selector).prop("disabled", false)
  }
}
