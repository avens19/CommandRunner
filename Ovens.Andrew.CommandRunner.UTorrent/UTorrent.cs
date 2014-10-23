using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using System.Threading.Tasks;
using Ovens.Andrew.CommandRunner.Common;
using RestSharp;

namespace Ovens.Andrew.CommandRunner.UTorrent
{
    [Export(typeof (IRunnable))]
    [ExportMetadata("Name", "UTorrent")]
    public class UTorrent : IRunnable
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
            Log.Comment("Starting uTorrent run");

            var token = await GetToken();

            string result = await RemoveTorrent(token, Parameters.Args["Hash"]);

            Log.Comment("uTorrent result: {0}", result);

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

        public async Task<string> RemoveTorrent(string token, string hash)
        {
            var request = new RestRequest("?token={token}&action=remove&hash={hash}");
            request.AddHeader("Authorization", _auth);
            request.AddUrlSegment("token", token);
            request.AddUrlSegment("hash", hash);
            return await HttpRunner.Execute(request, _host, false);
        }

        public void WaitAfter()
        {
        }

        public void Condition()
        {
        }
    }
}