using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
                var logger = sp.GetRequiredService<ILogger<Startup>>();

                return new Lazy<CdsServiceClient>(() =>
                {
                    logger.LogInformation("Connecting to CDS");
                    var client = new CdsServiceClient(Environment.GetEnvironmentVariable("CdsServiceConnectionString"));

                    if (client.IsReady)
                    {
                        logger.LogInformation("Connected to CDS...");
                        return client;
                    }
                    else throw client.LastCdsException;
                });
            });

            builder.Services.AddScoped<IOrganizationService>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<Startup>>();
                var client = sp.GetRequiredService<Lazy<CdsServiceClient>>().Value;

                logger.LogInformation("Cloning client");
                var clone = client.Clone();
                logger.LogInformation("Cloned client");

                return clone;
            });
        }
    }
}