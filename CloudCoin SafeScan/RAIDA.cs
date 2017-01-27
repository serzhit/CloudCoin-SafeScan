using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using RestSharp;
using Newtonsoft.Json;
using System.Net;

namespace CloudCoin_SafeScan
{
    public class RAIDA
    {
        public const short NODEQNTY = 25;
        public Node[] NodesArray = new Node[NODEQNTY];
        public enum Countries
        {
            Australia,
            Macedonia,
            Philippines,
            Serbia,
            Bulgaria,
            Sweden,
            California,
            UK,
            Punjab,
            Banglore,
            Texas,
            USA1,
            USA2,
            USA3,
            Romania,
            Taiwan,
            Russia1,
            Russia2,
            Columbia,
            Singapore,
            Germany,
            Canada,
            Venezuela,
            Hyperbad,
            Switzerland,
            Luxenburg
        };

        public RAIDA()
        {
            for(int i=0; i<NodesArray.Length;i++)
            {
                NodesArray[i] = new Node(i);
            }
        }

        public class Node
        {
            public int Number;
            public Countries Country
            {
                get
                {
                    switch (Number)
                    {
                        case 0: return Countries.Australia;
                        case 1: return Countries.Macedonia;
                        case 2: return Countries.Philippines;
                        case 3: return Countries.Serbia;
                        case 4: return Countries.Bulgaria;
                        case 5: return Countries.Sweden;
                        case 6: return Countries.California;
                        case 7: return Countries.UK;
                        case 8: return Countries.Punjab;
                        case 9: return Countries.Banglore;
                        case 10: return Countries.Texas;
                        case 11: return Countries.USA1;
                        case 12: return Countries.Romania;
                        case 13: return Countries.Taiwan;
                        case 14: return Countries.Russia1;
                        case 15: return Countries.Russia2;
                        case 16: return Countries.Columbia;
                        case 17: return Countries.Singapore;
                        case 18: return Countries.Germany;
                        case 19: return Countries.Canada;
                        case 20: return Countries.Venezuela;
                        case 21: return Countries.Hyperbad;
                        case 22: return Countries.USA2;
                        case 23: return Countries.Switzerland;
                        case 24: return Countries.Luxenburg;
                        default: return Countries.USA3;
                    }
                }
            }

            public string Name { get; set; }
            public Uri BaseUri
            {
                get { return new Uri("https://RAIDA" + Number.ToString() + ".cloudcoin.global/service"); }
            }
            public Uri BackupUri
            {
                get { return new Uri("https://RAIDA" + Number.ToString() + ".cloudcoin.ch/service"); }
            }
            public EchoResponse LastEchoStatus;
            public DetectResponse LastDetectResult;
            public string Location
            {
                get
                {
                    switch (Country)
                    {
                        case RAIDA.Countries.Australia:
                            return "Australia";
                        case RAIDA.Countries.Macedonia:
                            return "Macedonia";
                        case RAIDA.Countries.Philippines:
                            return "Philippines";
                        case RAIDA.Countries.Serbia:
                            return "Serbia";
                        case RAIDA.Countries.Bulgaria:
                            return "Bulgaria";
                        case RAIDA.Countries.Sweden:
                            return "Sweden";
                        case RAIDA.Countries.California:
                            return "California";
                        case RAIDA.Countries.UK:
                            return "UK";
                        case RAIDA.Countries.Punjab:
                            return "Punjab";
                        case RAIDA.Countries.Banglore:
                            return "Banglore";
                        case RAIDA.Countries.Texas:
                            return "Texas";
                        case RAIDA.Countries.USA1:
                            return "USA";
                        case RAIDA.Countries.Romania:
                            return "Romaina";
                        case RAIDA.Countries.Taiwan:
                            return "Taiwan";
                        case RAIDA.Countries.Russia1:
                            return "Russia";
                        case RAIDA.Countries.Russia2:
                            return "Russia";
                        case RAIDA.Countries.Columbia:
                            return "Columbia";
                        case RAIDA.Countries.Singapore:
                            return "Singapore";
                        case RAIDA.Countries.Germany:
                            return "Germany";
                        case RAIDA.Countries.Canada:
                            return "Canada";
                        case RAIDA.Countries.Venezuela:
                            return "Venezuela";
                        case RAIDA.Countries.Hyperbad:
                            return "Hyderabad";
                        case RAIDA.Countries.USA2:
                            return "USA";
                        case RAIDA.Countries.Switzerland:
                            return "Switzerland";
                        case RAIDA.Countries.Luxenburg:
                            return "Luxenburg";
                    }
                    return "Unknown";
                }
            }

