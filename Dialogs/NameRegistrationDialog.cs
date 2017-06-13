using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace WeatherBot.Dialogs
{
    [Serializable]
    public class NameRegistrationDialog : IDialog<object>
    {
        private int _attempts = 3;
        private string _name; 

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("First of all, what's your name?");
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            // if the user surpassed three attempts, stop trying
            if (_attempts <= 0)
            {
                context.Fail(new TooManyAttemptsException("Name was empty or incorrect"));
            }

            var message = await result;
            if (message.Text.Length == 0)
            {
                _attempts--;
                await context.PostAsync(
                    "Hmm. Let's try that again: what's your name (e.g. 'Drew', 'Bill', 'Satya'...)?");
                context.Wait(this.MessageReceivedAsync);
            }
            else
            {
                _name = message.Text;
                PromptDialog.Confirm(
                    context,
                    this.AfterNameConfirmation,
                    $"So, you want me to call you {_name}"
                );
            }
        }

        private async Task AfterNameConfirmation(IDialogContext context, IAwaitable<bool> result)
        {
            bool confirmed = await result;
            if (confirmed)
            {
                await context.PostAsync($"Alrighty then, I'll call you {_name}!");
                context.Done(_name);
            }
            else
            {
                context.Fail(new UserCancellationException("The user didn't specify the correct name"));
            }
        }
    }
}