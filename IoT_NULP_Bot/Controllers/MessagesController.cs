using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Xml.Linq;
using com.valgut.libs.bots.Wit;
using com.valgut.libs.bots.Wit.Models;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IoT_NULP_Bot
{
    //[BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {

            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                var wit = new WitClient("RKIJ5LVF4GKLEWJ5O5NPUPXVCQMKGLJW");
                var msg = wit.Converse(activity.From.Id, activity.Text);

                var intent = string.Empty; double conf = 0;
                try
                {
                    var a = msg.entities["intent"];
                    if (a != null)
                    {
                        foreach (var z in msg.entities["intent"])
                        {
                            if (z.confidence > conf)
                            {
                                conf = z.confidence;
                                intent = z.value.ToString();
                            }
                        }
                    }
                }
                catch (KeyNotFoundException)
                {

                }
                // default reply if intent not found in database
                string res = "Я вас не розумію :(";
                using (var ctx = new IoT_BotDbEntities())
                {
                    var arrToRandomFrom = ctx.Responses.Where(x => x.Intent.content == intent).ToArray();
                    if(arrToRandomFrom.Length > 0)
                        res = arrToRandomFrom[new Random().Next(arrToRandomFrom.Length)].content;
                }

                // return our reply to the user
                Activity reply = activity.CreateReply(res);
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}