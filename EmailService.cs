using SendGrid;
using SendGrid.Helpers.Mail;

namespace SendGridForwarder2;

public class EmailService : IEmailService
{
    private readonly string _sendGridApiKey;

    public EmailService(string sendGridApiKey)
    {
        _sendGridApiKey = sendGridApiKey;
    }
    
    public record ForwardEmailRequest(string From, string To, string ReplyTo,  string Subject, string Body);

    public async Task<Response> ForwardEmail(ForwardEmailRequest request)
    {
        var (from, to, replyTo, subject, body) = request;
        
        var client = new SendGridClient(_sendGridApiKey);
        var msg = new SendGridMessage
        {
            From = new EmailAddress(from),
            Subject = subject,
            ReplyTo = new EmailAddress(replyTo),
            PlainTextContent = body,
            HtmlContent = body
        };
        msg.AddTo(new EmailAddress(to));
        return await client.SendEmailAsync(msg);
    }
}