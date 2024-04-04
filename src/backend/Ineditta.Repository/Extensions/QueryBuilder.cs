using System.Text;

using MySqlConnector;

namespace Ineditta.Repository.Extensions
{
    public static class QueryBuilder
    {
#pragma warning disable CA1305 // Specify IFormatProvider
        public static void AppendListToQueryBuilder<T>(
    StringBuilder queryBuilder,
    IEnumerable<T> items,
    string columnName,
    IDictionary<string, object> parameters)
        {
            if (items == null || !items.Any())
            {
                return;
            }

            var parametersForCurrentItems = CreateParametersForItems(items, parameters.Count);

            parametersForCurrentItems.ToList().ForEach(parameters.Add);

            var parameterNames = parametersForCurrentItems.Select(p => p.Key);
            queryBuilder.Append($" AND {columnName} IN ({string.Join(", ", parameterNames)})");
        }

        public static void AppendListToQueryBuilder<T>(
    StringBuilder queryBuilder,
    IEnumerable<T> items,
    IReadOnlyList<string> columnsNames,
    IDictionary<string, object> parameters)
        {
            if (items == null || !items.Any())
            {
                return;
            }

            var parametersForCurrentItems = CreateParametersForItems(items, parameters.Count);

            parametersForCurrentItems.ToList().ForEach(parameters.Add);

            var parameterNames = parametersForCurrentItems.Select(p => p.Key);

            queryBuilder.Append(" AND (");

            for (int i = 0; i < columnsNames.Count; i++)
            {
                queryBuilder.Append($"{columnsNames[i]} IN ({string.Join(", ", parameterNames)}) ");

                if (i < columnsNames.Count - 1)
                {
                    queryBuilder.Append("OR ");
                }
            }

            queryBuilder.Append(')');
        }

        public static void AppendListToQueryBuilder<T>(
    StringBuilder queryBuilder,
    IEnumerable<T> items,
    string columnName,
    List<MySqlParameter> sqlParameters,
    ref int currentParameterCount)
        {
            if (items == null || !items.Any())
            {
                return;
            }

            var parametersForCurrentItems = CreateParametersForItems(items, ref currentParameterCount);
            sqlParameters.AddRange(parametersForCurrentItems);

            var parameterNames = parametersForCurrentItems.Select(p => p.ParameterName);
            queryBuilder.Append($" AND {columnName} IN ({string.Join(", ", parameterNames)})");
        }

        public static void AppendListToQueryBuilder<T1,T2>(
        StringBuilder queryBuilder,
        IEnumerable<T1> itemsList1,
        string columnName1,
        IEnumerable<T2> itemsList2,
        string columnName2,
        List<MySqlParameter> sqlParameters,
        ref int currentParameterCount)
        {
            if (itemsList1 == null || !itemsList1.Any() || itemsList2 == null || !itemsList2.Any())
            {
                return;
            }

            var parametersForCurrentItems1 = CreateParametersForItems(itemsList1, ref currentParameterCount);
            sqlParameters.AddRange(parametersForCurrentItems1);

            var parametersForCurrentItems2 = CreateParametersForItems(itemsList2, ref currentParameterCount);
            sqlParameters.AddRange(parametersForCurrentItems2);

            var parameterNames1 = parametersForCurrentItems1.Select(p => p.ParameterName);
            var parameterNames2 = parametersForCurrentItems2.Select(p => p.ParameterName);
            queryBuilder.Append($" AND ({columnName1} IN ({string.Join(", ", parameterNames1)}) OR {columnName2} IN ({string.Join(", ", parameterNames2)}))");
        }

        public static void AppendListJsonToQueryBuilder<T>(StringBuilder queryBuilder,
            IEnumerable<T> items,
            string columnName,
            string jsonPath,
            List<MySqlParameter> sqlParameters,
            ref int currentParameterCount)
        {
            if (items == null || !items.Any())
            {
                return;
            }

            var whereClause = new List<string>();
            var index = 1;

            foreach (var item in items)
            {
                whereClause.Add($"JSON_CONTAINS({columnName}, @value{(currentParameterCount + index)})");
                index++;
            }

            var query = string.Join(" OR ", whereClause);

            var parametersForCurrentItems = CreateParametersForItems(items, jsonPath, ref currentParameterCount);
            sqlParameters.AddRange(parametersForCurrentItems);


            queryBuilder.Append($" AND ( {query} )");
        }

