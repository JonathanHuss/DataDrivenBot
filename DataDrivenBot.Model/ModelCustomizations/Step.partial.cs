using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataDrivenBot.Model
{
    [Serializable]
    public partial class Step
    {
        public virtual async Task Execute(IDialogContext context)
        {
            await ProcessResponse(context, null);
        }

        public virtual Step GetNextStep(string result = null, Dictionary<string, string> properties = null)
        {
            return NextStep;
        }

        protected async Task ProcessResponse(IDialogContext context, string result)
        {
            ConversationDataModel db = new ConversationDataModel();
            int currentStepID;
            context.PrivateConversationData.TryGetValue("CurrentStepID", out currentStepID);

            Step currentStep = db.Steps.Find(currentStepID);

            Dictionary<string, string> properties = null;
            context.PrivateConversationData.TryGetValue("Properties", out properties);

            if (currentStep.PayloadProperty != null)
                properties[currentStep.PayloadProperty.Name] = result.ToString();

            Step nextStep = currentStep.GetNextStep(result, properties);
            
            if (nextStep == null)
                context.Done<object>(null);
            else
            {
                context.PrivateConversationData.SetValue("CurrentStepID", nextStep.ID);
                context.PrivateConversationData.SetValue("Properties", properties);

                await nextStep.Execute(context);
            }
        }

        public async Task MessageLoop(IDialogContext context, IAwaitable<string> message)
        {
            string result = await message;

            await ProcessResponse(context, result);
        }
        public async Task MessageLoop(IDialogContext context, IAwaitable<bool> message)
        {
            string result = (await message).ToString();

            await ProcessResponse(context, result);
        }

        public async Task MessageLoop(IDialogContext context, IAwaitable<long> message)
        {
            string result = (await message).ToString();

            await ProcessResponse(context, result);
        }

        protected string InjectPropertyValues(string text, Dictionary<string, string> properties)
        {
            Regex regex = new Regex("{{.\\S*}}");
            MatchCollection matches = regex.Matches(text);

            foreach (Match match in matches)
            {
                string property = match.Value.Replace("{{", "").Replace("}}", "");

                string injectedValue = "";

                if (property.Contains(":"))
                {
                    string[] propertyParts = property.Split(':');

                    JObject jsonObject = JObject.Parse(properties[propertyParts[0]]);
                    injectedValue = jsonObject.SelectToken(propertyParts[1]).ToString();
                }
                else
                {
                    if (properties.ContainsKey(property))
                        injectedValue = properties[property];
                }

                text = text.Replace(match.Value, injectedValue);
            }

            return text;
        }

    }
}
