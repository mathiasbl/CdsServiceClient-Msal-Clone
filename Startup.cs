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
            // setup connection
            var client = GetClient();

            builder.Services.AddSingleton(sp => new CdsServiceClientPool(client, sp.GetRequiredService<ILogger<CdsServiceClientPool>>()));
            builder.Services.AddScoped<CdsServiceClientPool.Lease>();

            //builder.Services.AddSingleton<CdsClientWithDate>(sp =>
            //{
            //    return new CdsClientWithDate()
            //    {
            //        localClient = client,
            //        createDate = DateTime.UtcNow
            //    };
            //});

            //builder.Services.AddScoped<CdsServiceClient>(sp =>
            //{
            //    Stopwatch st = new Stopwatch();
            //    st.Restart();
            //    var ms01 = st.ElapsedMilliseconds;
            //    var logger = sp.GetRequiredService<ILogger<Startup>>();
            //    var ms02 = st.ElapsedMilliseconds;
            //    var client = sp.GetRequiredService<CdsClientWithDate>();
            //    var ms03 = st.ElapsedMilliseconds;
            //    logger.LogInformation($"Cloning");
            //    var clone = client.localClient.Clone();
            //    var ms04 = st.ElapsedMilliseconds;
            //    logger.LogInformation($"Cloned client - {ms01} - {ms02} - {ms03} - {ms04}");
            //    st.Stop();
            //    st = null;
            //    return clone;
            //});
        }

        private CdsServiceClient GetClient()
        {
            TraceControlSettings.TraceLevel = System.Diagnostics.SourceLevels.All;
            TraceControlSettings.AddTraceListener(new ConsoleTraceListener());
            var client = new CdsServiceClient(Environment.GetEnvironmentVariable("CdsServiceConnectionString"));
            if (client.IsReady)
            {
                return client;
            }
            else
                throw client.LastCdsException;
        }

        internal class CdsClientWithDate
        {
            public CdsServiceClient localClient { get; set; }
            public DateTime createDate { get; set; }
        }
    }
}