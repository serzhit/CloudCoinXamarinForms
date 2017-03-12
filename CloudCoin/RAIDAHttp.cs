using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace CloudCoin
{
	public class RAIDAHttpClient : HttpClient
	{
		public RAIDAHttpClient(Uri baseUri, TimeSpan timeout)
		{
			BaseAddress = baseUri;
			Timeout = timeout;
			DefaultRequestHeaders.Accept.Clear();
			DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
		}
	}

	public class RAIDAResponse : HttpResponseMessage
	{
		public string server { get; set; }
		public string sn { get; set; }
		public string status { get; set; }
		public string message { get; set; }
		public string time { get; set; }
		public TimeSpan responseTime { get; set; }

		public RAIDAResponse()
		{
			server = "unknown";
			sn = null;
			status = "unknown";
			message = "empty";
			time = "";
		}

		public RAIDAResponse(string server, string sn, string status, string message, string time)
		{
			this.server = server;
			this.sn = sn;
			this.status = status;
			this.message = message;
			this.time = time;
		}
	}
}
