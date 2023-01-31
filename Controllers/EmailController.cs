using Microsoft.AspNetCore.Mvc;

namespace SendGridForwarder2.Controllers;

[ApiController]
[Route("[controller]")]
public class EmailController : ControllerBase
{
    private readonly IEmailService _emailService;
    
    public EmailController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost]
    public async Task<IActionResult> ForwardMail(ForwardEmailRequest request)
    {
        var r = await _emailService.ForwardEmail(request);
        return Ok(r);
    }
}