using System;
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
            USA,
            Romania,
            Taiwan,
            Russia,
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
                        case 11: return Countries.USA;
                        case 12: return Countries.Romania;
                        case 13: return Countries.Taiwan;
                        case 14: return Countries.Russia;
                        case 15: return Countries.Russia;
                        case 16: return Countries.Columbia;
                        case 17: return Countries.Singapore;
                        case 18: return Countries.Germany;
                        case 19: return Countries.Canada;
                        case 20: return Countries.Venezuela;
                        case 21: return Countries.Hyperbad;
                        case 22: return Countries.USA;
                        case 23: return Countries.Switzerland;
                        case 24: return Countries.Luxenburg;
                        default: return Countries.USA;
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
                EchoResponse getEcho = new EchoResponse();
                try
                {
                    getEcho = JsonConvert.DeserializeObject<EchoResponse>(client.Execute(request).Content);
                }
                catch (JsonException e)
                {
                    getEcho = new EchoResponse(Name, "Network problem", getEcho.ErrorMessage, DateTime.Now.ToString());
                    
                }
                if (getEcho.ErrorException != null)
                        getEcho = new EchoResponse(Name, "Network problem", getEcho.ErrorMessage, DateTime.Now.ToString());

                return getEcho;
            }
        }

        public class EchoResponse : RestResponse<EchoResponse>
        {
            public string server { get; set; }
            public string status { get; set; }
            public string message { get; set; }
            public string time { get; set; }

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

    }
}
