namespace UniqIo.Services.Interface
{
    public interface IEmailService
    {
        void SendEmailConfirmation(string reciever,string name ,string token);
        void SendResetPassword(string reciever, string name, string token);
    }
}
