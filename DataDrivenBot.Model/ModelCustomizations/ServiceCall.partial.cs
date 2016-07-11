using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using System.Net;
using System.IO;

namespace DataDrivenBot.Model
{
    [Serializable]
    public partial class ServiceCall : Step
    {
        public override async Task Execute(IDialogContext context)
        {
            Dictionary<string, string> properties = null;
            context.PrivateConversationData.TryGetValue("Properties", out properties);

            string updatedURL = InjectPropertyValues(URL, properties);

            HttpWebRequest webRequest = HttpWebRequest.CreateHttp(updatedURL);

            using (HttpWebResponse response = webRequest.GetResponse() as HttpWebResponse)
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string content = reader.ReadToEnd();
                    await ProcessResponse(context, content);
                }
            }
        }
    }
}
