# SendGrid Email Sender / Forwarder
A simple, open-source software application for sending emails using the SendGrid API, built with .NET 7 and with Docker support.

## Features
- Easy setup with SendGrid API key
- Simple UI for composing and sending emails
- Ability to send emails to multiple recipients
- Support for HTML templates
- Ability to build route presets using environment variables in either Docker environment, or .env file!

## Prerequisites
- SendGrid account and API key
- .NET 7 OR Docker

## Getting Started
- Clone or download the repository
- Create a SendGrid account and obtain your API key
- Replace the placeholder API key in your .env or Docker env: `SENDGRID_API_KEY=...`
- Build mail preset routes using env variables: `MAIL_PRESET_0=route=/contact;from=test@example.com;to=recipient@example.com;replyTo=info@example.com;subject=Contact;`
- You can place as many ROUTES as you want! just increment the index in `$"MAIL_PRESET_{index}"`. All parameters are optional except for `route=...;`
- Run the application with either Docker or .NET 7!

## How To Use
1. set up your presets;
```.env
SENDGRID_API_KEY=your_api_key_here
# Add preset custom route, all the variables below are optional except for 'route=...;'
MAIL_PRESET_0="route=/contact;from=test@example.com;to=recipient@example.com;replyTo=info@example.com;subject=Contact;"

# Set this to true to open an endpoint /email, that accepts all parameters including 'from' property. (Not recommended)
ALLOW_OPEN_EMAIL_ENDPOINT=false
```
2. Send a post request:

```
POST http://server/contact 
// (all parameters that are not set in the MAIL_PRESET_{index} value, are expected in the body of the request).
BODY 
{
  "body": "<h1>Hello World</h1>"
}
```

## Built With
.NET 7
ASP.NET Core
SendGrid API
Docker

## Contribute
Contributions are always welcome! Feel free to submit a pull request.

## License
This project is licensed under the MIT License.
