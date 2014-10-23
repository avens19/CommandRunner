using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using System.Threading.Tasks;
using Ovens.Andrew.CommandRunner.Common;
using RestSharp;

namespace Ovens.Andrew.CommandRunner.XBMC
{
    [Export(typeof (IRunnable))]
    [ExportMetadata("Name", "XBMC")]
    public class Xbmc : IRunnable
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
            Log.Comment("Starting Xbmc run");
            var request = new RestRequest {RequestFormat = DataFormat.Json};
            request.AddHeader("Authorization", _auth);
            var input = new {
                jsonrpc = "2.0",
                method = "VideoLibrary.Scan",
                id = "scan"
            };
            request.AddBody(input);
            request.Method = Method.POST;
            string result = await HttpRunner.Execute(request, _host, true);
            Log.Comment("Xbmc result: {0}", result);
            return true;
        }

        public void WaitAfter()
        {
        }

        public void Condition()
        {
        }
    }
}