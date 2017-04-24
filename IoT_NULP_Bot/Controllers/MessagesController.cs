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

        private static IoT_BotDbEntities db = new IoT_BotDbEntities();

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            
            switch (activity.Type)
            {
                case ActivityTypes.Message:
                    WitClient wit = new WitClient("ZHKXGGSDV7VISTIZDWQOPWJ7DZYQ3APD");
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
                default:
                    break;
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private static Photo GetRandomPhoto()
        {
                var arrToRandomFrom = db.Photos.ToArray();
                return arrToRandomFrom[new Random().Next(arrToRandomFrom.Length)];
            
        }

        private static string GetReplyFromDb(string intent)
        {
                var arrToRandomFrom = db.Responses.Where(x => x.Intent.content == intent).ToArray();
                if (arrToRandomFrom.Length > 0)
                    return arrToRandomFrom[new Random().Next(arrToRandomFrom.Length)].content;
                else
                {
                    var noreply = db.Responses.Where(x => x.Intent.content == "noreply").ToArray();
                    return noreply[new Random().Next(noreply.Length)].content;
                }
        }
    }
}