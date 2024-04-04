import Swal from 'sweetalert2';

export default class ConfirmationService {
  static confirmDelete({ title = 'Tem certeza?', message,  icon = 'warning', fn}) {
    Swal.fire({
      icon: icon,
      title: title,
      text: message,
      showConfirmButton: true,
      showCancelButton: true,
      confirmButtonColor: "#3085d6",
      cancelButtonColor: "#d33",
      confirmButtonText: "Sim, excluir!",
    }).then((result) => {
      if (result.isConfirmed) {
        fn();
      }
    });
}

}