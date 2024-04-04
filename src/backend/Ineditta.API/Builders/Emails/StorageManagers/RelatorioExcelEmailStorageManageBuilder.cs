using Ineditta.API.Builders.Worksheets;
using Ineditta.Application.Emails.StoragesManagers.Entities;

namespace Ineditta.API.Builders.Emails.StorageManagers
{
    public static class RelatorioExcelEmailStorageManageBuilder
    {
        public static byte[] Criar(IEnumerable<EmailStorageManager> emails)
        {
            using var wb = WorksheetBuilder.Create();

            var bytes = wb.AddWorkSheet("Busca Rápida")
                .Build(ws =>
                {
                    ws.SetDefaultStyles(30);

                    var headers = ObterColunasPadroes();
                    for (int i = 0; i < headers.Count; i++)
                    {
                        var cell = ws.Cell(1, (i + 1)).SetValue(headers[i]);
                        cell.StyleHeader();
                    }

                    ws.Column(3).Width = 80;
                })
                .Build(ws =>
                {
                    var linhaAtual = 2;

                    foreach (var email in emails)
                    {
                        ws.Cell(linhaAtual, 1).Value = email.From.Valor;

                        ws.Cell(linhaAtual, 2).Value = email.To.Valor;

                        ws.Cell(linhaAtual, 3).Value = email.Assunto;

                        ws.Cell(linhaAtual, 4).Value = email.Enviado ? "Sim" : "Não";

                        ws.Cell(linhaAtual, 5).Value = email.DataInclusao;

                        linhaAtual++;
                    }
                })
                .ToByteArray(wb);

            return bytes;
        }

        private static List<string> ObterColunasPadroes()
        {
            return new List<string>
            {
                "De",
                "Para",
                "Assunto",
                "E-mail recebido",
                "Data do envio"
            };
        }
    }
}
