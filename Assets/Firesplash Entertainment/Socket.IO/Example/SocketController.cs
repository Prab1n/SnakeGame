/*using Firesplash.UnityAssets.SocketIO;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Fusion;
using System.Text;
using FishGame;
using Newtonsoft.Json;
using UnityEngine.Events;
using System.Security.Cryptography;

#if HAS_JSON_NET
//If Json.Net is installed, this is required for Example 6. See documentation for information on how to install Json.NET
//Please note that most recent unity versions bring Json.Net with them by default, you would only need to enable the compiler flag as documented.
using Newtonsoft.Json;
#endif
namespace CrabKing
{
    public class SocketController : NetworkBehaviour
    {
        public SocketIOCommunicator socketObj;
        public Text uiStatus, uiGreeting, uiPodName;
        public PlayerJoinData playerInfo;
        public static FishHealthEvent fishHealthEvent;
        public string apiGameName;

        public enum GameNames
        {
            FireUnicorn,
            CrabKing
        }

        public GameNames gameName;
        [SerializeField]
        string url;

        [Serializable]
        struct ItsMeData
        {
            public string version;
        }

        [Serializable]
        struct ServerTechData
        {
            public string timestamp;
            public string podName;
        }
        private string playerToken;
        private byte[] btToken;
        private Dictionary<PlayerRef, string> playerTokenDictionary;
        private Dictionary<PlayerRef, BetCounter> playerBetCounter;

        public static SocketController instance;
        private GameManager gameManager;


        public string gameId;
        private void Awake()
        {
            instance = this;
            fishHealthEvent = new FishHealthEvent();
        }
        public void AddBetCounterToSocket(PlayerRef player, BetCounter betCounter)
        {
            if (playerBetCounter == null) playerBetCounter = new Dictionary<PlayerRef, BetCounter>();
            playerBetCounter.Add(player, betCounter);
        }
        public override void Spawned()
        {
            base.Spawned();
            if (Runner.IsClient)
            {
                //Debug.Log("socket connected");

                playerToken = PlayerPrefs.GetString("token");

                btToken = Encoding.ASCII.GetBytes(playerToken);
                RPC_AddIntoPlayerScore(Runner.LocalPlayer, btToken);
            }
            else if (Runner.IsServer)
            {
                playerTokenDictionary = new Dictionary<PlayerRef, string>();

                if (gameName.Equals(GameNames.FireUnicorn))
                {

                }

                socketObj.Instance.On("connect", (string data) =>
                {
                    //Debug.Log("LOCAL: Hey, we are connected!");

                });

                socketObj.Instance.On("disconnect", (string payload) =>
                {
                    if (payload.Equals("io server disconnect"))
                    {
                        //Debug.Log("Disconnected from server.");
                        uiStatus.text = "Finished. Server closed connection.";
                    }
                    else
                    {
                        //Debug.LogWarning("We have been unexpecteldy disconnected. This will cause an automatic reconnect. Reason: " + payload);
                    }
                });
                socketObj.Instance.On("transaction:create_response", (response) =>
                {
                    TransactionCreateResponse td = JsonUtility.FromJson<TransactionCreateResponse>(response);
                    if (td.showBalance)
                    {
                        ShowBalanceToUser(td.client_id, td.balance);

                    }
                });

                socketObj.Instance.On("player:joined_response", (string payload) =>
                {
                    //Debug.Log("We received the following JSON payload from the server for example 6: " + payload);
                    PlayerJoinResponse res = JsonConvert.DeserializeObject<PlayerJoinResponse>(payload);
                    //Debug.LogWarning("Balance :: " + res.balance);
                    if (res.success)
                    {
                        ProcessRecievedData(res.client_id, res.balance);
                    }

                    //Send a goodbye to the server
                });


                socketObj.Instance.On("fish:health", (string payload) =>
                {
                    //Debug.LogWarning("This is Fish Health " + payload);
                    FishHealthGet res = JsonConvert.DeserializeObject<FishHealthGet>(payload);
                    //Debug.LogWarning("Balance :: " + res.balance);
                    if (res.success)
                    {
                        ProcessFishHealth(res);
                        fishHealthEvent.Invoke(res.health, res.fishId, res.client_id, res.balance);
                    }

                    //Send a goodbye to the server
                });

                socketObj.Instance.On("fish:changebet", (string payload) =>
                {
                    //Debug.Log("We received the following JSON payload from the server for example 6: " + payload);
                    FishHealthGet res = JsonConvert.DeserializeObject<FishHealthGet>(payload);
                    //Debug.LogWarning("Balance :: " + res.balance);
                    if (res.success)
                    {
                        ProcessFishHealth(res);

                    }

                    //Send a goodbye to the server
                });


                socketObj.Instance.On("connect_error", (err) => Debug.Log(err));
                socketObj.Instance.On("error:response", (err) => Debug.LogError(err));
                socketObj.Instance.On("connect_failed", (err) => Debug.Log(err));
                socketObj.Instance.On("disconnect", (err) => {
                    Debug.Log(err);
                    Debug.Log("Disconnect is called");
                } );
                socketObj.Instance.On("error", (err) => Debug.Log(err));

                SIOAuthPayload auth = new SIOAuthPayload();
                auth.AddElement("id", 1234); //The server will access this using socket.handshake.auth.id
                auth.AddElement("token", "sp7@fusionconnectapi#-b8986501ba2e136604859a1bed5da418-#!@#$"); //The server will access this using socket.handshake.auth.token
                                                                                                          //You could again use the component config for the target by using
                                                                                                          //sioCom.Instance.Connect(auth);
                if (NetworkManagerServerCrabAvenger.instance)
                {
                    *//*                    print("game manager");
                    *//*
                    gameName = GameNames.CrabKing;
                    NetworkManagerServerCrabAvenger.instance.playerLeftEvent.AddListener(PlayerLeftEventUpdate);

                }
                if (GameManager.instance)
                {
                    //print("game manager found");
                    gameName = GameNames.FireUnicorn;

                    gameManager = GameManager.instance;
                    gameManager.playerLeft.AddListener(PlayerLeftEventUpdate);
                }
                socketObj.Instance.Connect(url, false, auth);

            }
        }

        private void PlayerLeftEventUpdate(PlayerRef player)
        {
            playerTokenDictionary.Remove(player);
        }

        private void ProcessRecievedData(int playerId, float data)
        {
            //Debug.Log("Recieved Data :: " + data);
            *//*            playerBetCounter[playerId].UpdateCredit(data);
            *//*
            playerBetCounter[playerId].creditAmountForClient = data;

            if (gameName.Equals(GameNames.CrabKing)) playerBetCounter[playerId].UpdateCredit(data);


            if (gameName.Equals(GameNames.FireUnicorn)) gameManager.SendScoreUpdate(playerId, data);
        }

        private void ProcessFishHealth(FishHealthGet fishdata)
        {

        }


        private void ShowBalanceToUser(int playerId, double data)
        {
            //Debug.Log("Balance: " + data*100);
            if (gameName == GameNames.CrabKing) playerBetCounter[playerId].UpdateCredit(data);
            if (gameName.Equals(GameNames.FireUnicorn)) gameManager.SendScoreUpdate(playerId, data);

            ResetTransactionData();
        }

        private void ResetTransactionData()
        {
            transactionData.client_id = -1;
            transactionData.price_used = 0;
            transactionData.won_amount = 0;
            transactionData.auth = string.Empty;
        }


        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void RPC_AddIntoPlayerScore(int playerId, byte[] token)
        {
            string stToken = Encoding.ASCII.GetString(token);
            playerTokenDictionary.Add(playerId, stToken);
            ConnectPlayerToSocket(playerInfo.room_id, stToken, playerId);
        }
        TransactionCreateData transactionData;
        public void ConnectPlayerToSocket(string roomId, string playerToken, int clientId)
        {
            //Debug.Log("Adding New Player Into Socket from Socket Handler");

            PlayerJoinData pjData = new PlayerJoinData();
            pjData.auth = playerToken;
            pjData.game_id = gameId;
            pjData.room_id = "0";
            pjData.client_id = clientId;

            string json = JsonConvert.SerializeObject(pjData);

            socketObj.Instance.Emit("player:joined", json, false);
        }
        public void MakeTransaction(int playerId, float bulletCost, float fishScore = 0)
        {
            //print("after bullet hit playerId" + playerId);
            //print("after bullet cost" + bulletCost);
            //print("fish score" + fishScore);
            if (!Runner.IsServer) return;
            transactionData = new TransactionCreateData();
            transactionData.auth = playerTokenDictionary[playerId];
            transactionData.client_id = playerId;
            transactionData.price_used = bulletCost;
            transactionData.won_amount = fishScore;
            transactionData.game_id = gameId;
            transactionData.room_id = "0";

            *//* if (gameName.Equals(GameName.CrabAvengers))
             {
                 transactionData.price_used /= 100;
                 transactionData.won_amount /= 100;
             }*//*
            string json = JsonUtility.ToJson(transactionData);
            //print("json make transaction"+ json);
            socketObj.Instance.Emit("transaction:create", json, false);
            //Debug.Log("Emit transaction : " + fishScore);
        }

        public void GetHealth(int playerId, string fishName, string fishId, int bet)
        {
            //print("after bullet hit playerId" + playerId);
            //print("after bullet cost" + bulletCost);
            //print("fish score" + fishScore);
            if (!Runner.IsServer) return;
            FishHealth fishHealth = new FishHealth();
            fishHealth.auth = playerTokenDictionary[playerId];
            fishHealth.client_id = playerId;
            fishHealth.fishName = fishName;
            fishHealth.fishId = fishId;
            fishHealth.bet = bet;
            fishHealth.gameName = apiGameName;
            *//* if (gameName.Equals(GameName.CrabAvengers))
             {
                 transactionData.price_used /= 100;
                 transactionData.won_amount /= 100;
             }*//*
            string json = JsonUtility.ToJson(fishHealth);
            //print("fish shoot is"+ json);
            socketObj.Instance.Emit("fish:shoot", json, false);

            //Debug.Log("Emit transaction : " + json);
        }


        public void callback(string eventName)
        {
            socketObj.Instance.On(eventName + "_ack", (string Data) =>
            {
                Debug.Log(Data);
            });
        }

        public void MakeTransactionExample()
        {
            // if (!Runner.IsServer) return;
            TransactionCreateData transactionData = new TransactionCreateData();
            transactionData.auth = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6Ijc1M2RjM2VlLTg4OGQtNGQyMS1hYmJjLWJmNmY1MTgyNjYxZiIsImlhdCI6MTY3NzQwNjU3NSwiZXhwIjoxNjc3NDkyOTc1fQ.ai8HJoqAPHxpWmcrqA1yJcSMoMWC-teVrewwQr-nkc8";
            transactionData.client_id = 1;
            transactionData.price_used = 5;
            transactionData.won_amount = 0;
            transactionData.game_id = gameId;
            transactionData.room_id = "0";

            string json = JsonUtility.ToJson(transactionData);
            //print("click click");
            socketObj.Instance.Emit("transaction:create", json, false);
        }
        *//*    public void CreateTransaction(string token)
            {
                string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjcwOTAwNGIyLWQzMjgtNDc4Ni1iNGIxLWM3ZjNmNzIyMmI0NiIsImlhdCI6MTY3NjAyNDU3NCwiZXhwIjoxNjc2MDI0ODc0fQ.7O3-fMzqmujL4DK9TxAcODWhGER5i_xVUuLW39BSXyk";
                transactionData = new TransactionCreateData();
                transactionData.auth = token;
                transactionData.client_id = 0;
                transactionData.price_used = 5;
                transactionData.won_amount = 5;
                string json = JsonUtility.ToJson(transactionData);
                print("click click");
                sioCom.Instance.Emit("transaction:create", json, false);
            }*//*
        // Start is called before the first frame update
    }

    public class FishHealth
    {
        public string auth;
        public int client_id;
        public string fishName;
        public string fishId;
        public int bet;
        public string gameName;
    }

    public class TransactionCreateData
    {
        public float price_used;
        public string auth;
        public int client_id;
        public string game_id;
        public float won_amount;
        public string room_id;
    }
    public class TransactionCreateResponse
    {
        public bool success;
        public int client_id;
        public double balance;
        public bool showBalance;
    }

    public struct PlayerJoinResponse
    {
        public bool success;
        public string message;
        public int client_id;
        public float balance;
    }

    public struct FishHealthGet
    {
        public bool success;
        public int client_id;
        public int health;
        public int balance;
        public string fishName;
        public string fishId;
        public int bet;

    }

    public struct ChangeBet
    {
        public int totalSpent;
        public int bet;
        public int totalHit;
        public String fishName;
        public String auth;
    }

    public class FishHealthEvent : UnityEvent<int, string, int, int>
    {

    }

}*/