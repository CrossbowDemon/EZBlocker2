using System;
using System.Net;
using System.Timers;

namespace EZBlocker2
{
    class CustomWebClient : WebClient
    {
        private Timer timer = null;
        private WebRequest request = null;

        public int Timeout { get; set; } = 5000;
        //public CookieContainer CookieContainer { get; set; }

        public CustomWebClient()
        {
            //CookieContainer = new CookieContainer();
            timer = new Timer(Timeout);
            timer.Elapsed += new ElapsedEventHandler((sender, e) => {
                timer.Stop();
                CancelAsync();
            });
        }

        public HttpStatusCode GetLastHttpStatusCode()
        {
            if (request != null && base.GetWebResponse(request) is HttpWebResponse response)
                return response.StatusCode;
            else
                throw new InvalidOperationException("Unable to retrieve the status code, maybe you haven't made a request yet.");
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            request = base.GetWebRequest(address);
            //((HttpWebRequest)request).CookieContainer = CookieContainer;

            if (Timeout > 0)
            {
                // Sync timeout
                request.Timeout = Timeout;

                // Async timeout
                if (timer.Interval != Timeout)
                    timer.Interval = Timeout;

                timer.Start();
            }

            return request;
        }
    }
}
