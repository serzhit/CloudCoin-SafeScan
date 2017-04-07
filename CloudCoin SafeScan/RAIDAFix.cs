using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Threading;
using RestSharp;
using Newtonsoft.Json;

namespace CloudCoin_SafeScan
{
    public partial class RAIDA
    {
        FixitHelper fixer;
        public event CoinFixStartedEventHandler CoinFixStarted;
        public event CoinFixFinishedEventHandler CoinFixFinished;

        public void onCoinFixStarted(CoinFixStartedEventArgs e)
        {
            CoinFixStarted?.Invoke(this, e);
        }

        public void onCoinFixFinished(CoinFixFinishedEventArgs e)
        {
            CoinFixFinished?.Invoke(this, e);
        }

        public async Task fixCoin(CloudCoin brokeCoin, int coinindex)
        {
            ObservableStatus[] result = new ObservableStatus[NODEQNTY];

            for(int i = 0; i < NODEQNTY; i++) //initializing Array of coin statuses
            {
                result[i] = new ObservableStatus(CloudCoin.raidaNodeResponse.unknown);
            }

            for (int index = 0; index < NODEQNTY; index++)
            {                
                if (brokeCoin.detectStatus[index] == CloudCoin.raidaNodeResponse.fail)
                { // This guid has failed, get tickets 
                    onCoinFixStarted(new CoinFixStartedEventArgs(coinindex, index)); //fire event that particular RAIDA key on particular coin has begun to be fixed
                    result[index] = await ProcessFixingGUID(index, brokeCoin);
                    onCoinFixFinished(new CoinFixFinishedEventArgs(coinindex, index, result[index].Status));
                }// end for failed guid
                else
                    result[index].Status = brokeCoin.detectStatus[index];
            }//end for all the guids
            bool isGood = result.All<ObservableStatus>(x => x.Status == CloudCoin.raidaNodeResponse.pass);
        }

        private async Task<ObservableStatus> ProcessFixingGUID(int guid_id, CloudCoin returnCoin)
        {
            fixer = new FixitHelper(guid_id, returnCoin.an);
            GetTicketResponse[] ticketStatus = new GetTicketResponse[3];
            ObservableStatus result = new ObservableStatus();

            int corner = 1;
            while (!fixer.finnished)
            {
                string[] trustedServerAns = new string[]
                {
                            returnCoin.an[fixer.currentTriad[0].Number],
                            returnCoin.an[fixer.currentTriad[1].Number],
                            returnCoin.an[fixer.currentTriad[2].Number]
                };

                ticketStatus = await get_tickets(fixer.currentTriad, trustedServerAns, returnCoin.nn, returnCoin.sn, returnCoin.denomination);
                // See if there are errors in the tickets                  
                if (ticketStatus[0].status != "ticket" || ticketStatus[1].status != "ticket" || ticketStatus[2].status != "ticket")
                {// No tickets, go to next triad corner 
                    corner++;
                    fixer.setCornerToCheck(corner);
                }
                else
                {// Has three good tickets   
                    var fff = await Instance.NodesArray[guid_id].fix(fixer.currentTriad, ticketStatus[0].message, ticketStatus[1].message,
                        ticketStatus[2].message, returnCoin.an[guid_id], returnCoin.sn);
                    if (fff.status == "success")  // the guid IS recovered!!!
                    {
                        returnCoin.detectStatus[guid_id] = result.Status = CloudCoin.raidaNodeResponse.pass;
                        onCoinFixFinished(new CoinFixFinishedEventArgs(returnCoin.sn, guid_id, result.Status));
                        fixer.finnished = true;
                    }
                    else if (fff.status == "fail")
                    { // command failed,  need to try another corner
                        corner++;
                        fixer.setCornerToCheck(corner);
                        returnCoin.detectStatus[guid_id] = CloudCoin.raidaNodeResponse.fail;
                    }
                    else if (fff.status == "error")
                    {
                        returnCoin.detectStatus[guid_id] = CloudCoin.raidaNodeResponse.error;
                    }
                    else
                    {
                        returnCoin.detectStatus[guid_id] = CloudCoin.raidaNodeResponse.error;
                    }
                    //end if else fix was successful
                }//end if else one of the tickts has an error. 
            }//end while fixer not finnihsed. 
            // the guid cannot be recovered! all corners checked
            
            return result;
        }

        private async Task<GetTicketResponse[]> get_tickets(Node[] triad, String[] ans, int nn, int sn, CloudCoin.Denomination denomination)
        {
            GetTicketResponse[] returnTicketsStatus = new GetTicketResponse[3];

            for(int i = 0; i < 3; i++)
            {
                int index = i;
                returnTicketsStatus[i] = await triad[index].getTicket(nn, sn, ans[index], denomination);
            }

            return returnTicketsStatus;
        }//end get_tickets

        public partial class Node
        {
            internal async Task<GetTicketResponse> getTicket(int nn, int sn, String an, CloudCoin.Denomination d)
            {
                var client = new RestClient();
                client.BaseUrl = BaseUri;
                var request = new RestRequest("get_ticket");
                request.AddQueryParameter("nn", nn.ToString());
                request.AddQueryParameter("sn", sn.ToString());
                request.AddQueryParameter("an", an);
                request.AddQueryParameter("pan", an);
                request.AddQueryParameter("denomination", Utils.Denomination2Int(d).ToString());
                request.Timeout = 5000;
                GetTicketResponse getTicketResult = new GetTicketResponse();

                return await Task.Run<GetTicketResponse>(() => 
                {
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
                });
            }//end get ticket

            internal async Task<FixResponse> fix(Node[] triad, String m1, String m2, String m3, String pan, int sn)
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
                request.Timeout = 5000;

                FixResponse fixResult = new FixResponse();

                
                return await Task.Run<FixResponse>(() => 
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    try
                    {
                        fixResult = JsonConvert.DeserializeObject<FixResponse>(client.Execute(request).Content);
                    }
                    catch (JsonException ex)
                    {
                        return new FixResponse(Name, sn, "error", "Server doesn't respond or returns invalid data", DateTime.Now.ToString());
                    }
                    fixResult = fixResult ?? new FixResponse(Name, sn, "error", "Node not found", DateTime.Now.ToString());
                    if (fixResult.ErrorException != null)
                        fixResult = new FixResponse(Name, sn, "error", "Problems with network connection", DateTime.Now.ToString());
                    sw.Stop();
                    fixResult.responseTime = sw.Elapsed;
                    return fixResult;
                });
                
            }//end fix
        }



        internal class GetTicketResponse : RestResponse<GetTicketResponse>
        {
            public string server { get; set; }
            public string sn { get; set; }
            public string status { get; set; }
            public string message { get; set; }
            public string time { get; set; }
            public TimeSpan responseTime { get; set; }

            internal GetTicketResponse()
            {

                server = "unknown";
                sn = "not set";
                status = "unknown";
                message = "empty";
                time = "";
            }
            internal GetTicketResponse(string server, int sn, string status, string message, string time)
            {
                this.server = server;
                this.sn = sn.ToString();
                this.status = status;
                this.message = message;
                this.time = time;
            }
        }

        internal class FixResponse : RestResponse<GetTicketResponse>
        {
            public string server { get; set; }
            public string sn { get; set; }
            public string status { get; set; }
            public string message { get; set; }
            public string time { get; set; }
            public TimeSpan responseTime { get; set; }

            internal FixResponse()
            {

                server = "unknown";
                sn = "unknown";
                status = "unknown";
                message = "empty";
                time = "";
            }
            internal FixResponse(string server, int sn, string status, string message, string time)
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
