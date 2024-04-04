using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using Ineditta.Application.AIs.DocumentosSindicais.Dtos;
using Ineditta.BuildingBlocks.Core.Domain.Models;

namespace Ineditta.Application.AIs.DocumentosSindicais.Factories
{
    public static class QuebraClausulaTextoFactory
    {

        public static Result<QuebraClausulaDto, Error> RealizarQuebraTexto(string textoContrato)
        {
            var listaClausulas = new List<ClausulaDto>();

            var textoCorrigido = ManipularTexto(textoContrato);

            textoCorrigido = QuebraClausulaCommonFactory.FormatarTexto(textoCorrigido);

            try
            {
                // Padrão 1
                string padrao1 = @"^(?<clausula>CLÁUSULA|CLAUSULA)(?<espaco1> )?(?<numeroOrdinal>(?<dezena>décima|vigésima|trigésima|quadragésima|quinquagésima|sexagésima|septuagésima|septagésima|octogésima|octagésima|nonagésima|centésima)?(?<espaco2> )?(?<unidade>primeira|segunda|terceira|quarta|quinta|sexta|sétima|oitava|nona)?)(?<espTrEsp> - |- | -|-|: )(?<tituloClausula>.+)";

                // Padrão 2
                string padrao2 = @"^(?<clausula>CLÁUSULA|CLAUSULA)(?<espaco1> +)(?<a>Nº\.)?(?<numeroClausula>[0-9]+)(?<simboloNumeroOrdinal>ª|º)?(?<espTrEsp> - |- | -|-|: |\. )(?<tituloClausula>.+)";

                // Padrão 3
                string padrao3 = @"^(?<clausula>CLÁUSULA)(?<espaco1> )(?<tituloClausula>.+)";

                // Padrão 4
                string padrao4 = @"^(?<numeroClausula>[0-9]+)(?<simboloNumeroOrdinal>ª|º)?(?<espTrEsp> - |- | -|-|: |\. )(?<tituloClausula>.+)";

                string[] padroes = { padrao1, padrao2, padrao3, padrao4 };

                foreach (var padrao in padroes)
                {
                    MatchCollection matches = Regex.Matches(textoCorrigido, padrao, RegexOptions.Multiline | RegexOptions.IgnoreCase);

                    var count = 1;
                    foreach (Match match in matches)
                    {
                        var tituloClausula = RemoverNumeroOrdinal(RemoverCRDoFinal(match.Groups["tituloClausula"].Value.Trim()));
                        var numeroOrdinalClausula = RemoverCRDoFinal(match.Groups["numeroOrdinal"].Value.Trim());
                        var numeroClausula = count;
                        if (!string.IsNullOrEmpty(match.Groups["numeroClausula"].Value))
                        {
                            numeroClausula = int.Parse(match.Groups["numeroClausula"].Value, CultureInfo.InvariantCulture);
                        } else
                        {
                            var numeroClausulaPorDescricao = QuebraClausulaCommonFactory.ObterNumeroClausula(numeroOrdinalClausula);
                            if (numeroClausulaPorDescricao > 0)
                            {
                                numeroClausula = numeroClausulaPorDescricao;
                            }
                        }

                        var clausula = new ClausulaDto(null, null, numeroClausula, tituloClausula, match.NextMatch().Success ? textoCorrigido.Substring(match.Index, match.NextMatch().Index - match.Index).Trim() : textoCorrigido.Substring(match.Index).Trim());

                        listaClausulas.Add(clausula);
                        count++;
                    }

                    if (listaClausulas.Any())
                    {
                        return Result.Success<QuebraClausulaDto, Error>(new QuebraClausulaDto(listaClausulas.Count, listaClausulas, false));
                    }
                }

                return Result.Failure<QuebraClausulaDto, Error>(Errors.General.Business($"Não foi possível identificar nenhum padrão para realizar a quebra das cláusulas"));
            }
            catch (Exception ex)
            {
                return Result.Failure<QuebraClausulaDto, Error>(Errors.General.Business($"Erro ao quebras as cláusulas do HTML: {ex.Message}"));
            }
        }

