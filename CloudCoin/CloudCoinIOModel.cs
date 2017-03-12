using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace CloudCoin
{
	class CloudCoinIn
	{
		public enum Type { json, jpeg, unknown }
		public Type Filetype;
		BinaryReader input;
		public CoinStack Coins;
		public CloudCoinIn(Stream input)
		{
			using (input)
			{
				if (input.CanRead)
				{
					byte[] signature = new byte[20];
					input.Read(signature, 0, 20);
					string sig = Encoding.UTF8.GetString(signature,0,20);
					var reg = new Regex(@"{[.\n\t\x09\x0A\x0D]*""cloudcoin""");
					if (Enumerable.SequenceEqual(signature.Take(3), new byte[] { 255, 216, 255 })) //JPEG
					{
						Filetype = Type.jpeg;
						var coin = ReadJpeg(input);
						Coins = new CoinStack(coin);
					}
					else if (reg.IsMatch(sig)) //JSON
					{
						Filetype = Type.json;
						Coins = ReadJson(input);
					}
				}
				else
				{
					throw new IOException();
				}
			}
		}

		private CloudCoin ReadJpeg(Stream jpegFS)
		{
			// TODO: catch exception for wrong file format
			//            filetype = Type.jpeg;
			byte[] fileByteContent = new byte[455];
			int numBytesToRead = fileByteContent.Length;
			int numBytesRead = 0;
			string[] an = new string[RAIDA.NODEQNTY];
			string[] aoid = new string[1];
			int sn;
			int nn;
			string ed;

			jpegFS.Position = 0;
			while (numBytesToRead > 0)
			{
				// Read may return anything from 0 to numBytesToRead.
				int n = jpegFS.Read(fileByteContent, numBytesRead, numBytesToRead);

				// Break when the end of the file is reached.
				if (n == 0)
					break;

				numBytesRead += n;
				numBytesToRead -= n;
			}

			string jpegHexContent = "";
			jpegHexContent = Utils.ToHexString(fileByteContent);

			for (int i = 0; i < RAIDA.NODEQNTY; i++)
			{
				an[i] = jpegHexContent.Substring(40 + i * 32, 32);
			}
			aoid[0] = jpegHexContent.Substring(840, 55);
			ed = jpegHexContent.Substring(898, 4);
			nn = short.Parse(jpegHexContent.Substring(902, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
			sn = int.Parse(jpegHexContent.Substring(904, 6), System.Globalization.NumberStyles.AllowHexSpecifier);

			return (new CloudCoin(nn, sn, an, ed, aoid));
		}

		private CoinStack ReadJson(Stream jsonFS)
		{
			jsonFS.Position = 0;
			StreamReader sr = new StreamReader(jsonFS);
			CoinStack stack = null;
			try
			{
				stack = JsonConvert.DeserializeObject<CoinStack>(sr.ReadToEnd());
			}
			catch (JsonException ex)
			{
				throw ex;
			}
			return stack;
		}
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class CloudCoinOut
	{
		[JsonProperty]
		public int sn { set; get; }
		[JsonProperty]
		public int nn { set; get; }
		[JsonProperty]
		public string[] an = new string[RAIDA.NODEQNTY];
		[JsonProperty]
		public string[] aoid = new string[1];//Account or Owner ID
		[JsonProperty]
		public string ed; //expiration in the form of Date expressed as a hex string like 97e2 Sep 2018

		public CloudCoinOut(CloudCoin coin)
		{
			sn = coin.sn;
			nn = coin.nn;
			an = coin.an;
			aoid = coin.aoid;
			ed = coin.ed;
		}	
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class CoinStackOut
	{
		public CoinStackOut(CoinStack stack)
		{
			cloudcoin = new List<CloudCoinOut>();
			foreach (CloudCoin coin in stack.cloudcoin)
			{
				cloudcoin.Add(new CloudCoinOut(coin));
			}
		}
		[JsonProperty]
		public List<CloudCoinOut> cloudcoin { get; set; }
/*		public void SaveInFile(string filename)
		{
			FileInfo fi = new FileInfo(filename);
			if (File.Exists(filename))
			{
				var FD = new SaveFileDialog();
				FD.InitialDirectory = fi.DirectoryName;
				FD.Title = "File Exists, Choose Another Name";
				FD.OverwritePrompt = true;
				FD.DefaultExt = "ccstack";
				FD.CreatePrompt = false;
				FD.CheckPathExists = true;
				FD.CheckFileExists = true;
				FD.AddExtension = true;
				FD.ShowDialog();
				fi = new FileInfo(FD.FileName);
			}
			Directory.CreateDirectory(fi.DirectoryName);
			using (StreamWriter sw = fi.CreateText())
			{
				string json = null;
				try
				{
					json = JsonConvert.SerializeObject(this);
					sw.Write(json);
				}
				catch (JsonException ex)
				{
					MessageBox.Show("CloudStackOut.SaveInFile Serialize exception: " + ex.Message);
				}
				catch (IOException ex)
				{
					MessageBox.Show("CloudStackOut.SaveInFile IO exception: " + ex.Message);
				}
			}
		} */
	}
}
