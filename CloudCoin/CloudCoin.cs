using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace CloudCoin
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CloudCoin
    {
        public enum Denomination { Unknown, One, Five, Quarter, Hundred, KiloQuarter }
        public enum Status { Authenticated, Counterfeit, Fractioned, Unknown }
        public enum raidaNodeResponse { pass, fail, error, unknown }

        [JsonProperty]
        public Denomination denomination
        {
            get
            {
                if (sn < 1) return Denomination.Unknown;
                else if (sn < 2097153) return Denomination.One;
                else if (sn < 4194305) return Denomination.Five;
                else if (sn < 6291457) return Denomination.Quarter;
                else if (sn < 14680065) return Denomination.Hundred;
                else if (sn < 16777217) return Denomination.KiloQuarter;
                else return Denomination.Unknown;
            }
        }
        [JsonProperty]
        public int sn { set; get; }
        [JsonProperty]
        public int nn { set; get; }
        [JsonProperty]
        public string[] an = new string[RAIDA.NODEQNTY];
        public string[] pans = new string[RAIDA.NODEQNTY];
        [JsonProperty]
        public raidaNodeResponse[] detectStatus;
        [JsonProperty]
        public string[] aoid = new string[1];//Account or Owner ID
        
        [JsonProperty]
        public string ed; //expiration in the form of Date expressed as a hex string like 97e2 Sep 2018

        public Status Verdict
        {
            get
            {
                if (percentOfRAIDAPass != 100)
                    return isPassed ? Status.Fractioned : Status.Counterfeit;
                else
                    return isPassed ? Status.Authenticated : Status.Counterfeit;
            }
        }

        public int percentOfRAIDAPass
        {
            get
            {
                return detectStatus.Count(element => element == raidaNodeResponse.pass || element == raidaNodeResponse.error) * 100 / detectStatus.Count();
            }
        }

        public bool isPassed
        {
            get
            {
                return (detectStatus.Count(element => element == raidaNodeResponse.pass) > RAIDA.MINTRUSTEDNODES4AUTH) ? true : false;
            }
        }
/*
        public ImageSource coinImage
        {
            get
            {
                switch (denomination)
                {
                    case Denomination.One:
                        return new BitmapImage(new Uri(@"pack://application:,,,/Resources/1coin.png", UriKind.Absolute));
                    case Denomination.Five:
                        return new BitmapImage(new Uri(@"pack://application:,,,/Resources/5coin.png", UriKind.Absolute));
                    case Denomination.Quarter:
                        return new BitmapImage(new Uri(@"pack://application:,,,/Resources/25coin.png", UriKind.Absolute));
                    case Denomination.Hundred:
                        return new BitmapImage(new Uri(@"pack://application:,,,/Resources/100coin.png", UriKind.Absolute));
                    case Denomination.KiloQuarter:
                        return new BitmapImage(new Uri(@"pack://application:,,,/Resources/250coin.png", UriKind.Absolute));
                    default:
                        return new BitmapImage(new Uri(@"pack://application:,,,/Resources/stackcoins.png", UriKind.Absolute));
                }
            }
        }
*/


		// Constructor from args
		[JsonConstructor]
        public CloudCoin(int nn, int sn, string[] ans, string expired, string[] aoid)
        {
            this.sn = sn;
            this.nn = nn;
            an = ans;
            ed = expired;
            this.aoid = aoid;
//            filetype = Type.json;
//            filename = null;
            generatePans();
            detectStatus = new raidaNodeResponse[RAIDA.NODEQNTY];
            for (int i = 0; i < RAIDA.NODEQNTY; i++) detectStatus[i] = raidaNodeResponse.unknown;
        }
        
		public void generatePans()
		{
            Random rnd = new Random();
            byte[] buf = new byte[16];
            for (int i = 0; i < RAIDA.NODEQNTY; i++)
            {
                string aaa = "";
                rnd.NextBytes(buf);
                for (int j = 0; j < buf.Length; j++)
                {
                    aaa += buf[j].ToString("X2");
                }
				pans[i] = aaa;
            }
        }

		public class CoinComparer : IComparer<CloudCoin>
		{
			int IComparer<CloudCoin>.Compare(CloudCoin coin1, CloudCoin coin2)
			{
				return (coin1.sn.CompareTo(coin2.sn));
			}
		}
		public class CoinEqualityComparer : IEqualityComparer<CloudCoin>
		{
			bool IEqualityComparer<CloudCoin>.Equals(CloudCoin x, CloudCoin y)
			{
				return x.sn == y.sn;
			}

			int IEqualityComparer<CloudCoin>.GetHashCode(CloudCoin obj)
			{
				return obj.sn;
			}
		}
    }
}
