using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace Ineditta.Application.AIs.DocumentosSindicais.Factories
{
    public static class QuebraClausulaCommonFactory
    {
        public static string FormatarTexto(string texto)
        {
            if (texto != null)
            {
                // FORMATAR OUTROS CASOS

                // Substituir pontos consecutivos por " - "
                string pattern = @"\.{3,}";
                string replacement = " - ";
                texto = Regex.Replace(texto, pattern, replacement);

                // FORMATAR ESPAÇO

                // Remover nobreakspace por espaço normal
                string pattern2 = " ";
                string replacement2 = " ";
                texto = Regex.Replace(texto, pattern2, replacement2);

                // Substituir tabulação por espaço
                string pattern3 = @"\t";
                string replacement3 = " ";
                texto = Regex.Replace(texto, pattern3, replacement3);

                // Remover espaços duplos
                string pattern4 = @" {1,}";
                string replacement4 = " ";
                texto = Regex.Replace(texto, pattern4, replacement4);

                // FORMATAR QUEBRA DE LINHA

                // Substituir \s para quebra de linha (Correção)
                string pattern5 = @"(\s){2,}";
                string replacement5 = Environment.NewLine + Environment.NewLine;
                texto = Regex.Replace(texto, pattern5, replacement5);

                // Remover espaço no início e final da quebra de linha
                string pattern6 = @"(\r\n) |(\n) ";
                string replacement6 = Environment.NewLine;
                texto = Regex.Replace(texto, pattern6, replacement6);

                string pattern7 = @" (\r\n)| (\n)";
                string replacement7 = Environment.NewLine;
                texto = Regex.Replace(texto, pattern7, replacement7);

                // Substituir sequências de três quebras de linha por duas quebras de linha
                string pattern8 = @"(\r\n){2,}|(\n){2,}";
                string replacement8 = Environment.NewLine + Environment.NewLine;
                texto = Regex.Replace(texto, pattern8, replacement8);

                // Remover quebra de linha no início e no final
                texto = texto.Trim();
            }

            return texto ?? string.Empty;
        }

        public static string ObterNomeClausula(string clausulaCompleta)
        {
            int primeiroIndiceTraco = clausulaCompleta.IndexOf('-');

            if (primeiroIndiceTraco != -1)
            {
                string nomeClausula = clausulaCompleta.Substring(primeiroIndiceTraco + 1).Trim();
                return nomeClausula;
            }
            else
            {
                #pragma warning disable CA2249 // Consider using 'string.Contains' instead of 'string.IndexOf'
                int primeiroIndiceDoisPontos = clausulaCompleta.IndexOf(':');
                #pragma warning restore CA2249 // Consider using 'string.Contains' instead of 'string.IndexOf'

                if (primeiroIndiceDoisPontos != -1)
                {
                    // Extraia o texto após o primeiro traço
                    string nomeClausula = clausulaCompleta.Substring(primeiroIndiceDoisPontos + 1).Trim();
                    return nomeClausula;
                }
                else
                {
                    // Se não houver um traço, retorna a cláusula completa
                    return clausulaCompleta;
                }
            }
        }

        public static int ObterNumeroClausula(string nomeClausula)
        {
            List<string> palavrasNomeClausula = nomeClausula.Split(' ').ToList();

            List<string> oitoPrimeirasPalavras = palavrasNomeClausula.Take(8).ToList();

            // Procura por palavras que indicam centenas (ex: "CENTÉSIMA")
            int centena = ObterValorCentena(oitoPrimeirasPalavras);

            // Procura por palavras que indicam dezenas (ex: "QUADRAGÉSIMA")
            int dezena = ObterValorDezena(oitoPrimeirasPalavras);

            // Procura por palavras que indicam unidades (ex: "PRIMEIRA")
            int unidade = ObterValorUnidade(oitoPrimeirasPalavras);

            // Soma os valores de centena, dezena e unidade
            int numero = centena + dezena + unidade;

            return numero;
        }

        private static int ObterValorCentena(List<string> palavrasNomeClausula)
        {
            List<string> centenas = ["CENTÉSIMA", "DUCENTÉSIMA", "TRICENTÉSIMA", "QUATRICENTÉSIMA", "QUINGENTÉSIMA", "SEISCENTÉSIMA", "SETECENTÉSIMA", "OITOCENTÉSIMA", "NOVECENTÉSIMA"];

            foreach (string palavra in palavrasNomeClausula)
            {
                var centena = centenas.Find(d => d.Equals(palavra, StringComparison.OrdinalIgnoreCase));

                if (centena is not null)
                {
                    return (centenas.IndexOf(centena) + 1) * 100;
                }
            }

            return 0; // Retorna 0 se não encontrar uma centena
        }

        private static int ObterValorDezena(List<string> palavrasNomeClausula)
        {
            List<string> dezenas = ["DÉCIMA", "VIGÉSIMA", "TRIGÉSIMA", "QUADRAGÉSIMA", "QUINQUAGÉSIMA", "SEXAGÉSIMA", "SEPTUAGÉSIMA", "OCTOGÉSIMA", "NONAGÉSIMA"];
            List<string> dezenasOrtografiaIncorreta = ["DÉCIMA", "VIGÉSIMA", "TRIGÉSIMA", "QUADRAGÉSIMA", "QUINQUAGÉSIMA", "SEXAGÉSIMA", "SEPTAGÉSIMA", "OCTAGÉSIMA", "NONAGÉSIMA"];

            foreach (string palavra in palavrasNomeClausula)
            {
                var dezena = dezenas.Find(d => d.Equals(palavra, StringComparison.OrdinalIgnoreCase));

                if (dezena is not null)
                {
                    return (dezenas.IndexOf(dezena) + 1) * 10;
                }

                var dezenaOrtografiaIncorreta = dezenasOrtografiaIncorreta.Find(d => d.Equals(palavra, StringComparison.OrdinalIgnoreCase));

                if (dezenaOrtografiaIncorreta is not null)
                {
                    return (dezenasOrtografiaIncorreta.IndexOf(dezenaOrtografiaIncorreta) + 1) * 10;
                }
            }

            return 0; // Retorna 0 se não encontrar uma dezena
        }

        private static int ObterValorUnidade(List<string> palavrasNomeClausula)
        {
            List<string> unidades = ["PRIMEIRA", "SEGUNDA", "TERCEIRA", "QUARTA", "QUINTA", "SEXTA", "SÉTIMA", "OITAVA", "NONA"];

            foreach (string palavra in palavrasNomeClausula)
            {
                var unidade = unidades.Find(d => d.Equals(palavra, StringComparison.OrdinalIgnoreCase));

                if (unidade is not null)
                {
                    return unidades.IndexOf(unidade) + 1;
                }
            }

            return 0; // Retorna 0 se não encontrar uma unidade
        }
    }
}
