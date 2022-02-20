using EvoComputerTechService.MapperProfiles;
using EvoComputerTechService.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvoComputerTechService.Extensions
{
    public static class AppServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddAutoMapper(options =>
            {
                options.AddProfile(typeof(AccountProfile));
                options.AddProfile(typeof(PaymentProfile));
            });

            services.AddTransient<IEmailSender, EmailSender>();
            services.AddScoped<IPaymentService, IyzicoPaymentService>();
            //services.AddTransient<EmailSender>();


            return services;
        }
    }
}
