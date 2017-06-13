using System;
using System.IdentityModel.Selectors;
using System.Threading.Tasks;
using Chronic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using WeatherBot.WeatherAPI;

namespace WeatherBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private bool _userWasGreeted = false;
        private string _name;
        private string _location;

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            if (_name == null || _location == null)
            {
                await this.SendWelcomeMessage(context);
            }
            else
            {
                var weatherApi = new ApiInstance();
                WeatherReport report = weatherApi.GetWeatherReport(_location);
                await context.PostAsync($"{_name}, the weather in {_location} is " + 
                    $"{report.Temperature} degrees F and {report.BriefReport.ToLower()}.");
            }
        }

        private async Task SendWelcomeMessage(IDialogContext context)
        {
            if (!_userWasGreeted)
            {
                await context.PostAsync("Hey there, I'm WeatherBot! Let's get you set up!");
            }
            _userWasGreeted = true;
            context.Call(new NameRegistrationDialog(), this.AfterNameRegistration);
        }

        private async Task AfterNameRegistration(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var nameStr = await result;
                _name = nameStr.ToString();
                context.Call(new LocationRegistrationDialog(), this.AfterLocationRegistration);
            }
            catch (TooManyAttemptsException)
            {
                await context.PostAsync("Woah there, let's try that again.");
                await this.SendWelcomeMessage(context);
            }
            catch (UserCancellationException)
            {
                await context.PostAsync("Whoops, was it my accent? Let's try again.");
                await this.SendWelcomeMessage(context);
            }
        }

        private async Task AfterLocationRegistration(IDialogContext context, IAwaitable<object> result)
        {
            var locResult = await result;
            _location = locResult.ToString();
            await context.PostAsync($"Alright, you're in my system as {_name} in {_location}!");
            context.Wait(MessageReceivedAsync);
        }
    }
}