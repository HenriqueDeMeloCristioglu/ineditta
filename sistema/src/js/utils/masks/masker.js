export default class Masker {
    static CNPJ(input) {
        if (!input) {
            return input;
        }
        const numericOnly = input.replace(/\D/g, '');
        const maskedCNPJ = numericOnly.replace(
            /^(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})$/,
            '$1.$2.$3/$4-$5'
        );
        return maskedCNPJ;
    }

    static phone(input) {
        if (!input) {
            return input;
        }

        const numericOnly = input.replace(/\D/g, '');

        // Check if it's a cell phone number (9 digits) or a regular phone number (8 digits)
        const isCellPhone = numericOnly.length === 9;

        const maskedPhone = isCellPhone
            ? numericOnly.replace(/^(\d{2})(\d{5})(\d{4})$/, '($1) $2-$3')
            : numericOnly.replace(/^(\d{2})(\d{4})(\d{4})$/, '($1) $2-$3');

        return maskedPhone;
    }

    static dateTime(input) {
        if (!input) return input

        const dateParts = input.split('T')[0].split('-');
        const timeParts = input.split('T')[1].split(':');

        const year = dateParts[0];
        const month = dateParts[1];
        const day = dateParts[2];
        const hour = timeParts[0];
        const minute = timeParts[1];

        return day + '/' + month + '/' + year + ' ' + hour + ':' + minute;
    }

    static subclasseCNAE(input) {
        if (!input) return input

        input = `${input}`

        return `${input[0]}${input[1]}.${input[2]}${input[3]}-${input[4]}-${input[5]}${input[6]}`
    }
}