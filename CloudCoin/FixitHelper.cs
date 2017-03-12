namespace CloudCoin
{
    class FixitHelper
    {
        //  instance variables
        public Node[] trustedServers = new Node[8];

        // Each servers only trusts eight others
        public Node[] trustedTriad1;
        public Node[] trustedTriad2;
        public Node[] trustedTriad3;
        public Node[] trustedTriad4;
        public Node[] currentTriad;
        public string[] ans1;
        public string[] ans2;
        public string[] ans3;
        public string[] ans4;
        public string[] currentAns = new string[4];
        public bool finnished = false;

        public bool triad_1_is_ready;
        public bool triad_2_is_ready;
        public bool triad_3_is_ready;
        public bool triad_4_is_ready;
        public bool currentTriadReady;

//        public string[] currentTrustedTriadTickets;

        // All triads have been tried
        public FixitHelper(int raidaNumber, string[] ans)
        {
            // Create an array so we can make sure all the servers submitted are within this allowabel group of servers.
//            Node[] trustedServers = new Node[8]; 
            // FIND TRUSTED NEIGHBOURS
            trustedServers = getTrustedServers(raidaNumber);
            
            trustedTriad1 = new Node[] { trustedServers[0], trustedServers[1], trustedServers[3] };
            trustedTriad2 = new Node[] { trustedServers[1], trustedServers[2], trustedServers[4] };
            trustedTriad3 = new Node[] { trustedServers[3], trustedServers[5], trustedServers[6] };
            trustedTriad4 = new Node[] { trustedServers[4], trustedServers[6], trustedServers[7] };
            currentTriad = trustedTriad1;
            // Try the first tried first

            ans1 = new string[] { ans[trustedTriad1[0].Number], ans[trustedTriad1[1].Number], ans[trustedTriad1[2].Number] };
            ans2 = new string[] { ans[trustedTriad2[0].Number], ans[trustedTriad2[1].Number], ans[trustedTriad2[2].Number] };
            ans3 = new string[] { ans[trustedTriad3[0].Number], ans[trustedTriad3[1].Number], ans[trustedTriad3[2].Number] };
            ans4 = new string[] { ans[trustedTriad4[0].Number], ans[trustedTriad4[1].Number], ans[trustedTriad4[2].Number] };

            currentAns = ans1;


        }// end of constructor

        private Node[] getTrustedServers(int raidaNumber)
        {
            Node[] result = new Node[8];
            var i = raidaNumber;
            return result = new Node[] 
            {
                RAIDA.Instance.NodesArray[(i+19)%25],
                RAIDA.Instance.NodesArray[(i+20)%25],
                RAIDA.Instance.NodesArray[(i+21)%25],
                RAIDA.Instance.NodesArray[(i+24)%25],
                RAIDA.Instance.NodesArray[(i+26)%25],
                RAIDA.Instance.NodesArray[(i+29)%25],
                RAIDA.Instance.NodesArray[(i+30)%25],
                RAIDA.Instance.NodesArray[(i+31)%25]
            };
        }


        public void setCornerToCheck(int corner)
        {
            switch (corner)
            {
                case 1:
                    currentTriad = trustedTriad1;
                    currentTriadReady = triad_1_is_ready;
                    break;
                case 2:
                    currentTriad = trustedTriad2;
                    currentTriadReady = triad_2_is_ready;
                    break;
                case 3:
                    currentTriad = trustedTriad3;
                    currentTriadReady = triad_3_is_ready;
                    break;
                case 4:
                    currentTriad = trustedTriad4;
                    currentTriadReady = triad_4_is_ready;
                    break;
                default:
                    finnished = true;
                    break;
            }
            // end switch
        }//end set corner to check

        /***
     * This changes the Triads that will be used
     */
        public void setCornerToTest(int mode)
        {

            switch (mode)
            {
                case 1:
                    currentTriad = trustedTriad1;
                    currentAns = ans1;
                    currentTriadReady = true;
                    break;
                case 2:
                    currentTriad = trustedTriad2;
                    currentAns = ans2;
                    currentTriadReady = true;
                    break;
                case 3:
                    currentTriad = trustedTriad3;
                    currentAns = ans3;
                    currentTriadReady = true;
                    break;
                case 4:
                    currentTriad = trustedTriad4;
                    currentAns = ans4;
                    currentTriadReady = true;
                    break;
                default:
                    finnished = true;
                    break;
            }//end switch
        }//End fix Guid

    }
}
