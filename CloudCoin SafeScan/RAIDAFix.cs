using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using GalaSoft.MvvmLight.Threading;
using RestSharp;
using Newtonsoft.Json;

namespace CloudCoin_SafeScan
{
    public partial class RAIDA
    {
        
        FixitHelper fixer;

        public async void fixCoin(CloudCoin brokeCoin, FixCoinWindow fixWin)
        {
            bool[] result = new bool[NODEQNTY];

            for (int guid_id = 0; guid_id < NODEQNTY; guid_id++)
            {
                int index = guid_id; // needed for async tasks
                if (brokeCoin.detectStatus[guid_id] == CloudCoin.raidaNodeResponse.fail)
                { // This guid has failed, get tickets 
                    result[guid_id] = await ProcessFixingGUID(index, brokeCoin, fixWin);
                }// end for failed guid
                else
                    result[guid_id] = true;
            }//end for all the guids
            bool isGood = result.All<bool>(x => x == true);
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                fixWin.ViewModel.StatusText = brokeCoin.sn + (isGood ? "" : " not") + " fixed!";
                Thread.Sleep(500);
            });
        }

        private async Task<bool> ProcessFixingGUID(int guid_id, CloudCoin returnCoin, FixCoinWindow fixWin)
        {
            fixer = new FixitHelper(guid_id, returnCoin.an);
            GetTicketResponse[] ticketStatus = new GetTicketResponse[3];
            bool result = false;

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
                        returnCoin.detectStatus[guid_id] = CloudCoin.raidaNodeResponse.pass;
//                        DispatcherHelper.CheckBeginInvokeOnUI(() => { fixWin.ViewModel.nodeStatus[guid_id] = true; });
                        fixer.finnished = true;
                        result = true;
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
            
            //            DispatcherHelper.CheckBeginInvokeOnUI(() => { fixWin.ViewModel.nodeStatus[guid_id] = false; });
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                fixWin.ViewModel.nodeStatus[guid_id] = result;
                fixWin.ViewModel.StatusText = "Node " + guid_id.ToString() + (result? "":" not") + " fixed!";
                Thread.Sleep(500);
            });

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
