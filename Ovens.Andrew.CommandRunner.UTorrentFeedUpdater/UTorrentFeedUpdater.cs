using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ovens.Andrew.CommandRunner.Common;
using RestSharp;

namespace Ovens.Andrew.CommandRunner.UTorrentFeedUpdater
{
    [Export(typeof(IRunnable))]
    [ExportMetadata("Name", "UTorrentFeedUpdater")]
    public class UTorrentFeedUpdater: IRunnable
    {
        private string _host;
        private string _username;
        private string _password;
        private string _auth;

        public void Initialize(Dictionary<string, string> settings)
        {
            _host = settings["host"];
            _username = settings["username"];
            _password = settings["password"];
            _auth = string.Format("{0}:{1}", _username, _password);
            _auth = string.Format("Basic {0}", Convert.ToBase64String(Encoding.ASCII.GetBytes(_auth)));
        }

        public async Task<bool> Run()
        {
            Log.Comment("Starting uTorrentFeedUpdater run");

            var token = await GetToken();

            string list = await GetList(token);

            Log.Comment("uTorrent get list succeeded");

            var l = JObject.Parse(list);

            var feeds = l["rssfeeds"];

            var feedIds = feeds.Select(feed => (int) feed[0]).ToList();

            foreach (var feedId in feedIds)
            {
                var s = await UpdateFeed(token, feedId);
                Trace.WriteLine(s);
            }

            Log.Comment("uTorrent update feeds succeeded");

            return true;
        }

        public async Task<string> GetToken()
        {
            var request = new RestRequest("token.html");
            request.AddHeader("Authorization", _auth);
            string tf = await HttpRunner.Execute(request, _host, true);
            string token = tf.Split('>')[2].Split('<')[0];

            return token;
        }

        public async Task<string> GetList(string token)
        {
            var request = new RestRequest("?token={token}&list=1");
            request.AddHeader("Authorization", _auth);
            request.AddUrlSegment("token", token);
            return await HttpRunner.Execute(request, _host, false);
        }

        public async Task<string> UpdateFeed(string token, int feedId)
        {
            var request = new RestRequest("?token={token}&action=rss-update&feed-id={feed}&update=1");
            request.AddHeader("Authorization", _auth);
            request.AddUrlSegment("token", token);
            request.AddUrlSegment("feed", feedId.ToString());
            return await HttpRunner.Execute(request, _host, false);
        }

        public void WaitAfter()
        {}

        public void Condition()
        {}
    }
}
