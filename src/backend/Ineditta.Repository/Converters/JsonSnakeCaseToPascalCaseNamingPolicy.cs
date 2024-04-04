using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Ineditta.BuildingBlocks.Core.Extensions;

namespace Ineditta.Repository.Converters
{
    public class JsonSnakeCaseToPascalCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            // Conversion from snake_case to PascalCase
            return name.ToPascalCase();
        }
    }
}
