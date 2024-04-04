using CSharpFunctionalExtensions;
using Ineditta.Excel.ComparativosMapasSindicais.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ineditta.Excel.ComparativosMapasSindicais
{
    public interface IComparativoMapaSindicalService
    {
        ValueTask<Result<byte[]>> GerarAsync(ComparativoMapaSindicalDto model);
    }
}
