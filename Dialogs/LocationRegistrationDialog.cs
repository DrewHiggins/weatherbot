using System;
using System.IdentityModel.Selectors;
using System.Threading.Tasks;
using Chronic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using WeatherBot.ExtensionMethods;

namespace WeatherBot.Dialogs
{
    [Serializable]
    public class LocationRegistrationDialog : IDialog
    {
        private int _attempts = 3;
        private string _location;

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync($"Now where are you?");
            context.Wait(this.MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            _location = message.Text;
            if (_location.IsValidCity())
            {
                var mapMessage = context.MakeMessage();
                mapMessage.Attachments.Add(new Attachment()
                {
                    ContentUrl =
                        $"https://maps.googleapis.com/maps/api/staticmap?center={_location.GetHTTPEncoded()}&zoom=12&size=300x300",
                    ContentType = "image/png",
                    Name = "map.png"
                });
                await context.PostAsync(mapMessage);
                PromptDialog.Confirm(
                    context,
                    this.AfterConfirmedLocation,
                    $"So you're in {_location}?"
                );
            }
            else
            {
                if (_attempts > 0)
                {
                    await context.PostAsync(
                        "Looks like your city isn't valid. Let's try again: where are you (e.g. 'Redmond, WA')?");
                    context.Wait(this.MessageReceivedAsync);
                }
                else
                {
                    context.Fail(new TooManyAttemptsException("Invalid location format"));
                }
            }

        }

        public async Task AfterConfirmedLocation(IDialogContext context, IAwaitable<bool> result)
        {
            bool confirmed = await result;

            if (confirmed)
            {
                context.Done(_location);
            }
            else
            {
                context.Fail(new UserCancellationException("The user entered the wrong location"));
            }
        }
    }
}