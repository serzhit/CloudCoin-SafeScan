using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using RestSharp;
using Newtonsoft.Json;

namespace CloudCoin_SafeScan
{
    public partial class RAIDA
    {
        
        public event EchoStatusChangedEventHandler EchoStatusChanged;
        public event DetectCoinCompletedEventHandler DetectCoinCompleted;

        //        MainWindow mainWin = ;
        public const short NODEQNTY = 25;
        public const short MINTRUSTEDNODES4AUTH = 8;
        public Node[] NodesArray = new Node[NODEQNTY];
        public enum Countries
        {
            Australia,
            Macedonia,
            Philippines,
            Serbia,
            Bulgaria,
            Russia3,
            Ukraine,
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
        private static RAIDA theOnlyRAIDAInstance = new RAIDA();
        public static RAIDA Instance
        {
            get
            {
                return theOnlyRAIDAInstance;
            }
        }
        public EchoResponse[] EchoStatus = new EchoResponse[NODEQNTY];
        

        private RAIDA()
        {
            for(int i=0; i<NodesArray.Length;i++)
            {
                NodesArray[i] = new Node(i);
            }
        }

        public void onEchoStatusChanged(EchoStatusChangedEventArgs e)
        {
            EchoStatusChanged?.Invoke(this, e);
        }

        public void getEcho() 
        {
            Task<EchoResponse>[] tasks = new Task<EchoResponse>[NODEQNTY];
            int i = 0;
            foreach (Node node in Instance.NodesArray)
            {
                int index = i;
                tasks[i] = Task.Factory.StartNew(() => node.Echo());
                //EchoStatus[i] = await tasks[i];
                tasks[i].ContinueWith((anc) => 
                {
                    EchoStatus[index] = anc.Result;
                    onEchoStatusChanged(new EchoStatusChangedEventArgs(index));
                });
//                tasks[i].ContinueWith(ancestor => MainWindow.Instance.ShowEchoProgress(ancestor.Result, node) );
                i++;
            }
            Task.Factory.ContinueWhenAll(tasks, ancestors => onEchoStatusChanged(new EchoStatusChangedEventArgs(25)) );
        }

        public void Detect(CloudCoin coin)
        {
            Stopwatch sw = new Stopwatch();
            CoinStack stack = new CoinStack(coin);
            Task<DetectResponse>[] tasks = new Task<DetectResponse>[NODEQNTY];
            sw.Start();
            int i = 0;
            foreach(Node node in Instance.NodesArray)
            {
                int index = i;
                tasks[i] = Task.Factory.StartNew(() => node.Detect(coin));
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                tasks[i].ContinueWith((anc) =>
                {
                    coin.detectStatus[index] = anc.Result;
                });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                i++;
            }

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Factory.ContinueWhenAll(tasks, (ancs) => 
            {
                sw.Stop();
                onDetectCoinCompleted(new DetectCoinCompletedEventArgs(coin, sw));
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        public void onDetectCoinCompleted(DetectCoinCompletedEventArgs e)
        {
            DetectCoinCompleted?.Invoke(this, e);
        }
/*
        public void Detect(CoinStack stack)
        {
            CheckCoinsWindow checkCoinsWindow = new CheckCoinsWindow();
//            checkCoinsWindow.Filename.Text = FD.SafeFileName;
            checkCoinsWindow.coinsToDetect = stack.coinsInStack;
            checkCoinsWindow.Show();

            Task[] checkStackTasks = new Task[stack.cloudcoin.Count()];
            Stopwatch stackCheckTime = new Stopwatch();
            stackCheckTime.Start();
            Stopwatch[] tw = new Stopwatch[stack.cloudcoin.Count()];
            for (int k = 0; k < stack.cloudcoin.Count(); k++)
            {
                var coin = stack.cloudcoin[k];
                Task<DetectResponse>[] checkCoinTasks = new Task<DetectResponse>[NODEQNTY];
                var t = tw[k] = new Stopwatch();
                t.Start();
                foreach (Node node in Instance.NodesArray)
                {
                    checkCoinTasks[node.Number] = Task.Factory.StartNew(() => node.Detect(coin));
                    checkCoinTasks[node.Number].ContinueWith(ancestor => { checkCoinsWindow.ShowDetectProgress(ancestor.Result, node); });
                }
                checkStackTasks[k] = Task.Factory.ContinueWhenAll(checkCoinTasks, delegate { checkCoinsWindow.AllCoinDetectCompleted(coin, t); });
            }
            Task.Factory.ContinueWhenAll(checkStackTasks, delegate { checkCoinsWindow.AllStackDetectCompleted(stack, stackCheckTime); });
        }
*/
        public partial class Node
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
                        case 5: return Countries.Russia3;
                        case 6: return Countries.Switzerland;
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
                        case 23: return Countries.Ukraine;
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
                    return Country.ToString();
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
                LastEchoStatus = getEcho;
                
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
                request.AddQueryParameter("denomination", Utils.Denomination2Int(coin.denomination).ToString());
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
                    getDetectResult = new DetectResponse(Name, coin.sn.ToString(), "Invalid response", "The server does not respond or returns invalid data", DateTime.Now.ToString());
                }
                getDetectResult = getDetectResult ?? new DetectResponse(Name, coin.sn.ToString(), "Network problem", "Node not found", DateTime.Now.ToString());
                if (getDetectResult.ErrorException != null)
                    getDetectResult = new DetectResponse(Name, coin.sn.ToString(), "Network problem", "Problems with network connection", DateTime.Now.ToString());

                sw.Stop();
                getDetectResult.responseTime = sw.Elapsed;

                if (getDetectResult.status == "pass")
                {
                    coin.an[Number] = coin.pans[Number];
                }
                
                coin.detectStatus[Number] = LastDetectResult = getDetectResult;

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