        public static void AppendListJsonToQueryBuilder<T1,T2>(StringBuilder queryBuilder,
            IEnumerable<T1> items1,
            string columnName1,
            string jsonPath1,
            IEnumerable<T2> items2,
            string columnName2,
            string jsonPath2,
            List<MySqlParameter> sqlParameters,
            ref int currentParameterCount)
        {
            if (items1 == null || !items1.Any() || items2 == null || !items2.Any())
            {
                return;
            }

            var whereClause = new List<string>();
            var index = 1;

            foreach (var item in items1)
            {
                whereClause.Add($"JSON_CONTAINS({columnName1}, @value{(currentParameterCount + index)})");
                index++;
            }

            var query1 = string.Join(" OR ", whereClause);

            var parametersForCurrentItems = CreateParametersForItems(items1, jsonPath1, ref currentParameterCount);
            sqlParameters.AddRange(parametersForCurrentItems);

            var whereClause2 = new List<string>();
            var index2 = 1;

            foreach (var item in items2)
            {
                whereClause2.Add($"JSON_CONTAINS({columnName2}, @value{(currentParameterCount + index2)})");
                index2++;
            }

            var query2 = string.Join(" OR ", whereClause2);

            var parametersForCurrentItems2 = CreateParametersForItems(items2, jsonPath2, ref currentParameterCount);
            sqlParameters.AddRange(parametersForCurrentItems2);


            queryBuilder.Append($" AND (( {query1} ) OR ( {query2} ))");
        }

        public static void AppendListJsonToQueryBuilder<T>(StringBuilder queryBuilder,
           IEnumerable<T> items,
           string columnName,
           string jsonPath,
           IDictionary<string, object> parameters)
        {
            if (items == null || !items.Any())
            {
                return;
            }

            var whereClause = new List<string>();
            var index = 1;

            foreach (var item in items)
            {
                whereClause.Add($"JSON_CONTAINS({columnName}, @value{(parameters.Count + index)})");
                index++;
            }

            var query = string.Join(" OR ", whereClause);

            var parametersForCurrentItems = CreateParametersForItems(items, jsonPath, parameters.Count);

            parametersForCurrentItems.ToList().ForEach(parameters.Add);

            queryBuilder.Append($" AND ( {query} )");
        }

        public static void AppendListArrayToQueryBuilder<T>(StringBuilder queryBuilder,
           IEnumerable<T> items,
           string columnName,
           List<MySqlParameter> sqlParameters,
           ref int currentParameterCount)
        {
            if (items == null || !items.Any())
            {
                return;
            }

            var whereClause = new List<string>();
            var index = 1;

            foreach (var item in items)
            {
                if (typeof(T) == typeof(int))
                {
                    whereClause.Add($"JSON_CONTAINS({columnName}, JSON_ARRAY(CAST(@value{(currentParameterCount + index)} AS SIGNED INTEGER)))");
                }
                else
                {
                    whereClause.Add($"JSON_CONTAINS({columnName}, JSON_ARRAY(@value{(currentParameterCount + index)}))");
                }
                
                index++;
            }

            var query = string.Join(" OR ", whereClause);

            var parametersForCurrentItems = CreateParametersForItems(items, string.Empty, ref currentParameterCount);
            sqlParameters.AddRange(parametersForCurrentItems);


            queryBuilder.Append($" AND ( {query} )");
        }

