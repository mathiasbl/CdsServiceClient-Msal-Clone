using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.PowerPlatform.Cds.Client;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

[assembly: FunctionsStartup(typeof(FunctionApp3.Startup))]

namespace FunctionApp3
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton(sp =>
            {
                var connectionString = sp.GetRequiredService<IConfiguration>().GetValue<string>("CdsConnectionString");

                var client = new CdsServiceClient(connectionString);

                return client;
            });

            builder.Services.AddScoped<IOrganizationService>(sp => sp.GetRequiredService<CdsServiceClient>().Clone());
        }
    }
}