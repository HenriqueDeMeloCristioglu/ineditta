import Swal from 'sweetalert2';

export default class NotificationService {
    static success({
        title,
        message,
        position = 'top-end',
        icon = 'success',
        showConfirmButton = false,
        showCancelButton = false,
        confirmButtonText = 'Confirmar',
        confirmButtonColor = '#3085d6',
        cancelButtonColor = '#d33',
        cancelButtonText = 'Cancelar',
        then,
        timer
    }) {
        Swal.fire({
            position,
            icon,
            title,
            text: message,
            showConfirmButton,
            showCancelButton,
            confirmButtonText,
            confirmButtonColor,
            cancelButtonText,
            cancelButtonColor,
            timer: timer == undefined ? 10000 : timer
        }).then(then)
    }

    static error({ title, message, position = 'top-end', icon = 'error', showConfirmButton = false, confirmButtonText = '', timer = 2000 }) {
        Swal.fire({
            position,
            icon,
            title,
            text: message,
            showConfirmButton,
            confirmButtonText,
            timer
        })
    }

    static warning({
        title,
        message,
        position = 'top-end',
        icon = 'warning',
        showConfirmButton = false,
        showCancelButton = false,
        confirmButtonText = '',
        confirmButtonColor = '#3085d6',
        cancelButtonColor = '#d33',
        then,
        timer = 2000
    }) {
        Swal.fire({
            position,
            icon,
            title,
            text: message,
            showConfirmButton,
            showCancelButton,
            confirmButtonText,
            confirmButtonColor,
            cancelButtonColor,
            timer
        }).then(then)
    }

    static info({
        title,
        message,
        position = 'top-end',
        icon = 'info',
        showConfirmButton = false,
        showCancelButton = false,
        confirmButtonText = '',
        confirmButtonColor = '#3085d6',
        cancelButtonColor = '#d33',
        then,
        timer = 2000
    }) {
        Swal.fire({
            position,
            icon,
            title,
            text: message,
            showConfirmButton,
            showCancelButton,
            confirmButtonText,
            confirmButtonColor,
            cancelButtonColor,
            timer
        }).then(then)
    }
}