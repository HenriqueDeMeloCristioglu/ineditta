<?php
session_start();

if (!$_SESSION) {
    echo "<script>document.location.href='http://localhost:8000/index.php'</script>";
}

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
    <meta name="author" content="Ineditta">

    <link href='https://fonts.googleapis.com/css?family=Source+Sans+Pro:300,400,600' rel='stylesheet' type='text/css'>

    <link rel='stylesheet' type='text/css' href='includes/css/msg.css' />

    <!-- Bootstrap 3.3.7 -->
    <link rel="stylesheet" href="anuencia.css">

    <!-- Bootstrap Internal -->
    <link rel="stylesheet" href="includes/css/styles.css">
</head>

<body>
    <?php include('loading_component.php'); ?>
    <style>
        h1 {
            font-size: 2em;
            font-weight: 500;
            text-align: center;
        }

        h2 {
            font-size: 1.5em;
            font-weight: 400;
            margin-left: 2em
        }

        .container {
            max-width: 1300px;
            padding: 50px 0;
        }

        .container p,
        .container p p {
            text-align: justify;
            margin-top: 16px;
        }

        .accept {
            margin-top: 16px;
        }

        .center {
            text-align: center;
        }
    </style>
    <section class="container">
        <h1>
            <span class="center">TERMO DE USO E POLÍTICA DE PRIVACIDADE</span><br>
            <span class="center">SISTEMA INEDITTA e WEBSITE</span>
        </h1>
        <div class="center">
            <span>
                <b><u>Atualizado e publicado em 20/10/2023.</u></b>
            </span>
        </div>


        <p>Este Termo de Uso e Política de Privacidade tem como objetivo, dar ao <b>USUÁRIO</b>, o direito de utilização do <b>SISTEMA INEDITTA</b> (sistema web), de propriedade única e exclusiva da <b>INEDITTA CONSULTORIA SINDICAL E TRABALHISTA LTDA. (INEDITTA)</b>, inscrita no CNPJ/MF sob nº 14.205.409/0001-59, com sede na Cidade de Barueri, Estado de São Paulo, na Alameda Araguaia, 2190 – sala 301, Edifício I, Centro Empresarial Araguaia II, Alphaville Industrial – CEP 06455-000, objeto da prestação de serviços de suporte para o gerenciamento dos Acordos Sindicais e Convenções Coletivas do <b>USUÁRIO</b>, com base no Contrato de Prestação de Serviços e Proposta Técnica e Comercial firmado entre a <b>INEDITTA</b> e o <b>USUÁRIO</b>.</p>
        <p>Este Termo de Uso e Política de Privacidade também tem o objetivo de informar, dar transparência e reforçar o compromisso da <b>INEDITTA</b> com a privacidade e segurança, inclusive das informações e dos dados coletados, produzidos, recebidos, compartilhados, arquivados, eliminados, conforme estabelecido na Lei nº 13.709, de 14 de agosto de 2018, intitulada de Lei Geral de Proteção de Dados Pessoais – LGPD.</p>
        <h2>
            <p>
                <b>1. DEFINIÇÕES</b>
            </p>
        </h2>
        <p>Para os fins deste Termo de Uso e Política de Privacidade, consideram-se:</p>
        <p>a) <b>USUÁRIO/TITULAR</b> – o <b>CONTRATANTE</b>, e todas as pessoas físicas por ele autorizadas, que utilizarem o <b>SISTEMA INEDITTA</b> para consulta às informações ali contidas, bem como toda pessoa natural (física) a quem se referem as informações e os dados pessoais que são objeto de tratamento.</p>
        <p>b) <b>CONTROLADOR</b> – pessoa natural (física) ou jurídica, de direito público ou privado, a quem competem as decisões referentes ao tratamento de dados pessoais, no caso, a <b>INEDITTA</b>.</p>
        <p>c) <b>DADOS PESSOAIS</b> – toda informação relacionada e que identifique a pessoa natural (física).</p>
        <p>d) <b>TERMINAL/TERMINAIS</b> – computadores, <i>notebooks, netbooks, smartphones, tablete, palm tops</i> e quaisquer outros dispositivos que se conectem com a <b>INTERNET</b>. </p>
        <p>e) <b>INTERNET</b> – sistema constituído do conjunto de protocolos lógicos, estruturado em escala mundial para uso público e irrestrito, com a finalidade de possibilitar a comunicação de dados entre <b>TERMINAIS</b> por meio de diferentes redes.</p>
        <p>f) <b>ENDEREÇO DE PROTOCOLO DE INTERNET (ENDEREÇO IP):</b> código atribuído a um <b>TERMINAL</b> de uma rede para permitir a sua identificação, definido segundo parâmetros internacionais.</p>
        <p>g) <b>SENHA:</b> conjunto de caracteres que podem ser constituídos por letras e/ou números, com a finalidade de verificar a identidade do <b>USUÁRIO</b> para acesso ao <b>SISTEMA INEDITTA</b>.</p>
        <p>h) <b><i>SITE OU WEBSITE:</i></b> Website é uma palavra que resulta da justa posição das palavras inglesas web (rede) e site (sítio, lugar). No contexto das comunicações eletrônicas, Website e site possuem o mesmo significado e são utilizadas para fazer referência a uma página ou a um agrupamento de páginas relacionadas entre si, acessíveis na <b>INTERNET</b> através de um determinado endereço.</p>
        <p>i) <b>CONSENTIMENTO:</b> significa a concordância e anuência do <b>USUÁRIO/TITULAR</b> aos termos e condições deste instrumento, inclusive mediante a alteração da primeira senha fornecida pela <b>INEDITTA</b>.</p>
        <p>j) <b><i>BROWSER:</i></b> é qualquer programa de navegação na <b>INTERNET</b>, mediante o qual o <b>USUÁRIO/TITULAR</b> visualiza a programação contida em uma página.</p>
        <p>k) <b><i>LINK:</i></b> programação eletrônica que disponibiliza acesso eletrônico, simbolizada por imagens ou palavras. Este acesso eletrônico direciona o <b><i>BROWSER</i></b> do <b>USUÁRIO/TITULAR</b> para outra página do <b><i>SITE</i></b> visualizado pelo <b>USUÁRIO/TITULAR</b>, ou ainda, a uma página de outro <b><i>SITE</i></b>, com o fim de conectar o <b><i>BROWSER</i></b> a tal página, permitindo que o <b>USUÁRIO/TITULAR</b> a visualize.</p>
        <p>l) <b><i>LAYOUT:</i></b> desenho, plano, esquema, forma ou design do <b><i>SITE</i></b>, ou de quaisquer sites e ferramentas utilizadas pela <b>INEDITTA</b>, para gestão e organização do <b>SISTEMA INEDITTA</b>.</p>
        <p>m) <b><i>COOKIES:</i></b> pequenos arquivos criados por sites visitados que são salvos no computador do usuário por meio do navegador. Esses arquivos contêm informações que servem para identificar o visitante, seja para personalizar a página de acordo com o perfil ou para facilitar o transporte de dados entre as páginas de um mesmo site.</p>
        <p>n) <b>SISTEMA INEDITTA:</b> sistema web, seus sites e ferramentas, disponibilizados pela <b>INEDITTA</b> aos <b>USUÁRIOS/TITULARES</b> para permitir a prestação dos serviços.</p>
        <p>o) <b>TERRITÓRIO:</b> qualquer local ou localidade, no território nacional brasileiro, no qual a <b>INEDITTA</b> disponibilizará os serviços.</p>
        <p>p) <b>TRATAMENTO:</b> toda operação realizada com dados pessoais, como as que se referem a coleta, produção, recepção, classificação, utilização, acesso, reprodução, transmissão, distribuição, processamento, arquivamento, armazenamento, eliminação, avaliação ou controle de informação, modificação, comunicação, transferência, difusão ou extração.</p>
        <p>q) <b>ANONIMIZAÇÃO:</b> utilização de meios técnicos razoáveis e disponíveis no momento do tratamento, por meio dos quais um dado perde a possibilidade de associação, direta ou indireta, a um indivíduo.</p>
        <p>Os termos que não estão acima relacionados, porém que tenham significado técnico usualmente aplicados no mercado, ou que no decorrer da relação contratual venham a ser utilizados nos usos e costumes comerciais, inclusive aqueles grafados em idioma estrangeiro, deverão ser compreendidos e interpretados em consonância com o conceito internacionalmente consagrados, no que conflitarem com as definições aqui convencionadas.</p>
        <h2>
            <p>
                <b>2. POLÍTICA DE PRIVACIDADE</b>
            </p>
        </h2>
        <p>A segurança dos <b>USUÁRIOS</b> é de extrema importância para a <b>INEDITTA</b>. Os dados pessoais necessários ao acesso do <b>SISTEMA INEDITTA</b> não são divulgados a terceiros, a não ser no cumprimento de ordens emitidas por autoridades públicas. As informações dos <b>USUÁRIOS/TITULARES</b> inseridas no <b>SISTEMA INEDITTA</b> são codificadas e criptografadas, não podendo ser lidas por terceiros. O presente Termo e Política de Privacidade obedece aos preceitos definidos na legislação aplicável, bem como na Lei nº 13.709, de 14 de agosto de 2018, intitulada como Lei Geral de Proteção de Dados Pessoais (LGPD).</p>
        <h2>
            <p>
                <b>3. IDENTIFICAÇÃO DE ACESSO</b>
            </p>
        </h2>
        <p>A <b>CONTRATANTE</b> enviará à <b>INEDITTA</b>, por e-mail, ou via formulário de cadastro disponível no sistema, o nome completo, o número do celular com o DDD, o número do telefone comercial com o DDD, o departamento, a função, o endereço de e-mail de todos os <b>USUÁRIOS/TITULARES</b> que terão acesso ao <b>SISTEMA INEDITTA</b>. </p>
        <p>A <b>CONTRATANTE</b>, através de seus <b>USUÁRIOS/TITULARES</b>, aceitará, de forma eletrônica, o presente Termo, e deverá reforçar com os seus <b>USUÁRIOS/TITULARES</b>, a responsabilidade pela confidencialidade da senha e pelo uso do <b>SISTEMA INEDITTA</b>.</p>
        <p>Em caso de desligamento de qualquer <b>USUÁRIO/TITULAR</b>, a <b>CONTRATANTE</b> se obriga a comunicar à <b>INEDITTA</b>, imediatamente e por e-mail, para que a <b>INEDITTA</b> providencie a exclusão do referido <b>USUÁRIO/TITULAR</b>.</p>
        <h2>
            <p>
                <b>4. CONCORDÂNCIA DOS TERMOS DE USO E POLÍTICA DE PRIVACIDADE</b>
            </p>
        </h2>
        <p>O cadastro dos <b>USUÁRIOS/TITULARES</b> será feito pela <b>INEDITTA</b>, que dará acesso ao <b>SISTEMA INEDITTA</b> aos <b>USUÁRIOS/TITULARES da CONTRATANTE</b>, os quais deverão alterar a senha inicialmente fornecida pela <b>INEDITTA</b>. Para que a nova senha seja definitivamente cadastrada, os <b>USUÁRIOS/TITULARES</b> deverão dar o aceite aos Termos de Uso e Política de Privacidade, em campo específico existente no <b>SISTEMA INEDITTA</b>.</p>
        <h2>
            <p>
                <b>5. DADOS E INFORMAÇÕES</b>
            </p>
        </h2>
        <p>Após concordância dos termos do presente instrumento, mediante aceite em campo específico do <b>SISTEMA INEDITTA</b>, e da permissão de acesso pela <b>INEDITTA</b>, os <b>USUÁRIOS/TITULARES</b> poderão analisar e consultar as informações e documentos das Convenções Coletivas e Acordos Sindicais, inseridas pela <b>INEDITTA</b> no <b>SISTEMA INEDITTA</b>, ficando proibida a <b>CONTRATANTE</b> e os <b>USUÁRIOS/TITULARES</b>, de copiar, publicar, distribuir, transmitir, exibir tais documentos e registros em qualquer <b>TERMINAL</b> ou em qualquer mídia, exceto se para uso interno da <b>CONTRATANTE</b>. </p>
        <p>O <b>SISTEMA INEDITTA</b> disponibiliza à <b>CONTRATANTE</b> e a seus <b>USUÁRIOS/TITULARES</b>, a integra das Convenções Coletivas de Trabalho e Acordos Sindicais, denominados arquivos oficiais. Além disso, a <b>INEDITTA</b> oferece na funcionalidade denominada “MAPA SINDICAL”, a conversão de cláusulas resumidas das Convenções Coletivas e Acordos Sindicais, em formato Excel, o comparativo de informações e o formulário de aplicação instrumento coletivo, e na funcionalidade “CALENDÁRIO SINDICAL”, as obrigações previstas nas Convenções Coletivas e Acordos Sindicais, em formato de calendário com ações a serem observadas pela <b>CONTRATANTE</b> e <b>USUÁRIOS</b>, ao longo do período de vigência das Convenções Coletivas e Acordos Sindicais. A <b>CONTRATANTE</b> e seus <b>USUÁRIOS/TITULARES</b>, concordam expressamente que tais informações correspondem ao entendimento da <b>INEDITTA</b>, e que tais entendimentos não devem se sobrepor à análise interna por parte da <b>CONTRATANTE</b>, do conjunto de informações e documentos disponíveis no sistema, para a tomada de decisões.</p>
        <p>A <b>INEDITTA</b> não se responsabiliza pelas informações inseridas pelos usuários no sistema, principalmente com relação aos preenchimentos das funcionalidades: comentários no módulo cláusulas, comentários no módulo sindicatos e criação de evento no módulo calendário sindical, não assumindo qualquer função fiscalizadora e verificadora.</p>


        <p>A <b>CONTRATANTE</b> e seus <b>USUÁRIOS/TITULARES</b> concordam em:
        <p>
        <p>a) Não divulgar, sob qualquer forma ou meio, os dados de código de acesso, usuário e senha disponibilizados pela <b>INEDITTA</b>, ou que forem por eles alterados, inclusive para outros funcionários, prepostos, terceiros, procuradores ou qualquer outra pessoa que tenha ou que venha a estabelecer qualquer vínculo com a <b>CONTRATANTE</b>;</p>
        <p>b) Não divulgar quaisquer dados individualizados de informações disponibilizadas no <b>SISTEMA INEDITTA</b>;</p>
        <p>c) Guardar sigilo e zelar pela privacidade das pessoas e fatos a ela relacionadas que estejam disponibilizadas no <b>SISTEMA INEDITTA</b>, não divulgando, por quaisquer meios, dados ou informações contendo nomes ou quaisquer outras variáveis que permitam a identificação de indivíduos ou afetem a sua privacidade;</p>
        <p>d) Guardar sigilo sobre as credenciais de autenticação (código de acesso, usuário e senha) fornecidas para acesso ao <b>SISTEMA INEDITTA</b>;</p>
        <p>e) Não repassar, compartilhar, comercializar ou transferir a terceiros as informações individualizadas em sua integridade ou qualquer parte, objeto deste Termo, que viole o seu sigilo.</p>
        <p>f) Não disponibilizar, emprestar ou permitir acesso de pessoas ou instituições não autorizadas ao <b>SISTEMA INEDITTA</b>;</p>
        <p>g) Não praticar ou permitir qualquer ação que comprometa a integridade do <b>SISTEMA INEDITTA</b>;</p>
        <p>h) Certificar que a sessão aberta em qualquer terminal (computadores, notebooks, netbooks, smartphones, tablete, palm tops e quaisquer outros dispositivos que se conectem com a internet) foi finalizada e fechada, quando do término das atividades ou afastamento do referido terminal, e que para reaver o acesso ao <b>SISTEMA INEDITTA</b>, será necessária nova autenticação;</p>
        <p>i) Alterar a senha, sempre que obrigatório ou que tenha suposição de descoberta por terceiros, não usando combinações simples que possam ser facilmente descobertas;</p>
        <p>j) Observar e cumprir as boas práticas de segurança da informação, e suas diretrizes, bem como este termo;</p>
        <p>k) Responder em todas as instâncias, e se responsabilizar pelas consequências das ações ou omissões de nossa parte, que possam pôr em risco ou comprometer a exclusividade de conhecimento da senha;</p>
        <p>l) Responder em todas as instâncias, e responsabilizar-se pelas consequências das ações ou omissões a que derem causa.</p>

        <p>Algumas informações e dados dos <b>USUÁRIOS/TITULARES</b> são coletados em decorrência da navegação em nosso website, tais como características do dispositivo de acesso, do navegador, do endereço de IP (Protocolo de Internet), data e hora de acesso, da origem do IP (Protocolo de Internet).</p>

        <p>A <b>INEDITTA</b> utiliza <i>cookies</i> para armazenar informações sobre a visita ao <i>website</i> Institucional, como a confirmação de leitura da Política de Privacidade e Termo de Consentimento para Tratamento de Dados e informações usadas na análise de estatísticas de acesso pelo Google Analytics (ferramenta de análise do Google). A <b>INEDITTA</b> não utiliza cookies para coletar dados pessoais do Usuário.</p>
        <h2>
            <p>
                <b>6. LINKS DE TERCEIROS</b>
            </p>
        </h2>
        <p>O <b>WEBSITE</b> e o <b>SISTEMA INEDITTA</b> mantém links para websites de terceiros. <b>A CONTRATANTE</b> e os <b>USUÁRIOS/TITULARES</b> estão cientes e concordam que a existência desses <i>links</i> não constitui endosso ou patrocínio de <i>websites</i> de terceiro, e reconhece estar sujeita aos termos de uso e políticas de privacidade dos respectivos <i>websites</i>, bem como, que a <b>INEDITTA</b> não se responsabiliza pelo conteúdo contido nas respectivas políticas de privacidade e nos links, nem por quaisquer alterações ou atualizações desses sistemas.</p>

        <h2>
            <p>
                <b>7. FINALIDADE DAS INFORMAÇÕES E DADOS</b>
            </p>
        </h2>
        <p>As informações e os dados coletados e recebidos estão diretamente relacionados ao fornecimento e à prestação de serviços pela <b>INEDITTA</b>, à promoção e divulgação de seus serviços, ao contato com o <b>USUÁRIO/TITULAR</b>, à manutenção de banco de dados, à customização e melhoria de performance da navegabilidade, assim como para o acompanhamento no nível de utilização do <b>USUÁRIO/TITULAR</b>.</p>
        <h2>
            <p>
                <b>8. COMPARTILHAMENTO DOS DADOS E DAS INFORMAÇÕES</b>
            </p>
        </h2>
        <p>A <b>INEDITTA</b> poderá, nos limites do necessário, compartilhar as informações e os dados coletados e recebidos com seus funcionários e equipe de trabalho, com eventuais parceiros que necessitem atuar conjuntamente para a execução dos serviços que presta ou para cumprir as finalidades descritas no item 7 desta política, em transações societárias, com parceiros de combate à fraude, com auditorias externas, com autoridades públicas ou órgãos oficiais, para a proteção da <b>INEDITTA</b> em qualquer tipo de conflito, inclusive ações judiciais.</p>
        <h2>
            <p>
                <b>9. DIREITOS AUTORAIS</b>
            </p>
        </h2>
        <p>O <b>SISTEMA INEDITTA</b> é de propriedade única e exclusiva da <b>INEDITTA</b>, assim como qualquer software e produtos desenvolvidos por ela, os quais encontram-se devidamente registrados e protegidos pela Lei. Qualquer violação por parte da <b>CONTRATANTE</b> e/ou de seus <b>USUÁRIOS/TITULARES</b>, acarretará a imediata rescisão do Contrato de Prestação de Serviços, suspensão imediata do acesso ao <b>SISTEMA INEDITTA</b>, bem como, pagamento de indenização pelos prejuízos que venham a ser causados à <b>INEDITTA</b> e ou a terceiros, sem prejuízo de medidas judiciais, se for o caso.</p>
        <h2>
            <p>
                <b>10. ARMAZENAMENTO DAS INFORMAÇÕES E DOS DADOS</b>
            </p>
        </h2>
        As informações e os dados coletados e recebidos serão mantidos e armazenados para o estrito cumprimento das finalidades do tratamento, bem como para o cumprimento de obrigações legais, regulatória, ou para o regular exercício do direito da <b>INEDITTA</b>, em servidores próprios ou por ela contratados.</p>
        <h2>
            <p>
                <b>11. EXCLUSÃO DOS DADOS E DAS INFORMAÇÕES</b>
            </p>
        </h2>
        <p>O <b>USUÁRIO/TITULAR</b> poderá solicitar a exclusão ou anonimização de seus dados e informações, através de solicitação expressa pelo e-mail suporte@ineditta.com.br. No entanto, mesmo após o pedido de exclusão, é possível que alguns dados e informações permaneçam armazenados, em razão de obrigações legais ou para a proteção de interesses da <b>INEDITTA</b> ou de terceiros.</p>
        <p>A revogação do consentimento ou quaisquer outras informações pelo <b>USUÁRIO/TITULAR</b> dos dados serão atendidas mediante solicitação expressa pelo e-mail suporte@ineditta.com.br.</p>
        <h2>
            <p>
                <b>12. ALTERAÇÕES E ATUALIZAÇÕES NO TERMO DE USO DE USO E POLÍTICA DE PRIVACIDADE</b>
            </p>
        </h2>
        <p>A <b>INEDITTA</b> se reserva ao direito de alterar, ajustar e atualizar, a qualquer tempo e sempre que necessário, as cláusulas e condições previstas neste Instrumento. Por esta razão, é importante que a <b>CONTRATANTE</b> e os <b>USUÁRIOS/TITULARES</b> tenham ciência de que a última atualização desta Política constará sempre ao final do documento, sendo recomendável que se mantenham atentos às informações prestadas e constantes no website e no <b>SISTEMA INEDITTA</b>. A partir da publicação das alterações no <b>SISTEMA INEDITTA</b>, e no momento do primeiro acesso, a <b>CONTRATANTE</b>, através de seus <b>USUÁRIOS/TITULARES</b>, deverá dar novo aceite no <b>SISTEMA INEDITTA</b>.
        <h2>
            </p>

            <b>13. FALHAS NO SISTEMA</b></p>
        </h2>
        <p>A <b>CONTRATANTE</b> e os <b>USUÁRIOS/TITULARES</b>, concordam que a <b>INEDITTA</b> não se responsabiliza por qualquer dano, prejuízo ou perda de equipamento <b>(TERMINAL)</b> dos <b>USUÁRIOS/TITULARES</b>, causados por eventuais falhas no <b>SISTEMA INEDITTA</b> ou na <b>INTERNET</b>, decorrente da conduta de terceiros, por danos decorrentes de ataques de vírus ao equipamento dos <b>USUÁRIOS/TITULARES</b> em decorrência de acesso, utilização ou navegação no <b>SISTEMA INEDITTA</b> ou na <b>INTERNET</b>, ou como transferência de dados, arquivos, imagens, textos ou áudios, bem como, por danos suportados em virtude de prejuízos resultantes de dificuldades técnicas ou falhas no <b>SISTEMA INEDITTA</b> ou na <b>INTERNET</b>.</p>
        <h2>
            <p>
                <b>14. SANÇÕES</b>
            </p>
        </h2>
        <p>Sem prejuízo de outras medidas, a <b>INEDITTA</b> poderá advertir, suspender ou cancelar, temporária ou definitivamente o acesso dos <b>USUÁRIOS/TITULARES</b>, ou aplicar sanção que impacte negativamente na utilização da infraestrutura tecnológica direcionada ao uso do <b>SISTEMA INEDITTA</b>, iniciando as ações legais cabíveis, responsabilizando inclusive pelo pagamento de perdas e danos pelos prejuízos a que derem causa, e/ou suspendendo a sua utilização se: (i) o <b>USUÁRIO/TITULAR</b> não cumprir qualquer dispositivo deste Termo; (ii) descumprir com seus deveres de <b>USUÁRIO/TITULAR</b>, (iii) praticar atos fraudulentos ou dolosos; (iv) não puder ser verificada a identidade do <b>USUÁRIO/TITULAR</b> ou quando qualquer informação fornecida por ele estiver incorreta; (v) a <b>INEDITTA</b> entender que o acesso ou qualquer atitude do <b>USUÁRIO/TITULAR</b> tenha causado algum dano à terceiro, à própria <b>INEDITTA</b>, ou ao <b>SISTEMA INEDITTA</b>.</p>
        <h2>
            <p>
                <b>15. SEGURANÇA</b>
            </p>
        </h2>
        <p>A <b>INEDITTA</b> adota políticas, mecanismos e procedimentos de segurança existentes no mercado para proteger os dados e informações pessoais de acesso não autorizados e de eventos acidentais ou ilícitos de destruição, perda, alteração, comunicação ou difusão.</p>


        <p>No entanto, em razão da própria natureza da internet, a segurança não pode ser irrestritamente assegurada contra todas as ameaças existentes no âmbito virtual. A <b>INEDITTA</b> tem o compromisso de adotar e manter as medidas de segurança disponíveis para prevenir incidentes, dentro de sua capacidade.</p>

        <p>A <b>INEDITTA</b> também exerce forte conscientização em sua equipe, funcionários, colaboradores e parceiros, sobre a importância de proteger as informações e os dados, através de treinamentos, atualizações, práticas, políticas, fluxos de controle, dever de confidencialidade e responsabilidades.</p>
        <h2>
            <p>
                <b>16. TOLERÂNCIA</b>
            </p>
        </h2>
        <p>A tolerância quanto ao eventual descumprimento de qualquer das disposições deste Termo de Uso e Política de Privacidade pela CONTRATANTE, ou por seus USUÁRIOS/TITULARES, não constituirá a renúncia ao direito de exigir o cumprimento da obrigação, nem perdão, nem alteração do que consta aqui previsto.</p>
        <h2>
            <p>
                <b>17. LEGISLAÇÃO E FORO</b>
            </p>
        </h2>
        <p>O presente Termo de Uso e Política de Privacidade é redigido de acordo com a legislação brasileira. Quaisquer disputas ou controvérsias oriundas de quaisquer atos praticados no âmbito da utilização do <b>SISTEMA INEDITTA</b> ou <b><i>SITE</i></b> pelos <b>USUÁRIOS/TITULARES</b>, inclusive com relação ao descumprimento do presente Termo, ou pela violação dos direitos da <b>INEDITTA</b>, e/ou de terceiros, inclusive de propriedade intelectual, de sigilo e de personalidade, serão processadas na Comarca de Barueri, no Estado de São Paulo.</p>
        <p>O aceite e consentimento do <b>USUÁRIO/TITULAR</b> é feito neste ato, eletronicamente, de forma livre, informada e inequívoca, nos termos da legislação aplicável e da Lei nº 13.709 (LGPD).</p>
        <br>
        <br>
        <p>
            <b>Última atualização e publicação: 20/10/2023.</b>
        </p>
        <button class="accept btn btn-primary" id="aceitarBtn">LI E ESTOU DE ACORDO COM OS TERMOS</button>
    </section>

    <script type='text/javascript' src="./js/anuencia.min.js"></script>
    <script type='text/javascript' src='./js/core.min.js'></script>
</body>

</html>