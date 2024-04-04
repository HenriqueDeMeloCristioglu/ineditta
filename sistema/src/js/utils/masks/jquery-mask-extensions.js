import $ from 'jquery';
import 'jquery-mask-plugin';
import '../../scripts/jquery-mask'

// Define your mask extension methods
$.fn.extend({
    maskCNPJ() {
        return this.mask('00.000.000/0000-00');
    },
    maskPhone() {
        return this.mask('(00)0000-0000');
    },
    maskCelPhone() {
        return this.mask('(00)0000-00000');
    },
    maskDDD() {
        return this.mask('00');
    },
    maskCEP() {
        return this.mask('00000-000');
    },
    maskDate() {
        return this.mask('00/00/0000');
    },
    maskPercentage() {
        return this.mask('000.00', {
            reverse: true,
            placeholder: '00.00',
        })
    },
    maskPercentageWithSufix() {
        return this.mask('000,00%', {
            reverse: true,
            placeholder: '00,00%',
        })
    },
    maskCustom(mask) {
        return this.mask(mask);
    },
    maskPhoneDDD() {
        return this.mask('(00) 0000-0000');
    },
    maskCodigoSindical() {
        return this.mask('000.000.000.00000-0');
    },
    maskHora() {
        return this.mask('00:00');
    },
    maskMonetario() {
        this.maskMoney({
            prefix: 'R$ ',
            thousands: '.',
            decimal: ',',
            affixesStay: false,
            allowNegative: true
        })
    }
});
