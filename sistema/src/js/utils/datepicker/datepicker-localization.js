import $ from 'jquery';

const addDatePickerLocalization = (localeCode) => {
    const locales = {
        'pt-BR': {
            format: 'dd/mm/yyyy',
            days: [
                'Domingo',
                'Segunda',
                'Terça',
                'Quarta',
                'Quinta',
                'Sexta',
                'Sábado',
                'Domingo',
            ],
            daysShort: ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'Sáb', 'Dom'],
            daysMin: ['Do', 'Se', 'Te', 'Qu', 'Qu', 'Se', 'Sa', 'Do'],
            months: [
                'Janeiro',
                'Fevereiro',
                'Março',
                'Abril',
                'Maio',
                'Junho',
                'Julho',
                'Agosto',
                'Setembro',
                'Outubro',
                'Novembro',
                'Dezembro',
            ],
            monthsShort: [
                'Jan',
                'Fev',
                'Mar',
                'Abr',
                'Mai',
                'Jun',
                'Jul',
                'Ago',
                'Set',
                'Out',
                'Nov',
                'Dez',
            ],
            today: 'Hoje',
            suffix: [],
            meridiem: [],
        },
        // Add more locale settings as needed
    };

    if (locales[localeCode]) {
        $.fn.datepicker.dates[localeCode] = locales[localeCode];
    } else {
        console.warn(`Locale settings for ${localeCode} not found.`);
    }
};

export default addDatePickerLocalization;