<?php

/**
 * @author    {Lucas A. Rodrigues Volpati}
 * @description	{ Arquivo de funções de apoio }
**/

/************
 * CONSTANTS
 ***********/
define("CONF_DATE_BR", 'd/m/Y');
define("CONF_YEAR", 'Y');
define("CONF_DATE_US", 'Y-m-d');


/******************
 * CPF - CNPJ MASK
 *****************/

function formatCnpjCpf($value)
{
	$cpfLength = 11;
	$cnpjOrCpf = preg_replace("/\D/", '', $value);
	
	if (strlen($cnpjOrCpf) === $cpfLength) {
		return preg_replace("/(\d{3})(\d{3})(\d{3})(\d{2})/", "\$1.\$2.\$3-\$4", $cnpjOrCpf);
	} 
	
	return preg_replace("/(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})/", "\$1.\$2.\$3/\$4-\$5", $cnpjOrCpf);
}

/******************
 * DATE FORMATTING
 *****************/

function date_fmt(string $date = "now"): string 
{
    return (new DateTime($date))->format(CONF_DATE_BR);
}

function getYear($date) {
	return (new DateTime($date))->format(CONF_YEAR);
}

function date_fmt_us(string $date = "now"): string 
{
    return date(CONF_DATE_US, strtotime(implode("-", explode("/", $date))));
}

/******************
 * OTHER MASKS
 *****************/

function formatCnae($value)
{
	
	return preg_replace("/(\d{2})(\d{2})(\d{1})(\d{2})/", "\$1.\$2-\$3-\$4", $value);
	
}

function formatPhoneNumber($phoneNumber) 
{
	// Limpeza
	$phone_number = preg_replace('/\D/', '', $phoneNumber);

	// Format
	$formattedPhoneNumber = sprintf('(%s) %s-%s', substr($phoneNumber, 0, 2), substr($phoneNumber, 2, 4), substr($phoneNumber, 6, 4));

	return $formattedPhoneNumber;
}