using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace DataDrivenBot.Model
{
    [Serializable]
    public partial class Display
    {
        public override async Task Execute(IDialogContext context)
        {
            Dictionary<string, string> properties = null;
            context.PrivateConversationData.TryGetValue("Properties", out properties);

            string updatedText = InjectPropertyValues(Text, properties);

            await context.PostAsync(updatedText);
            await ProcessResponse(context, null);
        }
    }
}
