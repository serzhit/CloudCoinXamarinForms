using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CloudCoin
{
	public class Node
	{
		public int Number;
		public string Name { get; set; }
		public TimeSpan timeout = new TimeSpan(0, 0, 0, 2, 500);
		public RAIDA.Countries Country
		{
			get
			{
				switch (Number)
				{
					case 0: return RAIDA.Countries.Australia;
					case 1: return RAIDA.Countries.Macedonia;
					case 2: return RAIDA.Countries.Philippines;
					case 3: return RAIDA.Countries.Serbia;
					case 4: return RAIDA.Countries.Bulgaria;
					case 5: return RAIDA.Countries.Sweden;
					case 6: return RAIDA.Countries.California;
					case 7: return RAIDA.Countries.UK;
					case 8: return RAIDA.Countries.Punjab;
					case 9: return RAIDA.Countries.Banglore;
					case 10: return RAIDA.Countries.Texas;
					case 11: return RAIDA.Countries.USA1;
					case 12: return RAIDA.Countries.Romania;
					case 13: return RAIDA.Countries.Taiwan;
					case 14: return RAIDA.Countries.Russia1;
					case 15: return RAIDA.Countries.Russia2;
					case 16: return RAIDA.Countries.Columbia;
					case 17: return RAIDA.Countries.Singapore;
					case 18: return RAIDA.Countries.Germany;
					case 19: return RAIDA.Countries.Canada;
					case 20: return RAIDA.Countries.Venezuela;
					case 21: return RAIDA.Countries.Hyperbad;
					case 22: return RAIDA.Countries.USA2;
					case 23: return RAIDA.Countries.Switzerland;
					case 24: return RAIDA.Countries.Luxenburg;
					default: return RAIDA.Countries.USA3;
				}
			}
		}


		public Uri BaseUri
		{
			get { return new Uri("https://RAIDA" + Number.ToString() + ".cloudcoin.global/service"); }
		}
		public Uri BackupUri
		{
			get { return new Uri("https://RAIDA" + Number.ToString() + ".cloudcoin.ch/service"); }
		}
		public RAIDAResponse LastEchoStatus;
		public RAIDAResponse LastDetectResult;
		public string Location
		{
			get
			{
				return Country.ToString();
			}
		}

		public Node(int number)
		{
			Number = number;
			Name = "RAIDA" + number.ToString();
		}

		public async Task<RAIDAResponse> EchoAsync()
		{
			string reqUri = "echo";
			var result = await RequestAsync(-1, reqUri);

			LastEchoStatus = result;

			return result;
		}

		public async Task<RAIDAResponse> DetectAsync(CloudCoin coin)
		{
			string reqUri = "detect?sn=" + coin.sn.ToString() +
							"&nn=" + coin.nn.ToString() +
							"&an=" + coin.an[Number] +
							"&pan=" + coin.pans[Number] +
						    "&denomination=" + Utils.Denomination2Int(coin.denomination).ToString();

			var result = await RequestAsync(coin.sn, reqUri);

			if (result.status == "pass")
			{
				coin.detectStatus[Number] = CloudCoin.raidaNodeResponse.pass;
				coin.an[Number] = coin.pans[Number];
			}
			else if (result.status == "fail")
				coin.detectStatus[Number] = CloudCoin.raidaNodeResponse.fail;
			else
				coin.detectStatus[Number] = CloudCoin.raidaNodeResponse.error;
			LastDetectResult = result;

			return result;
		}

		internal async Task<RAIDAResponse> GetTicketAsync(int nn, int sn, string an, CloudCoin.Denomination d)
		{
			string reqUri = "get_ticket?nn=" + nn.ToString() +
							"&sn=" + sn.ToString() +
							"&an=" + an +
							"&pan=" + an +
							"denomination=" + Utils.Denomination2Int(d).ToString();
			var result = await RequestAsync(sn, reqUri);
			return result;
		}//end get ticket

		internal async Task<RAIDAResponse> FixAsync(Node[] triad, string m1, string m2, string m3, string pan, int sn)
		{
			string reqUri = "fix?fromserver1=" + triad[0].Number.ToString() +
							"&fromserver2=" + triad[1].Number.ToString() +
							"&fromserver3=" + triad[2].Number.ToString() +
							"message1=" + m1 +
							"message2=" + m2 +
							"message3=" + m3 +
							"pan=" + pan;
			var result = await RequestAsync(sn, reqUri);
			return result;
		}//end fix

		async Task<RAIDAResponse> RequestAsync(int sn, string reqUri)
		{
			var client = new RAIDAHttpClient(BaseUri, timeout);
			RAIDAResponse result = new RAIDAResponse();

			Stopwatch sw = new Stopwatch();
			sw.Start();
			try
			{
				var response = await client.GetStringAsync(reqUri);
				result = JsonConvert.DeserializeObject<RAIDAResponse>(response);
			}
			catch (JsonException ex)
			{
				return new RAIDAResponse(Name, sn.ToString(), "error", ex.Message, DateTime.Now.ToString());
			}
			result = result ?? new RAIDAResponse(Name, sn.ToString(), "error", "Node not found", DateTime.Now.ToString());
			if (!result.IsSuccessStatusCode)
				result = new RAIDAResponse(Name, sn.ToString(), "error", "Problems with network connection", DateTime.Now.ToString());
			sw.Stop();
			result.responseTime = sw.Elapsed;
			return result;
		}

		public override string ToString()
		{
			string result = "Server: " + Name +
				"\nLocation: " + Location +
				"\nStatus: " + LastEchoStatus.status +
				"\nEcho: " + LastEchoStatus.responseTime.ToString("sfff") + "ms";
			return result;
		}

		public string ToString(RAIDAResponse res)
		{
			string result = "Server: " + Number +
				"\nLocation: " + Location +
				"\nStatus: " + res.status +
				"\nEcho: " + res.responseTime.ToString("sfff") + "ms";
			return result;
		}
	}
}