            public Node(int number)
            {
                Number = number;
                Name = "RAIDA" + number.ToString();
            }

            public EchoResponse Echo()
            {
                var client = new RestClient();
                client.BaseUrl = BaseUri;
                var request = new RestRequest("echo");
                request.Timeout = 2500;
                EchoResponse getEcho = new EchoResponse();

                Stopwatch sw = new Stopwatch();
                sw.Start();
                try
                {
                    getEcho = JsonConvert.DeserializeObject<EchoResponse>(client.Execute(request).Content);
                }
                catch (JsonException e)
                {
                    getEcho = new EchoResponse(Name, "Invalid respose", getEcho.ErrorMessage, DateTime.Now.ToString());
                    
                }
                getEcho = getEcho ?? new EchoResponse(Name, "Network problem", "Node not found", DateTime.Now.ToString());
                if (getEcho.ErrorException != null)
                        getEcho = new EchoResponse(Name, "Network problem", getEcho.ErrorMessage, DateTime.Now.ToString());

                sw.Stop();
                getEcho.responseTime = sw.Elapsed;
                
                return getEcho;
            }

            public DetectResponse Detect (CloudCoin coin)
            {
                var client = new RestClient();
                client.BaseUrl = BaseUri;
                var request = new RestRequest("detect");
                request.AddQueryParameter("nn", coin.nn.ToString());
                request.AddQueryParameter("sn", coin.sn.ToString());
                request.AddQueryParameter("an", coin.an[Number]);
                request.AddQueryParameter("pan", coin.pans[Number]);
                request.AddQueryParameter("denomination", Convert.Denomination2Int(coin.denomination).ToString());
                request.Timeout = 2000;
                DetectResponse getDetectResult = new DetectResponse();

                Stopwatch sw = new Stopwatch();
                sw.Start();
                try
                {
                    getDetectResult = JsonConvert.DeserializeObject<DetectResponse>(client.Execute(request).Content);
                }
                catch (JsonException e)
                {
                    getDetectResult = new DetectResponse(Name, coin.sn.ToString(), "Invalid response", "THe server does not respond or returns invalid data", DateTime.Now.ToString());

                }
                getDetectResult = getDetectResult ?? new DetectResponse(Name, coin.sn.ToString(), "Network problem", "Node not found", DateTime.Now.ToString());
                if (getDetectResult.ErrorException != null)
                    getDetectResult = new DetectResponse(Name, coin.sn.ToString(), "Network problem", "Problems with network connection", DateTime.Now.ToString());

                sw.Stop();
                getDetectResult.responseTime = sw.Elapsed;

                return getDetectResult;
            }

            public override string ToString()
            {
                string result = "Server: " + Name + 
                    "\nLocation: " + Location + 
                    "\nStatus: " + LastEchoStatus.status + 
                    "\nEcho: " + LastEchoStatus.responseTime.ToString("sfff") + "ms";
                return result;
            }

            public string ToString(DetectResponse res)
            {
                string result = "Server: " + Number +
                    "\nLocation: " + Location +
                    "\nStatus: " + res.status +
                    "\nEcho: " + res.responseTime.ToString("sfff") + "ms";
                return result;
            }
        }

        public class EchoResponse : RestResponse<EchoResponse>
        {
            public string server { get; set; }
            public string status { get; set; }
            public string message { get; set; }
            public string time { get; set; }
            public TimeSpan responseTime { get; set; }

            public EchoResponse()
            {
                
                server = "unknown";
                status = "unknown";
                message = "empty";
                time = "";
            }

            public EchoResponse(string server, string status, string message, string time)
            {
                this.server = server;
                this.status = status;
                this.message = message;
                this.time = time;
            }

        }

        public class DetectResponse : RestResponse<DetectResponse>
        {
            public string server { get; set; }
            public string status { get; set; }
            public string sn { get; set; }
            public string message { get; set; }
            public string time { get; set; }
            public TimeSpan responseTime { get; set; }

            public DetectResponse()
            {

                server = "unknown";
                status = "unknown";
                message = "empty";
                time = "";
            }

            public DetectResponse(string server, string sn, string status, string message, string time)
            {
                this.server = server;
                this.sn = sn;
                this.status = status;
                this.message = message;
                this.time = time;
            }

        }


    }
}
