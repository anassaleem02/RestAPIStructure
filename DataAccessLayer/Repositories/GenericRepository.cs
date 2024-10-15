using CommonDataLayer.Enums;
using CommonDataLayer.Helpers;
using CommonDataLayer.IRepositories;
using CommonDataLayer.Model.ResponseModels;
using Dapper;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Net;

namespace DataAccessLayer.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly IDbConnection _dbConnection;

        public GenericRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<IEnumerable<TEntity>> GetDataAsync(string searchTerm, string searchColumn, string orderByColumn, bool isAscending, int pageSize, int pageNumber, List<string> searchableColumns, IDbTransaction transaction = null)
        {
            try
            {
                pageNumber = pageNumber > 0 ? pageNumber - 1 : 0;
                var tableName = typeof(TEntity).Name;
                var validProperties = typeof(TEntity).GetProperties().Select(p => p.Name).ToList();

                if (searchableColumns != null && searchableColumns.Any(column =>
                    !validProperties.Any(validProperty =>
                        string.Equals(validProperty, column, StringComparison.OrdinalIgnoreCase)))
                )
                {
                    throw new InvalidOperationException("Invalid searchable column(s) provided.");
                }

                if (searchableColumns == null || !searchableColumns.Any())
                {
                    searchableColumns = validProperties;
                }

                string whereClause = "IsDeleted = 0";
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    string searchIn = " CONCAT(" + string.Join(",", searchableColumns.Where(col => validProperties.Contains(col))) + ")";
                    if (string.IsNullOrEmpty(searchColumn))
                    {
                        whereClause += $" AND {searchIn} LIKE @SearchTerm";
                    }
                    else if (searchableColumns.Any(col => string.Equals(col, searchColumn, StringComparison.OrdinalIgnoreCase)))
                    {
                        whereClause += $" AND [{searchColumn}] LIKE @SearchTerm";
                    }
                    else
                    {
                        throw new InvalidOperationException("Invalid search column provided.");
                    }
                }

                var allowedOrderByColumns = validProperties.Concat(new[] { "Id" }).ToList();
                if (!string.IsNullOrEmpty(orderByColumn) && !allowedOrderByColumns.Any(col => string.Equals(col, orderByColumn, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException("Invalid order by column provided.");
                }

                string orderByClause = string.IsNullOrEmpty(orderByColumn)
                    ? "ORDER BY [Id] DESC"
                    : $"ORDER BY [{orderByColumn}] {(isAscending ? "ASC" : "DESC")}";

                string paginationClause = pageSize > 0
                    ? $"OFFSET {pageNumber * pageSize} ROWS FETCH NEXT {pageSize} ROWS ONLY"
                    : string.Empty;

                string sql = $@"
            SELECT x.*, x.Full_Count 
            FROM (SELECT *, COUNT(*) OVER() AS full_count FROM {tableName} WHERE {whereClause}) AS x 
            {orderByClause} 
            {paginationClause}";

                var param = new { SearchTerm = $"%{searchTerm}%" };
                var result = await _dbConnection.QueryAsync<TEntity>(sql, param, transaction: transaction);
                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error fetching data", ex);
            }
        }



        public async Task<IEnumerable<TEntity>> GetAllAsync(IDbTransaction transaction = null)
        {
            var tableName = typeof(TEntity).Name;
            var sql = $"SELECT * FROM {tableName} WHERE IsDeleted = 0";
            return await _dbConnection.QueryAsync<TEntity>(sql, transaction: transaction);
        }

        public async Task<TEntity?> GetByIdAsync(int id, IDbTransaction transaction = null)
        {
            var tableName = typeof(TEntity).Name;
            var sql = $"SELECT * FROM {tableName} WHERE Id = @Id AND IsDeleted = 0";
            return await _dbConnection.QueryFirstOrDefaultAsync<TEntity>(sql, new { Id = id }, transaction);
        }

        public async Task<int> DeleteAsync(int id, IDbTransaction transaction = null)
        {
            var tableName = typeof(TEntity).Name;
            var sql = $"DELETE FROM {tableName} WHERE Id = @Id";
            return await _dbConnection.ExecuteAsync(sql, new { Id = id }, transaction);
        }

        public async Task<int> SoftDeleteAsync(int id, IDbTransaction transaction = null)
        {
            var entityType = typeof(TEntity);
            var tableName = entityType.Name;

            var deletedOnProperty = entityType.GetProperty("DeletedOn");
            var isDeletedProperty = entityType.GetProperty("IsDeleted");

            if (deletedOnProperty == null || isDeletedProperty == null)
            {
                throw new InvalidOperationException("The entity does not support soft delete.");
            }

            var sql = $"UPDATE {tableName} SET IsDeleted = 1, DeletedOn = @DeletedOn WHERE Id = @Id";
            var parameters = new { Id = id, DeletedOn = DateTime.UtcNow };

            return await _dbConnection.ExecuteAsync(sql, parameters, transaction);
        }

        public async Task<int> AddAsync(TEntity entity, IDbTransaction transaction = null)
        {
            var tableName = typeof(TEntity).Name; // Assumes table name matches entity name
            var properties = typeof(TEntity).GetProperties();

            // Handle identity column
            var identityProperty = properties.FirstOrDefault(p => p.Name == "Id"); // Assuming "Id" is the identity column

            var columnNames = string.Join(", ", properties
                .Where(p => identityProperty == null || p.Name != identityProperty.Name)
                .Select(p => p.Name));

            var parameterNames = string.Join(", ", properties
                .Where(p => identityProperty == null || p.Name != identityProperty.Name)
                .Select(p => "@" + p.Name));

            // Set default property values
            var createdOnProperty = entity.GetType().GetProperty("CreatedOn");
            var statusProperty = entity.GetType().GetProperty("IsActive");
            var isDeletedProperty = entity.GetType().GetProperty("IsDeleted");
            if (createdOnProperty != null && createdOnProperty.CanWrite)
            {
                createdOnProperty.SetValue(entity, DateTime.UtcNow);
            }
            if (statusProperty != null && statusProperty.CanWrite)
            {
                statusProperty.SetValue(entity, true);
            }
            if (isDeletedProperty != null && isDeletedProperty.CanWrite)
            {
                isDeletedProperty.SetValue(entity, false);
            }

            var sql = $"INSERT INTO {tableName} ({columnNames}) VALUES ({parameterNames}); SELECT CAST(SCOPE_IDENTITY() as int);";

            var parameters = properties
                .Where(p => identityProperty == null || p.Name != identityProperty.Name)
                .ToDictionary(p => "@" + p.Name, p => p.GetValue(entity));

            // Execute the query and return the new identity value (if any)
            var result = await _dbConnection.ExecuteScalarAsync<int>(sql, parameters, transaction);
            return result;
        }

        public async Task<int> UpdateAsync(TEntity entity, IDbTransaction transaction = null)
        {
            var entityType = typeof(TEntity);

            // Get properties of the entity
            var properties = entityType.GetProperties();

            // Identify primary key property (assuming it's named "Id" by convention)
            var keyProperty = properties.FirstOrDefault(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));

            // Set UpdatedOn property dynamically
            var updatedOnProperty = entity.GetType().GetProperty("UpdatedOn");

            if (updatedOnProperty != null && updatedOnProperty.CanWrite)
            {
                updatedOnProperty.SetValue(entity, DateTime.UtcNow);
            }

            if (keyProperty == null)
            {
                throw new InvalidOperationException("No primary key property found.");
            }

            // Generate SET clause
            var setClause = string.Join(", ", properties
                .Where(p => !p.Name.Equals(keyProperty.Name, StringComparison.OrdinalIgnoreCase))
                .Select(p => $"{p.Name} = @{p.Name}"));

            // Build SQL query
            var tableName = entityType.Name; // Assumes table name matches entity name
            var sql = $"UPDATE {tableName} SET {setClause} WHERE {keyProperty.Name} = @{keyProperty.Name}";

            // Create parameters dictionary
            var parameters = properties.ToDictionary(p => $"@{p.Name}", p => p.GetValue(entity));

            // Execute the SQL query
            return await _dbConnection.ExecuteAsync(sql, parameters, transaction);
        }

        public async Task<IEnumerable<TResult>> ExecuteJoinAsync<TResult>(string sql, object parameters = null, IDbTransaction transaction = null)
        {
            return await _dbConnection.QueryAsync<TResult>(sql, parameters, transaction);
        }

        public async Task<ApiResponse> ExecuteInTransactionAsync(Func<IDbTransaction, Task<ApiResponse>> operation, ILogger logger, IStringLocalizer localizer, ErrorCode errorCode, SuccessCode? successCode = null)
        {
            IDbTransaction? transaction = null;

            try
            {
                if (_dbConnection.State == ConnectionState.Closed)
                {
                    _dbConnection.Open();
                }

                transaction = _dbConnection.BeginTransaction();

                var result = await operation(transaction);

                // If the operation is successful, commit the transaction
                if (result.Success)
                {
                    transaction.Commit();

                    // Set success message if applicable
                    if (successCode.HasValue)
                    {
                        result.Message = LocalizationHelper.GetLocalizedMessage(localizer, successCode.Value);
                    }
                }
                else
                {
                    // Rollback if the operation was not successful
                    transaction.Rollback();
                    result.Message = localizer[errorCode.ToString()];
                }

                return result;
            }
            catch (Exception ex)
            {
                // Attempt to rollback the transaction in case of an exception
                transaction?.Rollback();

                // Log the exception
                logger.LogError(ex, "Transaction failed.");

                // Return an error response
                return new ApiResponse
                {
                    HttpStatusCode = HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = LocalizationHelper.GetLocalizedMessage(localizer, errorCode),
                    Error = ex.Message
                };
            }
            finally
            {
                if (_dbConnection.State == ConnectionState.Open)
                {
                    _dbConnection.Close();
                }
            }
        }
    }
}
