<?php

/**
 * @author    {Enter5}
 * @package   {1.0.0}
 * @description	{ }
 * @historic {
		2022-06-30 16:32 ( v1.0.0 ) -
	}
**/

// Exibir na tela os alertas e erro do PHP? SIM = 1 ou NÃO = 0

date_default_timezone_set('America/Sao_Paulo');

// Montando o código do erro que será apresentado
$localizar  = array( strtolower(__DIR__), "/", "\\", ".php", ".");
$substituir = array( "", "", "", "", "-" );
$error_code = strtoupper( str_replace( $localizar, $substituir,  strtolower( __FILE__  ) ) ) . "-";

// Declarando os caminhos principais do sistema.
// $localizar 	= array( "\\", "/includes/php" );
// $substituir	= array( "/", "" );
// $path 		= str_replace( $localizar, $substituir, "https://ineditta.com/documentos_sistema");
$path = __DIR__ . "/../../../documentos_sistema";

function preparePdfName($pdfName) {

    $name = substr($pdfName, 0, -4) . "-0065" . base64_encode((new DateTime())->format('d-m-Y-H:i:s'));

    $pattern =
        [
            "/(á|à|ã|â|ä)/",
            "/(Á|À|Ã|Â|Ä)/",
            "/(é|è|ê|ë)/",
            "/(É|È|Ê|Ë)/",
            "/(í|ì|î|ï)/",
            "/(Í|Ì|Î|Ï)/",
            "/(ó|ò|õ|ô|ö)/",
            "/(Ó|Ò|Õ|Ô|Ö)/",
            "/(ú|ù|û|ü)/",
            "/(Ú|Ù|Û|Ü)/",
            "/(ñ)/",
            "/(Ñ)/",
            "/(ç)/",
            "/(Ç)/"
        ];

    $name = preg_replace($pattern, explode(" ","a A e E i I o O u U n N c C"),$name);

    $name = str_replace(['----', '---', '--'], '-', str_replace(['.',',','/','?','!', '(', ')'], "-", implode("-", explode(" ", $name))));

    return $name;
}

if (isset($_FILES['file']) && !empty($_FILES['file']['name']) && $_FILES['file']['type'] == "application/pdf") {


    if (!is_dir($path . "/documentos_sindicais")) {
        mkdir($path . "/documentos_sindicais", 0755);
    }

    $fileName = preparePdfName($_FILES['file']['name']);

    $filePath = $path . "/documentos_sindicais/" . $fileName . ".pdf";

    $move_upload_rs = move_uploaded_file($_FILES['file']['tmp_name'], $filePath);

    if($move_upload_rs){
        
        $data = ["file_name" => $fileName . ".pdf", "path" => $filePath, "msg" => "Upload realizado com sucesso!", "status" => 1];
        print json_encode($data);
    }else{
        $data = ["msg" => "Falha ao realizar o upload!", "status" => 0];
        print json_encode($data);
    }

}else {
    die("Upload error!");
}

