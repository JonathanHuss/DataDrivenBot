using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Configuration;
using DataDrivenBot.Model;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ExpressionEvaluator;

namespace DataDrivenBot.Dialogs
{
    [Serializable]
    public class MasterDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(StartConversation);
        }

        public async Task StartConversation(IDialogContext context, IAwaitable<object> argument)
        {
            ConversationDataModel db = new ConversationDataModel();
            int conversationStartID = Int32.Parse(ConfigurationManager.AppSettings["ConversationStartID"]);

            ConversationTemplate template = db.ConversationTemplates.Find(conversationStartID);

            Step startingStep = template.Steps.First(s => s.StartingStep == true);

            context.PrivateConversationData.SetValue("CurrentStepID", startingStep.ID);
            context.PrivateConversationData.SetValue("Properties", new Dictionary<string, string>());

            await startingStep.Execute(context);
        }
    }
}