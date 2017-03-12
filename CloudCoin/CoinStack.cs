using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CloudCoin
{

	[JsonObject(MemberSerialization.OptIn)]
	public class CoinStack : IEnumerable<CloudCoin>
	{
		[JsonProperty]
		public List<CloudCoin> cloudcoin { get; set; }
		public int coinsInStack
		{
			get
			{
				return cloudcoin.Count;
			}
		}
		public int SumInStack
		{
			get
			{
				int s = 0;
				foreach (CloudCoin coin in cloudcoin)
				{
					s += Utils.Denomination2Int(coin.denomination);
				}
				return s;
			}
		}
		public int Ones
		{
			get
			{
				int s = 0;
				foreach (CloudCoin coin in cloudcoin)
				{
					if (coin.denomination == CloudCoin.Denomination.One)
						s++;
				}
				return s;
			}
		}
		public int Fives
		{
			get
			{
				int s = 0;
				foreach (CloudCoin coin in cloudcoin)
				{
					if (coin.denomination == CloudCoin.Denomination.Five)
						s++;
				}
				return s;
			}
		}
		public int Quarters
		{
			get
			{
				int s = 0;
				foreach (CloudCoin coin in cloudcoin)
				{
					if (coin.denomination == CloudCoin.Denomination.Quarter)
						s++;
				}
				return s;
			}
		}
		public int Hundreds
		{
			get
			{
				int s = 0;
				foreach (CloudCoin coin in cloudcoin)
				{
					if (coin.denomination == CloudCoin.Denomination.Hundred)
						s++;
				}
				return s;
			}
		}
		public int KiloQuarters
		{
			get
			{
				int s = 0;
				foreach (CloudCoin coin in cloudcoin)
				{
					if (coin.denomination == CloudCoin.Denomination.KiloQuarter)
						s++;
				}
				return s;
			}
		}
		public int AuthenticatedQuantity
		{
			get
			{
				int s = 0;
				foreach (CloudCoin coin in cloudcoin)
				{
					if (coin.Verdict == CloudCoin.Status.Authenticated)
						s++;
				}
				return s;
			}
		}
		public int FractionedQuantity
		{
			get
			{
				int s = 0;
				foreach (CloudCoin coin in cloudcoin)
				{
					if (coin.Verdict == CloudCoin.Status.Fractioned)
						s++;
				}
				return s;
			}
		}
		public int CounterfeitedQuantity
		{
			get
			{
				int s = 0;
				foreach (CloudCoin coin in cloudcoin)
				{
					if (coin.Verdict == CloudCoin.Status.Counterfeit)
						s++;
				}
				return s;
			}
		}

		public CoinStack()
		{
			cloudcoin = new List<CloudCoin>();
		}

		public CoinStack(CloudCoin coin)
		{
			CloudCoin[] _collection = { coin };
			cloudcoin = new List<CloudCoin>(_collection);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		public IEnumerator<CloudCoin> GetEnumerator()
		{
			return cloudcoin.GetEnumerator();
		}
		public void Add(CoinStack stack2)
		{
			cloudcoin.AddRange(stack2);
//			cloudcoin = cloudcoin. Distinct(new CloudCoin.CoinEqualityComparer()).ToList();
		}
	}
}
