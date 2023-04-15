using DotLiquid;
using Microsoft.EntityFrameworkCore;
using SportsStore.Data;
using System.Data.Common;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SportsStore.Helper
{
    public static class RawSqlQuery
    {
        //public static List<T> customRawSqlQuery<T>(string query, Func<DbDataReader, T> map)
        //{
        //    using (var context = new ApplicationDbContext())
        //    {
        //        using (var command = context.Database.GetDbConnection().CreateCommand())
        //        {
        //            command.CommandText = query;
        //            command.CommandType = CommandType.Text;

        //            context.Database.OpenConnection();

        //            using (var result = command.ExecuteReader())
        //            {
        //                var entities = new List<T>();

        //                while (result.Read())
        //                {
        //                    entities.Add(map(result));
        //                }

        //                return entities;
        //            }
        //        }
        //    }
        //}

        public static List<object> GetDynamicSqlValue(string query)
        {
            using (var context = new ApplicationDbContext())
            {
                using (var command = context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = query;
                    command.CommandType = CommandType.Text;

                    context.Database.OpenConnection();

                    using (var result = command.ExecuteReader())
                    {
                        var entities = new List<object>();

                        while (result.Read())
                        {
                            entities.Add(result);
                        }

                        return entities;
                    }
                }
            }
        }
    }
}

