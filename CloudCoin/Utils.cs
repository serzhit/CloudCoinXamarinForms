using System;

namespace CloudCoin
{
    public static class Utils
    {

/*        public static string ChooseInputFile()
        {
            OpenFileDialog FD = new OpenFileDialog();
            FD.Multiselect = true;
            FD.Title = "Choose file with Cloudcoin(s)";
            FD.InitialDirectory = @"C:\Users\Sergey\Documents\GitHub\CloudCoinFoundation\Bank";
            if (FD.ShowDialog() == true)
            {
                return FD.FileName;
            }
            else
            {
                throw new FileNotFoundException();
            }
        }
*/        

        public static string ToHexString(byte[] digest)
        {
            String hash = "";
            foreach (byte aux in digest)
            {
                int b = aux & 0xff;
                if (b.ToString("X2").Length == 1) hash += "0";
                hash += b.ToString("X2");
            }
            return hash;
        }

        public static int Denomination2Int(CloudCoin.Denomination d)
        {
            switch (d)
            {
                case CloudCoin.Denomination.One: return 1;
                case CloudCoin.Denomination.Five: return 5;
                case CloudCoin.Denomination.Quarter: return 25;
                case CloudCoin.Denomination.Hundred: return 100;
                case CloudCoin.Denomination.KiloQuarter: return 250;
                default: return 0;
            }
        }
    }
}
