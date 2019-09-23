namespace SensorsManager.Web.Api.Services
{
    public enum Generic
    {
        NullObject,
        DatabaseFailed,
        InvalidRequest,
     
    }
    public enum Custom
    {
        Conflict,
        NotFound
    }
    public enum Email
    {
        InvalidEmail,
        MailSucceded,
        MailFailed,
        MailSucceded_Code
    }
}