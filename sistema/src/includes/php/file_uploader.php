<?php

/**
 * @author    {Lucas Alcantara Rodrigues}
 * @package   {2.0.0}
 * @description	{Função para upload de arquivos.}
 * @historic {
		2022-12-12 15:37 ( v2.0.0 ) -
	}
 **/

// Montando o código do erro que será apresentado
$localizar  = array(strtolower(__DIR__), "/", "\\", ".php", ".");
$substituir = array("", "", "", "", "-");
$error_code = strtoupper(str_replace($localizar, $substituir,  strtolower(__FILE__))) . "-";

// Declarando os caminhos principais do sistema.
$localizar     = array("\\", "/includes/php");
$substituir    = array("/", "");
// $path 		= str_replace( $localizar, $substituir, __DIR__ . "/storage");//localhost para teste
// $path = __DIR__ . "/../../../documentos_sistema";
$path = "/srv/ineditta";

$response["response_status"]['status'] = 1;
$response["response_status"]["msg"] = "Upload iniciado!";
$request = $_REQUEST;

define("MIME_TYPE_ACCEPTED", [
    "pdf" => "application/pdf",
    "doc" => "application/msword",
    "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
    "xls" => "application/vnd.ms-excel",
    "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
]);

function preparePdfName($pdfName)
{

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

    $name = preg_replace($pattern, explode(" ", "a A e E i I o O u U n N c C"), $name);

    $name = str_replace(['----', '---', '--'], '-', str_replace(['.', ',', '/', '?', '!', '(', ')'], "-", implode("-", explode(" ", $name))));

    return $name;
}

if (key_exists("file_docsind", $_FILES)) { //origem docsind.js
    $file = "file_docsind";
    $subPath = "arquivos_helpdesk";
    $principalPath = "/{$subPath}";
} else if (key_exists("file_documentos", $_FILES)) { //origem documentos.js
    $file = "file_documentos";
    $subPath = "arquivos_helpdesk";
    $principalPath = "/{$subPath}";
} else if (key_exists("file_helpdesk", $_FILES)) { //origem helpdesk.js
    $file = "file_helpdesk";
    $subPath = "arquivos_helpdesk";
    $principalPath = "/{$subPath}";
} else if (key_exists("file_documentos_sindicais", $_FILES)) { //origem documentos_sindicais
    $file = "file_documentos_sindicais";
    $subPath = "documentos/documentos_sindicais";
    $principalPath = "/{$subPath}";
}else {
    $file = "file";
    $subPath = "documentos";
    $principalPath = "/{$subPath}";
}

$fullFilePath = "{$path}{$principalPath}";

if (isset($_FILES[$file]) && !empty($_FILES[$file]['name'])) {

    if (!in_array($_FILES[$file]['type'], MIME_TYPE_ACCEPTED)) {
        $response["response_status"]['status'] = 0;
        $response["response_status"]['msg'] = "Tipo de arquivo não suportado!";
    } else if ($_FILES[$file]['size'] > 5242880) {
        $response["response_status"]['status'] = 0;
        $response["response_status"]['msg'] = "Tamanho do arquivo maior que 5MB!";
    }

    $fileName = preparePdfName($_FILES[$file]['name']);
    $filePath = "";
    if ($_FILES[$file]['type'] == 'application/pdf') {
        $filePath = $path . $principalPath . "/" . $fileName . ".pdf";
        $finalName = $fileName . ".pdf";
    } else if ($_FILES[$file]['type'] == 'application/msword') {
        $filePath = $path . $principalPath . "/" . $fileName . ".doc";
        $finalName = $fileName . ".doc";
    } else if ($_FILES[$file]['type'] == 'application/vnd.ms-excel') {
        $filePath = $path . $principalPath . "/" . $fileName . ".xls";
        $finalName = $fileName . ".xls";
    } else if ($_FILES[$file]['type'] == 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet') {
        $filePath = $path . $principalPath . "/" . $fileName . ".xlsx";
        $finalName = $fileName . ".xlsx";
    } else if ($_FILES[$file]['type'] == 'application/vnd.openxmlformats-officedocument.wordprocessingml.document') {
        $filePath = $path . $principalPath . "/" . $fileName . ".docx";
        $finalName = $fileName . ".docx";
    }

    if ($filePath != "" && $response["response_status"]['status'] != 0) {
        $moved = move_uploaded_file($_FILES[$file]['tmp_name'], $filePath);

        if ($moved) {
            $data = ["file_name" => $finalName, "path" => $filePath];

            $response["response_status"]['status'] = 1;
            $response["response_status"]['msg'] = "Upload realizado com sucesso!";
            $response['response_data'] = $data;
        }else {
            $response["response_status"]['status'] = 0;
            $response["response_status"]['msg'] = "Falha ao realizar o upload!";
            $response['response_data']['path'] = $filePath;
            $response['response_data']['file_name'] = $finalName;
        }

    } else {
        $response["response_status"]['status'] = 0;
    }
} else {
    $response["response_status"]['status'] = 0;
    $response["response_status"]['msg'] = "Não foi possível localizar o arquivo, favor verificar a seleção e/ou o nome do documento!";
}

print json_encode($response);
