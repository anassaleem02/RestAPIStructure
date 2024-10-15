using Microsoft.Extensions.DependencyInjection;
using CommonDataLayer.IRepositories;
using DataAccessLayer.Repositories;
using System.Data.SqlClient;
using System.Data;
using BusinessLayer.Services;
using Microsoft.Extensions.Localization;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Builder;
using CommonDataLayer.IServices;

namespace DataAccessLayer
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, string connectionString)
        {
            services.AddTransient<IDbConnection>(sp => new SqlConnection(connectionString));
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped(typeof(IUserRepository), typeof(UserRepository));
            return services;
        }

        public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
        {
            // Register your business services here
            services.AddScoped(typeof(IGenericService<,>), typeof(GenericService<,>));
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            // Register the generic LocalizationService
            services.AddSingleton(typeof(IStringLocalizer<>), typeof(LocalizationService<>));

            // Configure Localization
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                        new CultureInfo("en"),
                        new CultureInfo("ur")
                };

                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
                options.RequestCultureProviders.Insert(1, new AcceptLanguageHeaderRequestCultureProvider());
            });

            return services;
        }

    }
}
