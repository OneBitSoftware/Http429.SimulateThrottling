// Change your Fiddler version as required
[assembly: Fiddler.RequiredVersion("4.6.3.44034")]

namespace Http429.SimulateThrottling
{
    using Fiddler;
    using System;
    using System.Linq;
    using System.Collections.Concurrent;

    /// <summary>
    /// This Fiddler extension tampers HTTP response messages to HTTP 429 Too Many Requests errors.
    /// It is written to only process *.sharepoint.com requests, and those start with /_vti_bin/client.svc
    /// Throttling driven by a concurrent queue. 10 requests must occur within 5 seconds for the throttling to kick in.
    /// </summary>
    public class ThrottleSimulatorExtension : IAutoTamper2, IFiddlerExtension
    {
        private int MaxRequests = 10;
        private int MaxRequestsTimeWindow = 5; // seconds
        private DateTime LastRequest { get; set; }
        private ConcurrentQueue<DateTime> LastRequests { get; set; }

        public void AutoTamperResponseAfter(Session oSession)
        {
            // exclude messages that we don't want to tamper
            if (oSession.hostname.EndsWith(".sharepoint.com") == false) return;
            if (oSession.PathAndQuery.StartsWith("/_vti_bin/client.svc") == false) return;

            if (LastRequests == null)
            {
                LastRequests = new ConcurrentQueue<DateTime>();
            }

            LastRequest = DateTime.Now;

            // clear up queue
            if (LastRequests.Count >= MaxRequests)
            {
                DateTime dequeueItem;
                LastRequests.TryDequeue(out dequeueItem);
            }

            // Add current request to the end of the queue
            LastRequests.Enqueue(LastRequest);

            // this checks if there are 10 requests within 5 seconds
            if (LastRequests.All(i => i > DateTime.Now.AddSeconds(-MaxRequestsTimeWindow)))
            {
                if (LastRequests.Count >= MaxRequests)
                {
                    oSession.ResponseHeaders.RemoveAll(); // removes SPO headers
                    oSession.ResponseHeaders.HTTPResponseCode = 429;
                    oSession.ResponseHeaders.HTTPResponseStatus = "429 Too Many Requests";
                    oSession.ResponseHeaders["Content-Type"] = "text/html";
                    oSession.utilSetResponseBody("This 429 result is forced by the Fiddler extension."); // required
                }

                // clear up the queue
                for (int i = MaxRequests; i < LastRequests.Count + 1; i++)
                {
                    DateTime dequeueItem;
                    LastRequests.TryDequeue(out dequeueItem);
                }
            }
        }

        // These must stay to satisfy the interface contract
        public void AutoTamperRequestAfter(Session oSession) { }
        public void AutoTamperRequestBefore(Session oSession) { }
        public void AutoTamperResponseBefore(Session oSession) { }
        public void OnBeforeReturningError(Session oSession) { }
        public void OnBeforeUnload() { }
        public void OnLoad() { }
        public void OnPeekAtResponseHeaders(Session oSession) { }
    }
}
