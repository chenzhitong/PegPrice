using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;

namespace PEG_Admin
{
    public class EmailSender : IEmailSender
    {
        public AuthMessageSenderOptions Options { get; } // set only via Secret Manager

        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(Options.FromDisplayName, Options.FromAddress));
            message.To.Add(new MailboxAddress(email, email));
            message.Subject = subject;
            message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = htmlMessage
            };
            using var client = new SmtpClient();
            client.Connect(Options.Host, Options.Port, false);
            client.Authenticate(Options.EmailUserName, Options.EmailPassword);
            client.Send(message);
            client.Disconnect(true);
            return Task.Delay(0);
        }
    }
}



<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net5.0</TargetFramework>
    <UserSecretsId>aspnet-PEG_Admin-48D6B1F4-4216-4D66-8995-52AC44875FEE</UserSecretsId>
  </PropertyGroup>

  <ItemGroup>
    <Content Remove="compilerconfig.json" />
  </ItemGroup>

  <ItemGroup>
    <_ContentIncludedByDefault Remove="wwwroot\css\common.css" />
    <_ContentIncludedByDefault Remove="wwwroot\css\user.min.css" />
  </ItemGroup>

  <ItemGroup>
    <None Include="compilerconfig.json" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="MailKit" Version="2.10.1" />
    <PackageReference Include="Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore" Version="5.0.3" />
    <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="5.0.3" />
    <PackageReference Include="Microsoft.AspNetCore.Identity.UI" Version="5.0.3" />
    <PackageReference Include="Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation" Version="5.0.3" />
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="5.0.3" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="5.0.3" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="5.0.3">
      <PrivateTokens>all</PrivateTokens>
      <IncludeTokens>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeTokens>
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.VisualStudio.Web.CodeGeneration.Design" Version="5.0.2" />
    <PackageReference Include="Neo" Version="2.10.3" />
  </ItemGroup>

  <ProjectExtensions><VisualStudio><UserProperties appsettings_1json__JsonSchema="" /></VisualStudio></ProjectExtensions>

</Project>
