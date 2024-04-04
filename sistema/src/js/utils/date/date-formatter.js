import DateParser from "./date-parser";

export default class DateFormatter {
    static toString(date) {
        return date ? `${date.getDate().toString().padStart(2, '0')}/${(date.getMonth() + 1).toString().padStart(2, '0')}/${date.getFullYear()}` : '';
    }

    static monthYear(date) {
        if (!date) {
            return '';
        }

        if (typeof date === 'string') {
            date = DateParser.fromString(date);
        }

        return date ? `${(date.getMonth() + 1).toString().padStart(2, '0')}/${date.getFullYear()}` : '';
    }

    static dayMonthYear(date) {
        if (!date) {
            return '';
        }

        if (typeof date === 'string') {
            date = DateParser.fromString(date);
        }

        const options = { day: '2-digit', month: '2-digit', year: 'numeric' }
        const data = new Date(date)

        return new Intl.DateTimeFormat('pt-BR', options).format(data)
    }

    static dateTime(dateTime) {
        const data = new Date(dateTime)

        data.setUTCHours(data.getUTCHours())

        const options = {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit',
            hour: '2-digit',
            minute: '2-digit',
            second: '2-digit',
            timeZoneName: 'short',
        };

        const formatoBrasil = new Intl.DateTimeFormat('pt-BR', options)
        const dataHoraFormatada = formatoBrasil.format(data)

        return dataHoraFormatada
    }

    static isDateValid(dateString) {
        if (dateString.length < 8) return

        const dateObject = new Date(dateString)

        return !isNaN(dateObject.getTime()) && dateString.trim() !== ''
    }
}