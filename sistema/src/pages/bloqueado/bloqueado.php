<?php
session_start();

/**
 * @author    {Enter5}
 * @package   {1.0.0}
 * @description	{ }
 * @historic {
		2020-08-28 13:40 ( v1.0.0 ) - 
	}
 **/

?>
<!DOCTYPE html>
<html lang="pt-br">

<head>
    <meta charset="utf-8">
    <title>Ineditta</title>
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta name="description" content="Ineditta">
    <meta name="author" content="The Red Team">

    <link rel="stylesheet" href="includes/css/styles.css">
    <link href='https://fonts.googleapis.com/css?family=Source+Sans+Pro:300,400,600' rel='stylesheet' type='text/css'>
    <link rel="stylesheet" type="text/css" href="https://cdn.datatables.net/1.10.19/css/dataTables.bootstrap4.min.css">
    <link href='includes/demo/variations/default.css' rel='stylesheet' type='text/css' media='all' id='styleswitcher'>
    <link href='includes/demo/variations/default.css' rel='stylesheet' type='text/css' media='all' id='headerswitcher'>
    <link rel="stylesheet" href="includes/plugins/chosen/chosen.min.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css">

    <!-- The following CSS are included as plugins and can be removed if unused-->
    <script src="includes/js/jquery-3.4.1.min.js"></script>
    <link rel='stylesheet' type='text/css' href='includes/plugins/form-toggle/toggles.css' />
    <link rel='stylesheet' type='text/css' href='includes/css/msg.css' />
    <link rel="stylesheet" href="includes/plugins/datagrid/styles/jplist-custom.css">
    <script src="includes/plugins/sweet-alert/all.js"></script>
    <script src="keycloak.js"></script>


<body onload="initKeycloak()" style="display: none;" class="horizontal-nav">
    <script type='text/javascript' src="includes/js/modulos.js"></script>
    <script>

        let timerInterval
        let time = 5
        Swal.fire({
            title: 'Usuário Bloqueado',
            html: 'Você será desconectado em <b>5</b> segundos.',
            timer: 5000,
            timerProgressBar: true,
            showConfirmButton: false,
            didOpen: () => {
                Swal.showLoading()
                const b = Swal.getHtmlContainer().querySelector('b')
                timerInterval = setInterval(() => {
                b.textContent = --time
                }, 1000)
            },
            willClose: () => {
                clearInterval(timerInterval)
            }
            }).then((result) => {
            /* Read more about handling dismissals below */
            if (result.dismiss === Swal.DismissReason.timer) {
                logout()
            }
        })
    </script>
</body>

</html>