using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using RestSharp;
using Newtonsoft.Json;

namespace CloudCoin_SafeScan
{
    public partial class RAIDA
    {

        public CloudCoin fixCoin(CloudCoin brokeCoin)
        {
            //            returnCoin = brokeCoin;
            //            returnCoin.setAnsToPans();// Make sure we set the RAIDA to the cc ans and not new pans. 

            var fixWin = new FixCoinWindow(brokeCoin);
            fixWin.Show();
            DateTime before = DateTime.Now;

            CloudCoin returnCoin = brokeCoin;
            bool fix_result = false;
            FixitHelper fixer;
            String[] trustedServerAns;
            int corner = 1;

            // For every guid, check to see if it is fractured
            for (int guid_id = 0; guid_id < NODEQNTY; guid_id++)
            {
                if (brokeCoin.detectStatus[guid_id] == CloudCoin.raidaNodeResponse.fail)
                { // This guid has failed, get tickets 

                    fixer = new FixitHelper(guid_id, brokeCoin.an);
                    GetTicketResponse[] ticketStatus = new GetTicketResponse[3];

                    corner = 1;
                    while (!fixer.finnished)
                    {
                        fix_result = false;
                        trustedServerAns = new String[]
                        {
                            returnCoin.an[fixer.currentTriad[0].Number],
                            returnCoin.an[fixer.currentTriad[1].Number],
                            returnCoin.an[fixer.currentTriad[2].Number]
                        };

                        ticketStatus = get_tickets(fixer.currentTriad, trustedServerAns, returnCoin.nn, returnCoin.sn, returnCoin.denomination);
                        // See if there are errors in the tickets                  
                        if (ticketStatus[0].status != "ticket" || ticketStatus[1].status != "ticket" || ticketStatus[2].status != "ticket")
                        {// No tickets, go to next triad corner 
                            //check for more fails. 
//                            if (ticketStatus[0].status == "fail") returnCoin.detectStatus[fixer.currentTriad[0].Number] = CloudCoin.raidaNodeResponse.fail;
//                            if (ticketStatus[1].status == "fail") returnCoin.detectStatus[fixer.currentTriad[1].Number] = CloudCoin.raidaNodeResponse.fail;
//                           if (ticketStatus[2].status == "fail") returnCoin.detectStatus[fixer.currentTriad[2].Number] = CloudCoin.raidaNodeResponse.fail;

                            corner++;
                            fixer.setCornerToCheck(corner);
                        }
                        else
                        {
                            // Has three good tickets   
                            fix_result = Instance.NodesArray[guid_id].fix(fixer.currentTriad, ticketStatus[0].message, ticketStatus[1].message, 
                                ticketStatus[2].message, returnCoin.an[guid_id], returnCoin.sn);
                            if (fix_result)  // the guid IS recovered!!!
                            {
                                returnCoin.detectStatus[guid_id] = CloudCoin.raidaNodeResponse.pass;
                                fixWin.Paint(guid_id, Brushes.Green);
                                fixer.finnished = true;
                                corner = 1;
                            }
                            else
                            { // command failed,  need to try another corner
                                corner++;
                                fixer.setCornerToCheck(corner);
                            }//end if else fix was successful
                        }//end if else one of the tickts has an error. 
                    }//end while fixer not finnihsed. 
                    if (!fix_result)  // the guid cannot be recovered! all corners checked
                    {
                        returnCoin.detectStatus[guid_id] = CloudCoin.raidaNodeResponse.fail;
                        fixWin.Paint(guid_id, Brushes.Black);
                    }
                }// end if guid is fail
            }//end for all the guids
            fixWin.Close();
            return returnCoin;
        }

        public GetTicketResponse[] get_tickets(RAIDA.Node[] triad, String[] ans, int nn, int sn, CloudCoin.Denomination denomination)
        {
            GetTicketResponse[] returnTicketsStatus = new GetTicketResponse[3];

            Task<GetTicketResponse>[] tasks = new Task<GetTicketResponse>[3];
            for(int i = 0; i < 3; i++)
            {
                int index = i;
                tasks[index] = Task.Factory.StartNew(() => triad[index].getTicket(nn, sn, ans[index], denomination));
            }

            Task final = Task.Factory.ContinueWhenAll(tasks, predecessors => {
                for (int j = 0; j < 3; j++)
                    returnTicketsStatus[j] = predecessors[j].Result; });
            final.Wait();

            return returnTicketsStatus;
        }//end get_tickets

