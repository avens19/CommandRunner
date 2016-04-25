using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ovens.Andrew.CommandRunner.Common;
using RestSharp;

namespace Ovens.Andrew.CommandRunner.Messenger
{
    [Export(typeof (IRunnable))]
    [ExportMetadata("Name", "Messenger")]
    public class Messenger: IRunnable
    {
        private string _host;
        private string _messageFormat;

        public void Initialize(Dictionary<string, string> settings)
        {
            _host = settings["host"];
            _messageFormat = settings["messageFormat"];
        }

        public async Task<bool> Run()
        {
            Log.Comment("Starting Messenger run");
            string body = string.Format(_messageFormat, Parameters.Args["Name"]);
                
            var request = new RestRequest { RequestFormat = DataFormat.Json };
            var input = new
            {
                data = new {
                    message = body
                }
            };
            request.AddBody(input);
            request.Method = Method.POST;
            string result = await HttpRunner.Execute(request, _host, true);

            Log.Comment("Message sent");

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