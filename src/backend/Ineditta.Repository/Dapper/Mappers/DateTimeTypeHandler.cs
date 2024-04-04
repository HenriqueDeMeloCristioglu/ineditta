using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dapper;

using MySqlConnector;

namespace Ineditta.Repository.Dapper.Mappers
{
    public class DateTimeTypeHandler : SqlMapper.TypeHandler<DateTime>
    {
        public override DateTime Parse(object value)
        {
            return value is MySqlDateTime mySqlDate && mySqlDate.IsValidDateTime
                ? mySqlDate.GetDateTime()
                : DateTime.MinValue;
        }

        public override void SetValue(IDbDataParameter parameter, DateTime value)
        {
            parameter.DbType = DbType.DateTime;
            parameter.Value = value;
        }
    }
}
