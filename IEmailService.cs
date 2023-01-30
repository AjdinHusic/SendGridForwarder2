using SendGrid;

namespace SendGridForwarder2;

public interface IEmailService
{
    Task<Response> ForwardEmail(EmailService.ForwardEmailRequest request);
}