using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System.Configuration;
using Microsoft.Bot.Builder.Dialogs;
using DataDrivenBot.Dialogs;

namespace DataDrivenBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type.Equals("Message", StringComparison.InvariantCultureIgnoreCase))
            {
                await Conversation.SendAsync(activity, () => new MasterDialog());

                return new HttpResponseMessage(HttpStatusCode.Accepted);
            }
            else
            {
                await HandleSystemMessage(activity);
            }

            return null;
        }

        private async Task HandleSystemMessage(IMessageActivity message)
        {
            if (message.Type == "Ping")
            {
                //Message reply = message.CreateReplyMessage();
                //reply.Type = "Ping";
                //return reply;
            }
            else if (message.Type == "DeleteUserData")
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == "BotAddedToConversation")
            {
            }
            else if (message.Type == "BotRemovedFromConversation")
            {
            }
            else if (message.Type == "UserAddedToConversation")
            {
                await Conversation.SendAsync(message, () => new MasterDialog());
            }
            else if (message.Type == "UserRemovedFromConversation")
            {
            }
            else if (message.Type == "EndOfConversation")
            {
            }
        }
        
    }
}