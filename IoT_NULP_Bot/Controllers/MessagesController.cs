using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using com.valgut.libs.bots.Wit;
using Microsoft.Bot.Connector;

namespace IoT_NULP_Bot.Controllers
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private bool _isSecond = false;

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            var wit = new WitClient("RKIJ5LVF4GKLEWJ5O5NPUPXVCQMKGLJW");
            switch (activity.Type)
            {
                case ActivityTypes.Message:

                    var msg = wit.Converse(activity.From.Id, activity.Text);

                    var intent = string.Empty;
                    double conf = 0;
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
                        if (arrToRandomFrom.Length > 0)
                            res = arrToRandomFrom[new Random().Next(arrToRandomFrom.Length)].content;
                        else
                        {
                            var noreply = ctx.Responses.Where(x => x.Intent.content == "noreply").ToArray();
                            res = noreply[new Random().Next(noreply.Length)].content;
                        }
                    }

                    // return our reply to the user
                    Activity reply = activity.CreateReply(res);
                    await connector.Conversations.ReplyToActivityAsync(reply);
                    break;
                case ActivityTypes.ConversationUpdate:
                    if (!_isSecond)
                    {


                        IMessageActivity imActivity = Activity.CreateMessageActivity();
                        imActivity.From = new ChannelAccount(activity.ChannelId, "AI @ Lviv Polytechnic");
                        imActivity.Conversation = new ConversationAccount(true, activity.Conversation.Id);
                        imActivity.ChannelId = activity.ChannelId;
                        imActivity.Text =
                            "Привіт! Я розмовний бот, запитай в мене про Штучний Інтелект, Львівську Політехніку і лемурів :)";
                        imActivity.Locale = "en-En";
                        connector.Conversations.SendToConversation((Activity) imActivity);
                        _isSecond = true;
                    }
                    break;
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }
    }
}