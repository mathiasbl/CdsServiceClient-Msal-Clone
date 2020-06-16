using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;

namespace FunctionApp3
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
        public void Accounts([ServiceBusTrigger("%TopicName%", "%accountSubscriptionName%")]byte[] body,
            string contentType,
            IDictionary<string, object> userProperties,
            string messageId)
        {
        }

        //[FunctionName("Timer")]
        //public void Accounts([TimerTrigger("* * * * * *")]TimerInfo myTimer)
        //{
        //}
    }
}