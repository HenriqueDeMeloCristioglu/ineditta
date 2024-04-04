<?php

// Include the random string file
require 'rand.php';

// Begin a new session
session_start();
if (!$_SESSION) {echo "<script>document.location.href='http://localhost:8080/index.php'</script>";}

// Set the session contents
$_SESSION['captcha_id'] = $str;
