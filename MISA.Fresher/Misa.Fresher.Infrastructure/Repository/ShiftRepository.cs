using Dapper;
using Microsoft.Extensions.Configuration;
using Misa.Fresher.Infrastructure.Helpers;
using MISA.Fresher.Core.DTOs.Shift;
using MISA.Fresher.Core.Entities;
using MISA.Fresher.Core.Exceptions;
using MISA.Fresher.Core.Interface.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Misa.Fresher.Infrastructure.Repository
{
    public class ShiftRepository : BaseRepository<Shift>, IShiftRepository
    {
        public ShiftRepository(IConfiguration configuration)
            : base(configuration)
        {
        }
        public async Task<bool> IsShiftCodeExistAsync(
        string ShiftCode,
        Guid? ExcludeShiftId = null)
        {
            using var connection = CreateConnection();

            var sql = @"
        SELECT 1
        FROM shift
        WHERE shift_code = @ShiftCode
          AND (@ExcludeShiftId IS NULL OR shift_id <> @ExcludeShiftId)
        LIMIT 1;
    ";

            var result = await connection.ExecuteScalarAsync<int?>(
                sql,
                new
                {
                    ShiftCode = ShiftCode,
                    ExcludeShiftId = ExcludeShiftId
                }
            );

            return result.HasValue;
        }

        public int UpdateInactiveMany(List<Guid> ids, bool inactive)
        {
            if (ids == null || ids.Count == 0)
                return 0;

            using var connection = CreateConnection();

            var sql = @"
            UPDATE `shift`
            SET inactive = @Inactive,
            modified_date = NOW()
            WHERE shift_id IN @Ids;
            ";

            return connection.Execute(sql, new
            {
                Ids = ids,
                Inactive = inactive
            });
        }


        private static void ApplyFilter(
          ref string where,
          DynamicParameters parameters,
          FilterCondition filter)
        {
            if (string.IsNullOrWhiteSpace(filter.Field))
                return;

            var propertyName =
                char.ToUpper(filter.Field[0]) + filter.Field.Substring(1);

            var column = DbColumnMapper.GetColumnName<Shift>(propertyName);

            if (string.IsNullOrEmpty(column))
                throw new RepositoryException(
                    $"Không tồn tại cột filter: {filter.Field}",
                    new Exception()
                );

            var paramName = $"@filter_{filter.Field}_{Guid.NewGuid():N}";

            switch (filter.Operator?.ToLower())
            {
                case "contains":
                    where += $" AND {column} LIKE {paramName} ";
                    parameters.Add(paramName, $"%{filter.Value}%");
                    break;

                case "notcontains":
                    where += $" AND {column} NOT LIKE {paramName} ";
                    parameters.Add(paramName, $"%{filter.Value}%");
                    break;

                case "startswith":
                    where += $" AND {column} LIKE {paramName} ";
                    parameters.Add(paramName, $"{filter.Value}%");
                    break;

                case "endswith":
                    where += $" AND {column} LIKE {paramName} ";
                    parameters.Add(paramName, $"%{filter.Value}");
                    break;

                case "eq":
                    {
                        object? value;

                        if (filter.Value is JsonElement json)
                        {
                            value = json.ValueKind switch
                            {
                                JsonValueKind.True => 1,
                                JsonValueKind.False => 0,
                                JsonValueKind.Number => json.GetInt32(),
                                JsonValueKind.String => json.GetString(),
                                _ => null
                            };
                        }
                        else
                        {
                            value = filter.Value;
                        }

                        parameters.Add(paramName, value);
                        where += $" AND {column} = {paramName} ";
                        break;
                    }


                default:
                    throw new RepositoryException(
                        $"Operator không hỗ trợ: {filter.Operator}",
                        new Exception()
                    );
            }
        }
        public async Task<(IEnumerable<Shift>, int)> GetPagingAsync(ShiftQueryRequest request)
        {
            try
            {
                using var connection = CreateConnection();
                await connection.OpenAsync();

                var offset = (request.PageNumber - 1) * request.PageSize;
                var where = "WHERE 1=1 ";
                // ===============================
                // PARAMETER ĐỘNG
                // ===============================
                var parameters = new DynamicParameters();
                parameters.Add("@PageSize", request.PageSize);
                parameters.Add("@Offset", offset);


                // ===============================
                // 2. SEARCH THEO KEYWORD (TEXT)
                // ===============================
                if (!string.IsNullOrWhiteSpace(request.Keyword))
                {
                    var codeCol = DbColumnMapper.GetColumnName<Shift>("ShiftCode");
                    var nameCol = DbColumnMapper.GetColumnName<Shift>("ShiftName");
                    var createdByCol = DbColumnMapper.GetColumnName<Shift>("CreatedBy");
                    var modifiedByCol = DbColumnMapper.GetColumnName<Shift>("ModifiedBy");
                    where += $"AND ({codeCol} LIKE @Keyword OR {nameCol} LIKE @Keyword OR {createdByCol} LIKE @Keyword OR {modifiedByCol} LIKE @Keyword) ";
                    parameters.Add("@Keyword", $"%{request.Keyword}%");
                }


                // ===============================
                // 3. FILTER
                // ===============================

                if (request.Filters != null && request.Filters.Any())
                {
                    foreach (var filter in request.Filters)
                    {
                        ApplyFilter(ref where, parameters, filter);
                    }
                }

                // ===============================
                // 4. SORT 
                // ===============================
                var allowedSortColumns = new List<string> { "ShiftCode", "ShiftName", "CreatedBy", "ModifiedBy", "CreatedDate", "ModifiedDate", "WorkingTime", "BreakingTime", "Inactive" };
                var sortColumn = DbColumnMapper.GetColumnName<Shift>("CreatedDate"); // default
                // xử lí chuyển columnname từ camelcase -> pascalcase 
                var sortBy = request.SortBy?.Trim();
                if (!string.IsNullOrEmpty(sortBy))
                {
                    var matchedColumn = allowedSortColumns
                        .FirstOrDefault(c => string.Equals(c, sortBy, StringComparison.OrdinalIgnoreCase));

                    if (matchedColumn != null)
                        sortColumn = DbColumnMapper.GetColumnName<Shift>(matchedColumn);
                }

                //  Sort direction chỉ nhận ASC hoặc DESC
                var sortDir = request.SortDir?.ToLower() == "asc" ? "ASC" : "DESC";
                // ===============================
                // 5. SQL 
                // ===============================
                var sql = $@"
                   SELECT *
                   FROM shift
                   {where}
                   ORDER BY {sortColumn} {sortDir}
                   LIMIT @PageSize OFFSET @Offset;

                   SELECT COUNT(*)
                   FROM shift
                   {where};
                     ";

                // ===============================
                // 6. QUERY
                // ===============================
                using var multi = await connection.QueryMultipleAsync(sql, parameters);

                var data = await multi.ReadAsync<Shift>();
                var total = await multi.ReadSingleAsync<int>();

                return (data, total);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Lỗi truy vấn dữ liệu Shift", ex);
            }
        }
    }
}

