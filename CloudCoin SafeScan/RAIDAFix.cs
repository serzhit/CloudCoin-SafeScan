/***
 * This software is distributed under MIT License
 * Cloudcoin Consortium, Sergey Gitinsky (c)2017
 * All rights reserved
 */
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;

namespace CloudCoin_SafeScan
{
    public partial class RAIDA
    {
        FixitHelper fixer;
        public event CoinFixStartedEventHandler CoinFixStarted;
        public event CoinFixProcessingEventHandler CoinFixProcessing;
        public event CoinFixFinishedEventHandler CoinFixFinished;

        public void onCoinFixStarted(CoinFixStartedEventArgs e)
        {
            CoinFixStarted?.Invoke(this, e);
        }

        public void onCoinFixProcessing(CoinFixProcessingEventArgs e)
        {
            CoinFixProcessing?.Invoke(this, e);
        }

        public void onCoinFixFinished(CoinFixFinishedEventArgs e)
        {
            CoinFixFinished?.Invoke(this, e);
        }

        public void fixCoin(CloudCoin brokenCoin, int coinindex)
        {
            ObservableStatus[] result = new ObservableStatus[NODEQNTY];
            

            for(int i = 0; i < NODEQNTY; i++) //initializing Array of coin statuses
            {
                result[i] = new ObservableStatus(CloudCoin.raidaNodeResponse.unknown);
            }

            for (int index = 0; index < NODEQNTY; index++)
            {
                int nodeN = index;
                if (brokenCoin.detectStatus[index] != CloudCoin.raidaNodeResponse.pass)
                { // This guid has failed, get tickets 
                    onCoinFixStarted(new CoinFixStartedEventArgs(coinindex, index)); //fire event that particular RAIDA key on particular coin has begun to be fixed
                    result[nodeN] = ProcessFixingGUID(nodeN, brokenCoin, coinindex);
                        
                  //  onCoinFixFinished(new CoinFixFinishedEventArgs(coinindex, index, result[index].Status));
                }// end for failed guid
                else
                    result[index].Status = brokenCoin.detectStatus[index];
            }//end for all the guids
            bool isGood = result.All<ObservableStatus>(x => x.Status == CloudCoin.raidaNodeResponse.pass);
        }

        private ObservableStatus ProcessFixingGUID(int guid_id, CloudCoin brokenCoin, int coinindex)
        {
            fixer = new FixitHelper(guid_id, brokenCoin.an);
            GetTicketResponse[] ticketStatus = new GetTicketResponse[3];
            ObservableStatus result = new ObservableStatus();

            int corner = 1;
            while (!fixer.finnished)
            {
                onCoinFixProcessing(new CoinFixProcessingEventArgs(coinindex, guid_id, corner));
                Logger.Write("Fixing coin " + brokenCoin.sn + ", node " + guid_id + ", corner " + corner + ".", Logger.Level.Debug);
                string[] trustedServerAns = new string[]
                {
                            brokenCoin.an[fixer.currentTriad[0].Number],
                            brokenCoin.an[fixer.currentTriad[1].Number],
                            brokenCoin.an[fixer.currentTriad[2].Number]
                };

                ticketStatus = get_tickets(fixer.currentTriad, trustedServerAns, brokenCoin.nn, brokenCoin.sn, brokenCoin.denomination);
                // See if there are errors in the tickets                  
                if (ticketStatus[0].status == "ticket" && ticketStatus[1].status == "ticket" && ticketStatus[2].status == "ticket")
                {// Has three good tickets   
                    Logger.Write("Fixing coin " + brokenCoin.sn + ", node " + guid_id + ", corner " + corner + ". We have three tickets from nodes " + ticketStatus[0].server +
                        ", " + ticketStatus[1].server + ", " + ticketStatus[2].server + ". Going to fix.", Logger.Level.Debug);
                    FixResponse fff = Instance.NodesArray[guid_id].fix(fixer.currentTriad, ticketStatus[0].message, ticketStatus[1].message,
                        ticketStatus[2].message, brokenCoin.pans[guid_id], brokenCoin.sn);
                    if (fff.status == "success")  // the guid IS recovered!!!
                    {
                        Logger.Write("Coin " + brokenCoin.sn + ", node " + guid_id + ", corner " + corner + ". Fixed!.", Logger.Level.Normal);
                        brokenCoin.detectStatus[guid_id] = result.Status = CloudCoin.raidaNodeResponse.pass;
                        onCoinFixFinished(new CoinFixFinishedEventArgs(coinindex, guid_id, result.Status));
                        brokenCoin.an[guid_id] = brokenCoin.pans[guid_id];
                        fixer.finnished = true;
                        return result;
                    }
                    else if (fff.status == "fail")
                    { // command failed,  need to try another corner
                        Logger.Write("Coin " + brokenCoin.sn + ", node " + guid_id + ", corner " + corner + ". Failed, trying another corner...", Logger.Level.Debug);
                        corner++;
                        fixer.setCornerToCheck(corner);
                        brokenCoin.detectStatus[guid_id] = CloudCoin.raidaNodeResponse.fail;
                    }
                    else if (fff.status == "error")
                    {
                        Logger.Write("Coin " + brokenCoin.sn + ", node " + guid_id + ", corner " + corner + ". Error, trying another corner....", Logger.Level.Debug);
                        corner++;
                        fixer.setCornerToCheck(corner);
                        brokenCoin.detectStatus[guid_id] = CloudCoin.raidaNodeResponse.error;
                    }
                    else
                    {
                        Logger.Write("Coin " + brokenCoin.sn + ", node " + guid_id + ", corner " + corner + ". Error, trying another corner....", Logger.Level.Debug);
                        corner++;
                        fixer.setCornerToCheck(corner);
                        brokenCoin.detectStatus[guid_id] = CloudCoin.raidaNodeResponse.error;
                    }
                    //end if else fix was successful
                }//end if else one of the tickts has an error. 
                else
                {// No tickets, go to next triad corner 
                    Logger.Write("Fixing coin " + brokenCoin.sn + ", node " + guid_id + ", corner " + corner + ". There is no three tickets from triad", Logger.Level.Debug);
                    corner++;
                    fixer.setCornerToCheck(corner);
                }

            }//end while fixer not finnihsed. 
            // the guid cannot be recovered! all corners checked
            Logger.Write("Coin " + brokenCoin.sn + ", node " + guid_id + " was not fixed!", Logger.Level.Normal);
            result.Status = brokenCoin.detectStatus[guid_id];
            onCoinFixFinished(new CoinFixFinishedEventArgs(coinindex, guid_id, result.Status));
            return result;
        }

