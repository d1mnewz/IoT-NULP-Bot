﻿using System;
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
                catch (KeyNotFoundException exc)
                {

                }
                string res = "Я вас не понимаю...";
                var doc = XDocument.Load(@"C:\Users\d1mne\Documents\Visual Studio 2015\Projects\IoT_NULP_Bot\Responses.xml");
                var r = (from x in doc.Descendants("Response")
                         where x.Attribute("intent").Value == intent
                         select x).FirstOrDefault();
                if (r != null)
                {
                    var arr = (from x in r.Descendants("Text") select x.Value).ToArray();
                    if (arr != null && arr.Length > 0)
                    {
                        var rnd = new Random();
                        res = arr[rnd.Next(0, arr.Length)];
                    }
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