        private static string ManipularTexto(string textoContrato)
        {
            StringBuilder textoManipulado = new StringBuilder();

            string[] linhas = textoContrato.Split('\n');

            string? linhaAnterior = null;

            foreach (var linha in linhas)
            {
                string linhaAtual = linha.Trim();

                if (string.IsNullOrEmpty(linhaAtual))
                {
                    textoManipulado.AppendLine();

                    continue;
                }

                // Verifica se a linha contém apenas um número (possivelmente número da página)
                if (ContemApenasNumero(linhaAtual))
                {
                    continue;
                }

                // Junto as linhas quando a linha anterior termina com letra mínuscula e a atual começa
                if (linhaAnterior != null)
                {
                    var linhaAtualComecaComLetraMinuscula = UltimaLetraMinuscula(linhaAtual);
                    var linhaAnteriorTerminaComLetraMinuscula = PrimeiraLetraMinuscula(linhaAnterior);

                    if (linhaAnteriorTerminaComLetraMinuscula && linhaAtualComecaComLetraMinuscula)
                    {
                        textoManipulado.Append(linhaAtual);
                        textoManipulado.Append(' ');
                        continue;
                    }
                }

                linhaAnterior = linhaAtual;

                textoManipulado.Append(linhaAtual);

                // Adiciona espaço entre linhas que não terminam com ponto final
                if (QuebraLinha(linhaAtual))
                {
                    // Adiciona quebra de linha no final de cada parágrafo
                    textoManipulado.AppendLine();
                    textoManipulado.AppendLine();
                }
                else
                {
                    // Adiciona espaço para concatenar texto
                    textoManipulado.Append(' ');
                }
            }

            // Remove espaços à esquerda no início de cada parágrafo
            string textoFinal = RemoverEspacosAEsquerda(textoManipulado.ToString());

            return textoFinal;
        }

        private static bool PrimeiraLetraMinuscula(string texto)
        {
            char primeiraLetra = texto.First();
            return char.IsLetter(primeiraLetra) && char.IsLower(primeiraLetra);
        }

        private static bool UltimaLetraMinuscula(string texto)
        {
            char ultimaLetra = texto.Last();
            return char.IsLetter(ultimaLetra) && char.IsLower(ultimaLetra);
        }

        private static bool QuebraLinha(string texto)
        {
            if (texto.EndsWith('.') || texto.EndsWith(';') || texto.EndsWith(':') || texto.Length < 50)
            {
                return true;
            }

            return false;
        }

        private static bool ContemApenasNumero(string texto)
        {
            // Utiliza expressão regular para verificar se a linha contém apenas números
            return Regex.IsMatch(texto, @"^\d+$");
        }

        private static string RemoverEspacosAEsquerda(string texto)
        {
            // Utiliza expressão regular para remover espaços à esquerda no início de cada linha
            return Regex.Replace(texto, @"^(\s*)(.+)", m => m.Groups[1].Value + m.Groups[2].Value.TrimStart(), RegexOptions.Multiline);
        }

        private static string RemoverCRDoFinal(string texto)
        {
            if (texto.EndsWith('\r'))
            {
                return texto.Substring(0, texto.Length - 1);
            }
            else
            {
                return texto;
            }
        }

        private static string RemoverNumeroOrdinal(string texto)
        {
            // Expressão regular para encontrar um numeral ordinal ou escrito por extenso seguido de um traço opcional
            Regex regex = new Regex(@"^\s*(?:(?:\d+|primeira|segunda|terceira|quarta|quinta|sexta|sétima|oitava|nona|décima|vigésima|trigésima|quadragésima|quinquagésima|sexagésima|septuagésima|octogésima|nonagésima|centésima)(?:\s+décima|\s+primeira|\s+segunda|\s+terceira|\s+quarta|\s+quinta|\s+sexta|\s+sétima|\s+oitava|\s+nona|\s+vigésima|\s+trigésima|\s+quadragésima|\s+quinquagésima|\s+sexagésima|\s+septuagésima|\s+octogésima|\s+nonagésima|\s+centésima)?)?(?:ª|º)?\s*(-)?\s*", RegexOptions.IgnoreCase);

            // Verificar se o texto corresponde ao padrão
            Match match = regex.Match(texto);

            if (match.Success)
            {
                // Encontrar o índice do próximo caractere alfabético após o número ordinal
                int indice = match.Index + match.Length;
                while (indice < texto.Length && !char.IsLetter(texto[indice]))
                {
                    indice++;
                }

                // Se encontrou um caractere alfabético, retornar a parte restante da string
                if (indice < texto.Length)
                {
                    return texto.Substring(indice);
                }
                // Se não encontrou nenhum caractere alfabético, retornar string vazia
                else
                {
                    return string.Empty;
                }
            }
            // Se não corresponde ao padrão, retornar a string original
            else
            {
                return texto;
            }
        }
    }
}
