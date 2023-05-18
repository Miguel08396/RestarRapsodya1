using RestarRapsodya.Models;

namespace RestarRapsodya.Services
{
    public interface IEmailService
    {
       public void SendEmail(EmailDTO solicitud);
        
    }
}
