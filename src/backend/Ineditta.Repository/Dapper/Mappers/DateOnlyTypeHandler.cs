using System.Data;

using Dapper;

using MySqlConnector;

namespace Ineditta.Repository.Dapper.Mappers
{
    public class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
    {
        public override DateOnly Parse(object value)
        {
            return value is MySqlDateTime mySqlDate && mySqlDate.IsValidDateTime
                ? DateOnly.FromDateTime(mySqlDate.GetDateTime())
                : DateOnly.FromDateTime(DateTime.MinValue);
        }

        public override void SetValue(IDbDataParameter parameter, DateOnly value)
        {
            parameter.DbType = DbType.Date;
            parameter.Value = value;
        }
    }
}
