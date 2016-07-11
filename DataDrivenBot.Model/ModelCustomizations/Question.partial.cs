using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace DataDrivenBot.Model
{
    [Serializable]
    public partial class Question : Step
    {
        public override Step GetNextStep(string result = null, Dictionary<string, string> properties = null)
        {
            if (ResponseType.Type == "Choice" || ResponseType.Type == "Boolean")
            {
                ResponseOption option = ResponseOptions.First(o => o.Text.Equals(result, StringComparison.InvariantCultureIgnoreCase));
                return option.NextStep;
            }
            else
                return NextStep;
        }

        public override async Task Execute(IDialogContext context)
        {
            if (ResponseType.Type == "Text")
            {
                PromptDialog.Text(context,
                                    MessageLoop,
                                    Prompt,
                                    RetryPrompt);
            }
            else if (ResponseType.Type == "Choice")
            {
                PromptDialog.Choice(context,
                                        MessageLoop,
                                        ResponseOptions.Select(o => o.Text),
                                        Prompt,
                                        RetryPrompt,
                                        promptStyle: PromptStyle.Auto);
            }
            else if (ResponseType.Type == "Boolean")
            {
                PromptDialog.Confirm(context,
                                    MessageLoop,
                                    Prompt,
                                    RetryPrompt);
            }
            else if (ResponseType.Type == "Integer")
            {
                PromptDialog.Number(context,
                                    MessageLoop,
                                    Prompt,
                                    RetryPrompt);
            }
        }
    }
}
