using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Features.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddApplicationRegistration(this IServiceCollection services,Type type)
        {
            var assm = Assembly.GetExecutingAssembly();
           
            services.AddAutoMapper(assm);
            services.AddMediatR(assm);

            return services;
        }
    }
}
