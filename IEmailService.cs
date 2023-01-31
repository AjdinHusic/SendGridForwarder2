using SendGrid;

namespace SendGridForwarder2;

public interface IEmailService
{
    Task<Response> ForwardEmail(ForwardEmailRequest request);
}