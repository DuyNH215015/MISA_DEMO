using Dapper;
using Microsoft.Extensions.Configuration;
using MISA.Fresher.Core.Attributes;
using MISA.Fresher.Core.Interface.Repository;
using MISA.Fresher.Core.Interface;
using MySqlConnector;
using System.Reflection;

namespace Misa.Fresher.Infrastructure.Repository
{
    /// <summary>
    /// BaseRepository generic
    /// Implement IBaseRepository
    /// </summary>
    public abstract class BaseRepository<T> : IBaseRepository<T>
    {
        #region Fields
        private readonly string _connectionString;

        /// <summary>
        /// Tên bảng trong DB
        /// </summary>
        protected readonly string _tableName;

        /// <summary>
        /// Property khóa chính
        /// </summary>
        protected readonly PropertyInfo _keyProperty;

        /// <summary>
        /// Tên cột khóa chính trong DB
        /// </summary>
        protected readonly string _keyColumn;
        #endregion

        #region Constructor
        protected BaseRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Developer")
                ?? throw new InvalidOperationException(
                    "Connection string 'Developer' is not configured.");

            // Lấy tên bảng từ TableAttribute 
            var tableAttr = typeof(T).GetCustomAttribute<TableAttribute>();
            _tableName = tableAttr != null
                ? tableAttr.Name
                : typeof(T).Name.ToLower();

            // Tìm khóa chính (property có tên kết thúc bằng Id)
            _keyProperty = typeof(T)
                .GetProperties()
                .First(p => p.Name.EndsWith("Id"));

            // Lấy tên cột DB của khóa chính
            var keyColumnAttr = _keyProperty.GetCustomAttribute<DbColumnAttribute>();
            _keyColumn = keyColumnAttr!.Name;
        }
        #endregion

        #region Create connection
        protected MySqlConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
        #endregion

        #region Insert
        /// <summary>
        /// Thêm mới bản ghi
        /// </summary>
        public Guid Insert(T entity)
        {
            using var connection = CreateConnection();

            var properties = typeof(T).GetProperties();
            var columns = new List<string>();
            var parameters = new List<string>();

            foreach (var prop in properties)
            {
                var dbColumn = prop.GetCustomAttribute<DbColumnAttribute>();
                if (dbColumn == null) continue;

                columns.Add(dbColumn.Name);
                parameters.Add($"@{prop.Name}");

                // Sinh khóa chính
                if (prop == _keyProperty &&
                    prop.PropertyType == typeof(Guid) &&
                    (Guid?)prop.GetValue(entity) == Guid.Empty)
                {
                    prop.SetValue(entity, Guid.NewGuid());
                }
            }

            var sql = $@"
                INSERT INTO {_tableName}
                ({string.Join(",", columns)})
                VALUES
                ({string.Join(",", parameters)})";

            connection.Execute(sql, entity);

            return (Guid)_keyProperty.GetValue(entity)!;
        }
        #endregion

        #region Update
        /// <summary>
        /// Cập nhật bản ghi theo khóa chính
        /// </summary>
        public int Update(T entity)
        {
            using var connection = CreateConnection();

            var properties = typeof(T).GetProperties();
            var setClauses = new List<string>();

            foreach (var prop in properties)
            {
                var dbColumn = prop.GetCustomAttribute<DbColumnAttribute>();
                if (dbColumn == null) continue;

                // Không update khóa chính
                if (prop == _keyProperty) continue;

                // Không update ngày tạo
                if (prop.Name == "CreatedDate") continue;

                // KHÔNG update người tạo
                if (prop.Name == "CreatedBy") continue;
                setClauses.Add($"{dbColumn.Name} = @{prop.Name}");
            }

            var sql = $@"
                UPDATE {_tableName}
                SET {string.Join(",", setClauses)}
                WHERE {_keyColumn} = @{_keyProperty.Name}";

            return connection.Execute(sql, entity);
        }
        #endregion

        #region Delete
        /// <summary>
        /// Xóa bản ghi 
        /// </summary>
        public int Delete(Guid id)
        {
            using var connection = CreateConnection();

            var sql = $@"
             DELETE FROM {_tableName}
             WHERE {_keyColumn} = @Id";

            return connection.Execute(sql, new { Id = id });
        }
        #endregion

        #region Delete
        /// <summary>
        /// Xóa bản ghi số lượng lớn
        /// </summary>
        public int DeleteMany(List<Guid> ids)
        {
            if (ids == null || ids.Count == 0)
                return 0;

            using var connection = CreateConnection();

            var sql = $@"
            DELETE FROM `{_tableName}`
            WHERE `{_keyColumn}` IN @Ids;
        ";

            return connection.Execute(sql, new
            {
                Ids = ids
            });
        }
        #endregion
    }
}
