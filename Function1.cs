using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text.Json;
using System.Xml;
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

namespace PortalSynchronization
{
    [ServiceBusAccount("ServiceBusConnectionString")]
    public class Outgoing
    {
        public const string MessageMaxSizeExceededPropertyName = "MessageMaxSizeExceeded";
        public const string ImageName = "Image";

        public readonly Dictionary<string, string> ActionMap = new Dictionary<string, string>()
        {
            ["Create"] = "Created",
            ["Update"] = "Updated",
            ["Delete"] = "Deleted",
        };

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private readonly IOrganizationService _organizationService;

        public Outgoing(
            IServiceProvider serviceProvider,
            ILogger<Outgoing> logger,
            IOrganizationService organizationService)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _organizationService = organizationService ?? throw new ArgumentNullException(nameof(organizationService));
        }

        [FunctionName("accounts")]
        public void Countries([ServiceBusTrigger("dumpqueue")]byte[] body,
            string contentType,
            IDictionary<string, object> userProperties,
            string messageId)
        {
        }
    }
}