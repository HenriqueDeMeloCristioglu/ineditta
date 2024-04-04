export default class NumberFormatter {
    static percentage(value, decimalPlaces = 2) {
        const formatter = new Intl.NumberFormat('pt-BR', {
            minimumFractionDigits: decimalPlaces,
            maximumFractionDigits: decimalPlaces,
            style: 'percent',

        });
        
        return formatter.format(value  / 100);
    }
}