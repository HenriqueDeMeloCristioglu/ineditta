<?php
session_start();

$userData = json_decode($_POST['perfil']);

if ($userData == null || $userData == "") {
    echo "
        <script>
            document.location.href='http://localhost:8000/auth/realms/Ineditta-prod/protocol/openid-connect/auth?client_id=logineditta&redirect_uri=http://localhost:8000/&state=20c23884-4d52-466d-a625-371de75281e9&response_mode=fragment&response_type=code&scope=openid&nonce=a54383db-4024-47dd-8c79-b03e602761a0'
        </script>
    ";
    die();
}

$_SESSION['login'] = $userData->email;

$userData = $_SESSION['login'];

include_once __DIR__ . "/includes/php/class.usuario.php";

$user = new usuario();

$usuario = $user->validateUser($userData)['response_data']['user'];


//bloqueado
$today = (new DateTime('now'))->format('Y-m-d');
$start = $usuario->ausencia_inicio != "00/00/0000" ? (new DateTime($usuario->ausencia_inicio))->format('Y-m-d') : "00/00/0000";
$end = $usuario->ausencia_fim != "00/00/0000" ? (new DateTime($usuario->ausencia_fim))->format('Y-m-d') : "00/00/0000";

if ($start == "00/00/0000" && $end == "00/00/0000") {
    $block = 0;
}

if ($start != "00/00/0000") {
    $block = $today >= $start && $today <= $end ? 1 : 0;
}

if ($usuario->id_grupoecon == 0) {
    $_SESSION['grupoecon'] = "cliente_grupo_id_grupo_economico";
} else {
    $_SESSION['grupoecon'] = $usuario->id_grupoecon;
}

$_SESSION['tipo'] = $usuario->tipo;

$destino = "perfil_grupo_economico.php";

if ($block == 1 || $usuario->is_blocked == 1) {
    $destino = "bloqueado.php";
} else {
    //Anuencia
    if ($user->verificaAnuencia($userData)['response_data']['anuencia'] == "inexistente") {
        $destino = "anuencia.php";
    }
}

echo "
        <script>
            document.location.href='http://localhost:8000/{$destino}'
        </script>
    ";
