using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ovens.Andrew.CommandRunner.Common;
using Twilio;

namespace Ovens.Andrew.CommandRunner.Messenger
{
    [Export(typeof (IRunnable))]
    [ExportMetadata("Name", "Messenger")]
    public class Messenger: IRunnable
    {
        private string _accountSid;
        private string _authToken;
        private string _fromNumber;
        private string _toNumber;

        public void Initialize(Dictionary<string, string> settings)
        {
            _accountSid = settings["accountSid"];
            _authToken = settings["authToken"];
            _fromNumber = settings["fromNumber"];
            _toNumber = settings["toNumber"];
        }

        public Task<bool> Run()
        {
            Log.Comment("Starting Messenger run");
            var t = new Task<bool>(() =>
            {
                var twilio = new TwilioRestClient(_accountSid, _authToken);

                string body = string.Format("The torrent {0} finished successfully", Parameters.Args["Name"]);

                var message = twilio.SendMessage(_fromNumber, _toNumber, body);

                Log.Comment("Message sent with status: {0}", message.Status);

                return true;
            });

            t.Start();

            return t;
        }

        public void WaitAfter()
        {
        }

        public void Condition()
        {
        }
    }
}