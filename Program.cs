using System;
using Twilio;
using Twilio.Rest.Api.V2010.Account;


class Program
{
    static void Main(string[] args)
    {
        DotNetEnv.Env.Load();
        string accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
        string authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");

        TwilioClient.Init(accountSid, authToken);

        var message = MessageResource.Create(
            body: "Hi this is a test message hehe",
            from: new Twilio.Types.PhoneNumber(Environment.GetEnvironmentVariable("FROM")),
            to: new Twilio.Types.PhoneNumber(Environment.GetEnvironmentVariable("TO"))
        );

        Console.WriteLine(message.Sid);
    }
}