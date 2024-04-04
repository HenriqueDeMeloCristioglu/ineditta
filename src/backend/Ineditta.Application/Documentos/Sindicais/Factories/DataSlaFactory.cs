namespace Ineditta.Application.Documentos.Sindicais.Factories
{
    public static class DataSlaFactory
    {
        public static DateOnly Criar(int? menorDataCorte)
        {
            var dataSla = DateTime.Today.AddDays(7);

            if (menorDataCorte is null || menorDataCorte == 0) return DateOnly.FromDateTime(dataSla);

            if (DateTime.Today.Month == dataSla.Month && DateTime.Today.Day < dataSla.Day && dataSla.Day < menorDataCorte)
            {
#pragma warning disable S6562
                var diasNoMes = DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);
                if (diasNoMes >= (int)menorDataCorte!)
                {
                    dataSla = new DateTime(dataSla.Year, dataSla.Month, (int)menorDataCorte!);
                }
                else
                {
                    dataSla = new DateTime(dataSla.Year, dataSla.Month, diasNoMes);
                }

                return DateOnly.FromDateTime(dataSla);

            }

            if (DateTime.Today.Month < dataSla.Month)
            {
                var diasNoMesAtual = DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);

                if (DateTime.Today.Day <= menorDataCorte)
                {
                    if (diasNoMesAtual >= menorDataCorte)
                    {
                        dataSla = new DateTime(DateTime.Today.Year, DateTime.Today.Month, (int)menorDataCorte!);
                    }
                    else
                    {
                        dataSla = new DateTime(DateTime.Today.Year, DateTime.Today.Month, diasNoMesAtual);
                    }
                    return DateOnly.FromDateTime(dataSla);
                }
                else
                {
                    if (dataSla.Day > menorDataCorte)
                    {
                        if (diasNoMesAtual >= menorDataCorte)
                        {
                            dataSla = new DateTime(dataSla.Year, dataSla.Month, (int)menorDataCorte!);
                        }
                        else
                        {
                            dataSla = new DateTime(dataSla.Year, dataSla.Month, diasNoMesAtual);
                        }
                    }
                    return DateOnly.FromDateTime(dataSla);
                }
            }

            return DateOnly.FromDateTime(dataSla);
        }
    }
}
