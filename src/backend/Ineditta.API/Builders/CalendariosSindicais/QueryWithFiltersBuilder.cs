using System.Text;

using MySqlConnector;

namespace Ineditta.API.Builders.CalendariosSindicais
{
    public class QueryWithFiltersBuilder
    {
        private StringBuilder Query { get; set; }
        private List<MySqlParameter> Parameters { get; set; }

        public QueryWithFiltersBuilder(StringBuilder query, List<MySqlParameter> parameters)
        {
            Query = query;
            Parameters = parameters;
        }
    }
}
