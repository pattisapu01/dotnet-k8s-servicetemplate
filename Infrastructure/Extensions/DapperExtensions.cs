using Cloud.$ext_safeprojectname$.Infrastructure.DB;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Cloud.$ext_safeprojectname$.Infrastructure.Extensions
{
    public static class DapperExtensions
    {
        public static async Task<IEnumerable<T>> QueryAsync<T>(
        this DbContext context,
        CancellationToken ct,
        string text,
        object parameters = null,
        int? timeout = null,
        CommandType? type = null
    )
        {
            using var command = new EFDapperCommand(
                context,
                text,
                parameters,
                timeout,
                type,
                ct
            );

            var connection = context.Database.GetDbConnection();
            return await connection.QueryAsync<T>(command.Definition);
        }
    }
}