        public partial class Node
        {
            public GetTicketResponse getTicket(int nn, int sn, String an, CloudCoin.Denomination d)
            {
                var client = new RestClient();
                client.BaseUrl = BaseUri;
                var request = new RestRequest("get_ticket");
                request.AddQueryParameter("nn", nn.ToString());
                request.AddQueryParameter("sn", sn.ToString());
                request.AddQueryParameter("an", an);
                request.AddQueryParameter("pan", an);
                request.AddQueryParameter("denomination", Utils.Denomination2Int(d).ToString());
                request.Timeout = 2000;
                GetTicketResponse getTicketResult = new GetTicketResponse();

                Stopwatch sw = new Stopwatch();
                sw.Start();
                try
                {
                    var response = client.Execute(request);
                    getTicketResult = JsonConvert.DeserializeObject<GetTicketResponse>(response.Content);
                }
                catch (JsonException e)
                {
                    getTicketResult = new GetTicketResponse(Name, sn, "error", "The server does not respond or returns invalid data", DateTime.Now.ToString());
                }
                getTicketResult = getTicketResult ?? new GetTicketResponse(Name, sn, "Network problem", "Node not found", DateTime.Now.ToString());
                if (getTicketResult.ErrorException != null)
                    getTicketResult = new GetTicketResponse(Name, sn, "Network problem", "Problems with network connection", DateTime.Now.ToString());
                sw.Stop();
                getTicketResult.responseTime = sw.Elapsed;

                return getTicketResult;
            }//end get ticket

            public bool fix(Node[] triad, String m1, String m2, String m3, String pan, int sn)
            {
                var client = new RestClient();
                client.BaseUrl = BaseUri;
                var request = new RestRequest("fix");
                request.AddQueryParameter("fromserver1", triad[0].Number.ToString());
                request.AddQueryParameter("fromserver2", triad[1].Number.ToString());
                request.AddQueryParameter("fromserver3", triad[2].Number.ToString());
                request.AddQueryParameter("message1", m1);
                request.AddQueryParameter("message2", m2);
                request.AddQueryParameter("message3", m3);
                request.AddQueryParameter("pan", pan);
                request.Timeout = 2000;

                FixResponse fixResult = new FixResponse();

                Stopwatch sw = new Stopwatch();
                sw.Start();
                try
                {
                    fixResult = JsonConvert.DeserializeObject<FixResponse>(client.Execute(request).Content);
                }
                catch (JsonException ex)
                {//quit
                    return false;
                }
                fixResult = fixResult ?? new FixResponse(Name, sn, "Network problem", "Node not found", DateTime.Now.ToString());
                if (fixResult.ErrorException != null)
                    fixResult = new FixResponse(Name, sn, "Network problem", "Problems with network connection", DateTime.Now.ToString());


                sw.Stop();
                if (fixResult.status == "success")
                {
                    return true;
                }
                return false;
            }//end fix
        }

        public class GetTicketResponse : RestResponse<GetTicketResponse>
        {
            public string server { get; set; }
            public string sn { get; set; }
            public string status { get; set; }
            public string message { get; set; }
            public string time { get; set; }
            public TimeSpan responseTime { get; set; }

            public GetTicketResponse()
            {

                server = "unknown";
                sn = "not set";
                status = "unknown";
                message = "empty";
                time = "";
            }
            public GetTicketResponse(string server, int sn, string status, string message, string time)
            {
                this.server = server;
                this.sn = sn.ToString();
                this.status = status;
                this.message = message;
                this.time = time;
            }
        }

        public class FixResponse : RestResponse<GetTicketResponse>
        {
            public string server { get; set; }
            public string sn { get; set; }
            public string status { get; set; }
            public string message { get; set; }
            public string time { get; set; }
            public TimeSpan responseTime { get; set; }

            public FixResponse()
            {

                server = "unknown";
                sn = "unknown";
                status = "unknown";
                message = "empty";
                time = "";
            }
            public FixResponse(string server, int sn, string status, string message, string time)
            {
                this.server = server;
                this.sn = sn.ToString();
                this.status = status;
                this.message = message;
                this.time = time;
            }
        }
    }
}
