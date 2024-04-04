<?php
session_start();

$_SESSION = [];

echo "
        <script>
            document.location.href='http://localhost:8000/auth/realms/Ineditta-prod/protocol/openid-connect/auth?client_id=logineditta&redirect_uri=https%3A%2F%2Fineditta.com%2Fdesenvolvimento%2F&state=20c23884-4d52-466d-a625-371de75281e9&response_mode=fragment&response_type=code&scope=openid&nonce=a54383db-4024-47dd-8c79-b03e602761a0'
        </script>
    ";
