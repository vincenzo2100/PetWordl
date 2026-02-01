using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetWorld.Application.Interfaces;
using PetWorld.Infrastracture.Persistence;
using PetWorld.Infrastracture.Persistence.Repositories;
using PetWorld.Infrastructure.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetWorld.Infrastracture.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
           
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<PetWorldDbContext>(options =>
                options.UseMySQL(connectionString));

            
            services.AddScoped<IMessageRepository, MessageRepository>();

            
            services.AddScoped<IAgentOrchestrator, AgentOrchestrator>();

            return services;
        }
    }
}