        public static void AppendListArrayToQueryBuilder<T>(StringBuilder queryBuilder,
           IEnumerable<T> items,
           string columnName,
           bool useStringCasting,
           List<MySqlParameter> sqlParameters,
           ref int currentParameterCount)
        {
            if (items == null || !items.Any())
            {
                return;
            }

            var whereClause = new List<string>();
            var index = 1;

            foreach (var item in items)
            {
                if (typeof(T) == typeof(int))
                {
                    whereClause.Add($"JSON_CONTAINS({columnName}, JSON_ARRAY(CAST(@value{(currentParameterCount + index)} AS SIGNED INTEGER)))");
                }
                else
                {
                    whereClause.Add($"JSON_CONTAINS({columnName}, JSON_ARRAY(@value{(currentParameterCount + index)}))");
                }

                index++;
            }

            var query = string.Join(" OR ", whereClause);

            var parametersForCurrentItems = CreateParametersForItems(items, useStringCasting, ref currentParameterCount);
            sqlParameters.AddRange(parametersForCurrentItems);


            queryBuilder.Append($" AND ( {query} )");
        }

        private static IEnumerable<MySqlParameter> CreateParametersForItems<T>(IEnumerable<T> items, ref int startFrom)
        {
            var parameters = new List<MySqlParameter>();

            foreach (var item in items)
            {
                var parameterName = "@value" + (++startFrom);
                parameters.Add(new MySqlParameter(parameterName, item));
            }

            return parameters;
        }

        private static IEnumerable<KeyValuePair<string, object>> CreateParametersForItems<T>(IEnumerable<T> items, int currentIndex)
        {
            var parameters = new List<KeyValuePair<string, object>>();

            foreach (var item in items)
            {
                if (item is null)
                {
                    continue;
                }

                var parameterName = "@value" + (++currentIndex);
                parameters.Add(new KeyValuePair<string, object>(parameterName, item));
            }

            return parameters;
        }

        private static IEnumerable<MySqlParameter> CreateParametersForItems<T>(IEnumerable<T> items,string jsonPath, ref int startFrom)
        {
            var parameters = new List<MySqlParameter>();

            foreach (var item in items)
            {
                var parameterName = "@value" + (++startFrom);

                if (string.IsNullOrEmpty(jsonPath))
                {
                    if (item is string)
                    {
                        parameters.Add(new MySqlParameter(parameterName, $"\"{item}\""));
                        continue;
                    }

                    parameters.Add(new MySqlParameter(parameterName, $"{item}"));
                    continue;
                }

                if (item is string)
                {
                    parameters.Add(new MySqlParameter(parameterName, $"{{\"{jsonPath}\": \"{item}\"}}"));
                    continue;
                }
                parameters.Add(new MySqlParameter(parameterName, $"{{\"{jsonPath}\": {item}}}"));
            }

            return parameters;
        }

        private static IEnumerable<MySqlParameter> CreateParametersForItems<T>(IEnumerable<T> items, bool useStringCasting, ref int startFrom)
        {
            var parameters = new List<MySqlParameter>();

            foreach (var item in items)
            {
                var parameterName = "@value" + (++startFrom);

                if (item is string && useStringCasting)
                {
                    parameters.Add(new MySqlParameter(parameterName, $"\"{item}\""));
                    continue;
                }

                parameters.Add(new MySqlParameter(parameterName, $"{item}"));
            }

            return parameters;
        }

        private static IEnumerable<KeyValuePair<string, object>> CreateParametersForItems<T>(IEnumerable<T> items, string jsonPath, int currentIndex)
        {
            var parameters = new List<KeyValuePair<string, object>>();

            foreach (var item in items)
            {
                var parameterName = "@value" + (++currentIndex);

                if (string.IsNullOrEmpty(jsonPath))
                {
                    if (item is string)
                    {
                        parameters.Add(new KeyValuePair<string, object>(parameterName, $"\"{item}\""));
                        continue;
                    }

                    parameters.Add(new KeyValuePair<string, object>(parameterName, $"{item}"));
                    continue;
                }

                if (item is string)
                {
                    parameters.Add(new KeyValuePair<string, object>(parameterName, $"{{\"{jsonPath}\": \"{item}\"}}"));
                    continue;
                }
                parameters.Add(new KeyValuePair<string, object>(parameterName, $"{{\"{jsonPath}\": {item}}}"));
            }

            return parameters;
        }
#pragma warning restore CA1305 // Specify IFormatProvider
    }
}
