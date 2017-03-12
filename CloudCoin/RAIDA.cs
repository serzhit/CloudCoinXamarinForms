using System;
using System.Linq;
using System.Threading.Tasks;

namespace CloudCoin
{
    public class RAIDA
    {
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
		static RAIDA theOnlyRAIDAInstance;
		public static RAIDA Instance
        {
            get
            {
				return theOnlyRAIDAInstance ?? new RAIDA();
            }
        }


		RAIDA()
		{
			for (int i = 0; i < NodesArray.Length; i++)
			{
				NodesArray[i] = new Node(i);
			}
		}

		public void getEcho(Action<Task> afterEveryNode, Action<Task[]> whenAllNodes) 
        {
            Task<RAIDAResponse>[] tasks = new Task<RAIDAResponse>[NODEQNTY];
            foreach (Node node in Instance.NodesArray)
            {
				tasks[node.Number] = node.EchoAsync();
				tasks[node.Number].Start();
				tasks[node.Number].ContinueWith(ancestor => afterEveryNode(ancestor) );
            }
			Task.Factory.ContinueWhenAll( tasks, ancestors => whenAllNodes(ancestors) );
        }

		public void Detect(CloudCoin coin, Action<Task> afterEveryNode, Action<Task[]> whenAllNodes)
        {
            Task<RAIDAResponse>[] tasks = new Task<RAIDAResponse>[NODEQNTY];
            foreach (Node node in Instance.NodesArray)
            {
				tasks[node.Number] = node.DetectAsync(coin);
				tasks[node.Number].Start();
				tasks[node.Number].ContinueWith(ancestor => afterEveryNode(ancestor) );
            }
			Task.Factory.ContinueWhenAll(tasks, ancestors => whenAllNodes(ancestors));
        }

		public void Detect(CoinStack stack, Action<Task> afterEveryNode, Action<Task[]> whenAllNodes, Action<Task[]> whenAllCoins)
        {
            Task[] checkStackTasks = new Task[stack.cloudcoin.Count()];

            for (int k = 0; k < stack.cloudcoin.Count(); k++)
            {
                var coin = stack.cloudcoin[k];
				Task<RAIDAResponse>[] checkCoinTasks = new Task<RAIDAResponse>[NODEQNTY];

                foreach (Node node in Instance.NodesArray)
                {
                    checkCoinTasks[node.Number] = node.DetectAsync(coin);
					checkCoinTasks[node.Number].Start();
					checkCoinTasks[node.Number].ContinueWith(ancestor => afterEveryNode(ancestor) );
                }
				checkStackTasks[k] = Task.Factory.ContinueWhenAll(checkCoinTasks, ancestors => whenAllNodes(ancestors) );
            }
			Task.Factory.ContinueWhenAll(checkStackTasks, ancestors => whenAllCoins(ancestors) );
        }

		FixitHelper fixer;

		public async Task fixCoin(CloudCoin brokeCoin, Action<Task> afterNodeFix)
		{
			bool[] result = new bool[NODEQNTY];

			for (int guid_id = 0; guid_id < NODEQNTY; guid_id++)
			{
				int index = guid_id; // needed for async tasks
				if (brokeCoin.detectStatus[guid_id] == CloudCoin.raidaNodeResponse.fail)
				{ // This guid has failed, get tickets 
					result[guid_id] = await ProcessFixingGUID(index, brokeCoin, afterNodeFix);
				}// end for failed guid
				else
					result[guid_id] = true;
			}//end for all the guids
			bool isGood = result.All(x => x == true);
		}

		async Task<bool> ProcessFixingGUID(int guid_id, CloudCoin returnCoin, Action<Task> afterNodeFix)
		{
			fixer = new FixitHelper(guid_id, returnCoin.an);
			RAIDAResponse[] ticketStatus = new RAIDAResponse[3];
			bool result = false;

			int corner = 1;
			while (!fixer.finnished)
			{
				string[] trustedServerAns = {
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
					var t = Instance.NodesArray[guid_id].FixAsync(fixer.currentTriad, ticketStatus[0].message, ticketStatus[1].message,
						ticketStatus[2].message, returnCoin.an[guid_id], returnCoin.sn);
					t.Start();
					var fff = await t;

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
					else
					{
						corner++;
						fixer.setCornerToCheck(corner);
						returnCoin.detectStatus[guid_id] = CloudCoin.raidaNodeResponse.error;
					}

					t.ContinueWith(ancestor => afterNodeFix(ancestor)); // Here we call callback
					//end if else fix was successful
				}//end if else one of the tickts has an error. 
			}//end while fixer not finnihsed. 
			 // the guid cannot be recovered! all corners checked

			return result;
		}

		async Task<RAIDAResponse[]> get_tickets(Node[] triad, string[] ans, int nn, int sn, CloudCoin.Denomination denomination)
		{
			RAIDAResponse[] returnTicketsStatus = new RAIDAResponse[3];

			for (int i = 0; i < 3; i++)
			{
				int index = i;
				returnTicketsStatus[i] = await triad[index].GetTicketAsync(nn, sn, ans[index], denomination);
			}

			return returnTicketsStatus;
		}//end get_tickets
	}
}
