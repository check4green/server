using System.Collections.Generic;

namespace SensorsManager.Web.Api.Services
{
    public class MessageService : IMessageService
    {

        private readonly IDictionary<Generic, string> _genericMessages;
        private readonly IDictionary<Custom, string> _customMessages;
        private readonly IDictionary<Email, string> _mailMessages;

        public MessageService()
        {
            _genericMessages = new Dictionary<Generic, string>()
            {
                {Generic.NullObject, "You have sent an empty object." } ,
                {Generic.DatabaseFailed,  "Database save failed."},
                {Generic.InvalidRequest, "Invalid request." },
           
            };

            _customMessages = new Dictionary<Custom, string>()
            {
                {Custom.Conflict, " already exists." },
                {Custom.NotFound, " does not exist." }
            };

            _mailMessages = new Dictionary<Email, string>()
            {
                {Email.InvalidEmail, "Your email has not been validated." },
                {Email.MailSucceded, "Mail has been sent." },
                {Email.MailSucceded_Code, "Check your mail, you have been sent a security code." },
                {Email.MailFailed, "Failed to send mail. Please try again." },
            };
        }

        public string GetMessage(Generic genericMessageType)
        {
            return _genericMessages[genericMessageType];
        }

        public string GetMessage(Custom customMessageType, string entityName)
        {
            return entityName + _customMessages[customMessageType];
        }

        public string GetMessage(Custom customMessageType, string entityName, string parameterName)
        {
            return $"A {entityName} with that {parameterName}" + _customMessages[customMessageType];
        }

        public string GetMessage(Email emaiMessageType)
        {
            return _mailMessages[emaiMessageType];
        }
    }
}