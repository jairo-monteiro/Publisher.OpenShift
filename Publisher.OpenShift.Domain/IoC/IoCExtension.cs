using Microsoft.Extensions.DependencyInjection;
using Publisher.OpenShift.Domain.Interfaces;
using Publisher.OpenShift.Domain.Services;

namespace Publisher.OpenShift.Domain.IoC
{
    public static class IoCExtension
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IPublicacaoService, PublicacaoService>();
        }
    }
}