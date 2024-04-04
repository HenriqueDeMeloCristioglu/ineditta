<!DOCTYPE html>
<html lang="pt-br">

<head>
    <meta charset="UTF-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Authenticator</title>
</head>

<body class="horizontal-nav">
    <style>
        .container {
            width: 100vw;
            height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
        }
    </style>

    <form id="form" action="autenthicator.php" method="post">
        <input name="perfil" type="hidden" class="form-control" id="camp_user">
    </form>


    <div class="container">
        <img src="/assets/images/loading.svg">
    </div>




    <script type="text/javascript">
        function submitform() {
            document.querySelector('#form').submit();
        }

        setTimeout(() => {

            submitform()

        }, 1000)
    </script>

    <script type='text/javascript' src='./js/core.min.js'></script>
    <script type='text/javascript' src='./js/authenticator.min.js'></script>
</body>

</html>