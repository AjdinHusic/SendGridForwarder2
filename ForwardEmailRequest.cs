namespace SendGridForwarder2;

public record ForwardEmailRequest(string From, string To, string ReplyTo,  string Subject, string Body);
