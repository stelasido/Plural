using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plural.Services
{
    public class NullMailService : IMailService
    {
        // INJECT THE LOGGER TO THE CLASS
        private readonly ILogger<NullMailService> _logger;
        public NullMailService(ILogger<NullMailService> logger)
        {
            _logger = logger;
        }
        public void SendMessage(string to, string subject, string body)
        {
            // WE WILL LOG EMAIL MESSAGE BUT NOT SEND IT.
            _logger.LogInformation($"To: {to}, BODY: {body}");
        }
    }
}