        private GetTicketResponse[] get_tickets(Node[] triad, string[] ans, int nn, int sn, CloudCoin.Denomination denomination)
        {
            GetTicketResponse[] returnTicketsStatus = new GetTicketResponse[3];
            Task<GetTicketResponse>[] tasks = new Task<GetTicketResponse>[3];

            for(int i = 0; i < 3; i++)
            {
                int index = i;
                tasks[i] = Task.Run(()=> { return triad[index].getTicket(nn, sn, ans[index], denomination); });
            }
            var cont = Task.Factory.ContinueWhenAll(tasks, (ancs) => 
            {
                for(int j = 0; j<ancs.Count(); j++)
                {
                    returnTicketsStatus[j] = ancs[j].Result;
                }
                
            });
            cont.Wait();
            return returnTicketsStatus;
        }//end get_tickets

        public partial class Node
        {
            internal GetTicketResponse getTicket(int nn, int sn, string an, CloudCoin.Denomination d)
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
                Logger.Write("GetTicket request for coin: " + sn + " at node " + this.Number + ", timeout " + request.Timeout + " returned '" + 
                    getTicketResult.status + "' with message '" + getTicketResult.message + "' in " + sw.ElapsedMilliseconds + "ms.", Logger.Level.Debug);
                return getTicketResult;
            }//end get ticket

            internal FixResponse fix(Node[] triad, string m1, string m2, string m3, string pan, int sn)
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
                request.Timeout = 10000;

                FixResponse fixResult = new FixResponse();
                Logger.Write("Fix request to node "+ Number + ": " + client.BuildUri(request), Logger.Level.Debug);

                Stopwatch sw = new Stopwatch();
                sw.Start();
                try
                {
                    var response = client.Execute(request).Content;
                    Logger.Write("Server RAIDA" + Number + " returned following string on fix request: '" + response + "'", Logger.Level.Debug);
                    fixResult = JsonConvert.DeserializeObject<FixResponse>(response);
                }
                catch (JsonException ex)
                {
                    sw.Stop();
                    fixResult = new FixResponse(Name, sn, "error", "Server doesn't respond or returned invalid data", DateTime.Now.ToString());
                    Logger.Write("Fix request for coin: " + sn + " at node " + Number + ", timeout " + request.Timeout + " returned '" +
                    fixResult.status + "' with message '" + fixResult.message + "' return coin sn: '" + fixResult.sn + "' in " + sw.ElapsedMilliseconds + "ms.", Logger.Level.Debug);
                    return fixResult;
                }
                fixResult = fixResult ?? new FixResponse(Name, sn, "error", "Node not found", DateTime.Now.ToString());
                if (fixResult.ErrorException != null)
                    fixResult = new FixResponse(Name, sn, "error", "Problems with network connection", DateTime.Now.ToString());
                sw.Stop();
                fixResult.responseTime = sw.Elapsed;
                Logger.Write("Fix request for coin: " + sn + " at node " + Number + ", timeout " + request.Timeout + " returned '" +
                    fixResult.status + "' with message '" + fixResult.message + "' return coin sn: '" + fixResult.sn + "' in " + sw.ElapsedMilliseconds + "ms.", Logger.Level.Debug);
                return fixResult;
                
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
