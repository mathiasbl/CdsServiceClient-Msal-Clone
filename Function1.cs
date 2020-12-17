using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text.Json;
using System.Xml;
using FunctionApp3;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Cds.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace FunctionApp3
{
    [ServiceBusAccount("ServiceBusConnectionString")]
    public class Function
    {
        private readonly IOrganizationService _organizationService;

        public Function(CdsServiceClientPool.Lease lease)
        {
            _organizationService = lease.Client;
        }

        //public Function(CdsServiceClient client)
        //{
        //    _organizationService = client;
        //}

        [FunctionName("accounts")]
        public void Accounts([ServiceBusTrigger("sbt-entities-firstrun", "intersects", IsSessionsEnabled = true)] byte[] body,
            string contentType,
            IDictionary<string, object> userProperties,
            string messageId)
        {
        }
    }
}