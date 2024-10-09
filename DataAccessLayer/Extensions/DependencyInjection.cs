using CommonDataLayer.IRepositories;
using DataAccessLayer.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Data.SqlClient;

namespace DataAccessLayer.Extensions { 
public static class DependencyInjection
{
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, string connectionString)
    {
        services.AddTransient<IDbConnection>(sp => new SqlConnection(connectionString));
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddLogging();
        return services;
    }
}
}
