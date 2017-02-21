using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudCoin_SafeScan
{
    class FixitHelper
    {
        //  instance variables
        public RAIDA.Node[] trustedServers = new RAIDA.Node[8];

        // Each servers only trusts eight others
        public RAIDA.Node[] trustedTriad1;
        public RAIDA.Node[] trustedTriad2;
        public RAIDA.Node[] trustedTriad3;
        public RAIDA.Node[] trustedTriad4;
        public RAIDA.Node[] currentTriad;
        public String[] ans1;
        public String[] ans2;
        public String[] ans3;
        public String[] ans4;
        public String[] currentAns = new String[4];
        public bool finnished = false;

        public bool triad_1_is_ready = false;
        public bool triad_2_is_ready = false;
        public bool triad_3_is_ready = false;
        public bool triad_4_is_ready = false;
        public bool currentTriadReady = false;

//        public String[] currentTrustedTriadTickets;

        // All triads have been tried
        public FixitHelper(int raidaNumber, String[] ans)
        {
            // Create an array so we can make sure all the servers submitted are within this allowabel group of servers.
//            RAIDA.Node[] trustedServers = new RAIDA.Node[8]; 
            // FIND TRUSTED NEIGHBOURS
            this.trustedServers = getTrustedServers(raidaNumber);
            
            this.trustedTriad1 = new RAIDA.Node[] { this.trustedServers[0], this.trustedServers[1], this.trustedServers[3] };
            this.trustedTriad2 = new RAIDA.Node[] { this.trustedServers[1], this.trustedServers[2], this.trustedServers[4] };
            this.trustedTriad3 = new RAIDA.Node[] { this.trustedServers[3], this.trustedServers[5], this.trustedServers[6] };
            this.trustedTriad4 = new RAIDA.Node[] { this.trustedServers[4], this.trustedServers[6], this.trustedServers[7] };
            this.currentTriad = this.trustedTriad1;
            // Try the first tried first

            ans1 = new String[] { ans[trustedTriad1[0].Number], ans[trustedTriad1[1].Number], ans[trustedTriad1[2].Number] };
            ans2 = new String[] { ans[trustedTriad2[0].Number], ans[trustedTriad2[1].Number], ans[trustedTriad2[2].Number] };
            ans3 = new String[] { ans[trustedTriad3[0].Number], ans[trustedTriad3[1].Number], ans[trustedTriad3[2].Number] };
            ans4 = new String[] { ans[trustedTriad4[0].Number], ans[trustedTriad4[1].Number], ans[trustedTriad4[2].Number] };

            currentAns = ans1;


        }// end of constructor

        private RAIDA.Node[] getTrustedServers(int raidaNumber)
        {
            RAIDA.Node[] result = new RAIDA.Node[8];
            var i = raidaNumber;
            return result = new RAIDA.Node[] 
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
                    this.currentTriad = this.trustedTriad1;
                    currentTriadReady = triad_1_is_ready;
                    break;
                case 2:
                    this.currentTriad = this.trustedTriad2;
                    currentTriadReady = triad_2_is_ready;
                    break;
                case 3:
                    this.currentTriad = this.trustedTriad3;
                    currentTriadReady = triad_3_is_ready;
                    break;
                case 4:
                    this.currentTriad = this.trustedTriad4;
                    currentTriadReady = triad_4_is_ready;
                    break;
                default:
                    this.finnished = true;
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
                    this.finnished = true;
                    break;
            }//end switch
        }//End fix Guid

    }
}
