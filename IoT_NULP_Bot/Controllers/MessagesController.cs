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
                    Activity reply = activity.CreateReply();
                  
                    switch (intent)
                    {

                        case "статистика вступу":
                            reply.Attachments = new List<Attachment>();
                            Attachment attachment = new Attachment();
                            attachment.ContentType = "image/png";
                            attachment.ContentUrl = @"http://image.prntscr.com/image/ee2502f6a68041e1813f1bcd125a8bb2.png";
                            Attachment secondAttachment = new Attachment();
                            secondAttachment.ContentType = "image/png";
                            secondAttachment.ContentUrl =
                                @"http://image.prntscr.com/image/258aa8e844d74ab7b63447a9f551ecbd.png";
                            reply.Attachments.Add(attachment);
                            reply.Attachments.Add(secondAttachment);
                            reply.Text = GetReplyFromDb(intent);
                            break;
                        case "фото":
                            var photo = GetRandomPhoto();
                            reply.Attachments = new List<Attachment>();
                            Attachment attachment1 = new Attachment();
                            attachment1.ContentType = "image/png";
                            attachment1.ContentUrl = photo.photoLink;
                            reply.Attachments.Add(attachment1);
                            reply.Text = photo.descrip;
                            break;
                        default:
                            reply.Text = GetReplyFromDb(intent);
                            break;
                    }
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

        private static Photo GetRandomPhoto()
        {
            using (var ctx = new IoT_BotDbEntities())
            {
                var arrToRandomFrom = ctx.Photos.ToArray();
                return arrToRandomFrom[new Random().Next(arrToRandomFrom.Length)];
            }
        }

        private static string GetReplyFromDb(string intent)
        {
            string res;
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

            return res;
        }
    }
}