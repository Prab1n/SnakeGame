using Firesplash.UnityAssets.SocketIO;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Newtonsoft.Json;

#if HAS_JSON_NET
//If Json.Net is installed, this is required for Example 6. See documentation for information on how to install Json.NET
//Please note that most recent unity versions bring Json.Net with them by default, you would only need to enable the compiler flag as documented.
using Newtonsoft.Json;
#endif
namespace Multiplayer
{
    public class ExampleScript : MonoBehaviour
    {
        public SocketIOCommunicator sioCom;
        public Text uiStatus, uiGreeting, uiPodName;

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

        // Start is called before the first frame update
        void Start()
        {
            //sioCom is assigned via inspector so no need to initialize it.
            //We just fetch the actual Socket.IO instance using its integrated Instance handle and subscribe to the connect event
            sioCom.Instance.On("connect", (string data) => {
                Debug.Log("LOCAL: Hey, we are connected!");
                uiStatus.text = "Socket.IO Connected. Doing work...";

                //NOTE: All those emitted and received events (except connect and disconnect) are made to showcase how this asset works. The technical handshake is done automatically.

                //First of all we knock at the servers door
                //EXAMPLE 1: Sending an event without payload data
                sioCom.Instance.Emit("KnockKnock");
            });

            //The server will respont to our knocking by askin who we are:
            //EXAMPLE 2: Listening for an event without payload
            sioCom.Instance.On("WhosThere", (string payload) =>
            {
                //We will always receive a payload object as Socket.IO does not distinguish. In case the server sent nothing (as it will do in this example) the object will be null.
                if (payload == null)
                    Debug.Log("RECEIVED a WhosThere event without payload data just as expected.");

                //As the server just asked for who we are, let's be polite and answer him.
                //EXAMPLE 3: Sending an event with payload data
                ItsMeData me = new ItsMeData()
                {
                    version = Application.unityVersion
                };
                sioCom.Instance.Emit("ItsMe", JsonUtility.ToJson(me), false); //Please note the third parameter which is semi-required if JSON.Net is not installed

            });


            //The server will now receive our event and parse the data we sent. Then it will answer with two events.
            //EXAMPLE 4: Listening for an event with plain text payload
            sioCom.Instance.On("Welcome", (string payload) =>
            {
                Debug.Log("SERVER: " + payload);
                uiGreeting.text = payload;
            });


            //EXAMPLE 5: Listening for an event with JSON Object payload
            sioCom.Instance.On("TechData", (string payload) =>
            {
                ServerTechData srv = JsonUtility.FromJson<ServerTechData>(payload);
                Debug.Log("Received the POD name from the server. Upadting UI. Oh! It's " + srv.timestamp + " by the way.");
                uiPodName.text = "I talked to " + srv.podName;

                //Let's ask for random numbers (example 6 below)
                sioCom.Instance.Emit("SendNumbers");
            });


            //EXAMPLE 6: Listening for an event with JSON Array payload
            sioCom.Instance.On("RandomNumbers", (string payload) =>
            {
                Debug.Log("We received the following JSON payload from the server for example 6: " + payload);

                //Please note that unity's JsonUtility is not able to parse JSON arrays. You would need JSON.Net for this task.
                //This is how it works - if Json.NET is installed:
#if HAS_JSON_NET
            int[] numbers = JsonConvert.DeserializeObject<int[]>(payload);
            Debug.Log("Thanks to Json.NET we were able to decode the numbers: " + string.Join(", ", numbers));
#endif

                //Send a goodbye to the server
                sioCom.Instance.Emit("Goodbye", "Thanks for talking to me!", true); //Please note the third parameter which is semi-required if JSON.Net is not installed
            });

            sioCom.Instance.On("transaction:create_response", (string payload) =>
            {
                Debug.Log("We received the following JSON payload from the server for example 6: " + payload);


                //Send a goodbye to the server
                sioCom.Instance.Emit("Goodbye", "Thanks for talking to me!", true); //Please note the third parameter which is semi-required if JSON.Net is not installed
            });

            sioCom.Instance.On("player:joined_response", (string payload) =>
            {
                Debug.Log("We received the following JSON payload from the server for example 6: " + payload);


                //Send a goodbye to the server
                sioCom.Instance.Emit("Goodbye", "Thanks for talking to me!", true); //Please note the third parameter which is semi-required if JSON.Net is not installed
            });


            //When the conversation is done, the server will close our connection after we said Goodbye
            sioCom.Instance.On("disconnect", (string payload) => {
                if (payload.Equals("io server disconnect"))
                {
                    Debug.Log("Disconnected from server.");
                    uiStatus.text = "Finished. Server closed connection.";
                }
                else
                {
                    Debug.LogWarning("We have been unexpecteldy disconnected. This will cause an automatic reconnect. Reason: " + payload);
                }
            });

            sioCom.Instance.On("connect_error", (err) => Debug.Log(err));
            sioCom.Instance.On("connect_failed", (err) => Debug.Log(err));
            sioCom.Instance.On("disconnect", (err) => Debug.Log(err));
            sioCom.Instance.On("error", (err) => Debug.Log(err));


            //We are now ready to actually connect
            //The simnple way will use the parameters set in the inspector (or with a former call to Connect(...)):
            //sioCom.Instance.Connect();

            //For this example we will also show how to transmit a token or other data for authentication purposes:
            //PLEASE NOTE: You can only transmit primitives using the "authPayload". int, string, float...
            SIOAuthPayload auth = new SIOAuthPayload();
            auth.AddElement("id", 1234); //The server will access this using socket.handshake.auth.id
            auth.AddElement("token", "UnitySample-abc123zyx"); //The server will access this using socket.handshake.auth.token
                                                               //You could again use the component config for the target by using
                                                               //sioCom.Instance.Connect(auth);

            //But the following command shows how you can programmatically connect to any server at any given time - in this case including our previously set auth information
            //    sioCom.Instance.Connect("https://sio-v4-example.unityassets.i01.clu.firesplash.de", false, auth);
            //   sioCom.Instance.Connect("http://ec2-3-236-80-43.compute-1.amazonaws.com/api1/admin/socket.io/", false, auth);
            //  sioCom.Instance.Connect("https://s7api.super7.us", false, auth);
            sioCom.Instance.Connect("https://f905-2400-1a00-b030-d1da-41ba-997b-e23-88af.ngrok.io/", false, auth);
            // sioCom.Instance.Connect("http://43.204.194.137/api1/admin/socket.io/", false, auth);
        }

        public void MakeTransaction()
        {
            // if (!Runner.IsServer) return;
            TransactionCreateData transactionData = new TransactionCreateData();
            transactionData.auth = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6Ijc1M2RjM2VlLTg4OGQtNGQyMS1hYmJjLWJmNmY1MTgyNjYxZiIsImlhdCI6MTY3NzQwNjU3NSwiZXhwIjoxNjc3NDkyOTc1fQ.ai8HJoqAPHxpWmcrqA1yJcSMoMWC-teVrewwQr-nkc8";
            transactionData.client_id = 1;
            transactionData.price_used = 5;
            transactionData.won_amount = 0;
            transactionData.game_id = "520897e8-9ea6-40a2-8afc-f1aad79220c5";
            transactionData.room_id = "0";

            string json = JsonUtility.ToJson(transactionData);
            print("click click");
            sioCom.Instance.Emit("transaction:create", json, false);
        }


        public void ConnectPlayerToSocket()
        {
            Debug.Log("Adding New Player Into Socket from Socket Handler");
            PlayerJoinData pjData = new PlayerJoinData();
            pjData.auth = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjYwNGFlZmU1LTVlNjQtNDczNi04OTEzLTdmMzE1YzYzMDRkZiIsImlhdCI6MTY3OTgyODUzNCwiZXhwIjoxNjc5OTE0OTM0fQ.wwcPfOk0OnYSw2Ksqrjs853802DJd8-ZeqLni-oMwMM";
            pjData.game_id = "520897e8-9ea6-40a2-8afc-f1aad79220c5";
            pjData.room_id = "0";
            pjData.client_id = 1;

            string json = JsonConvert.SerializeObject(pjData);

            sioCom.Instance.Emit("player:joined", json, false);

        }



    }

    [Serializable]
    public struct PlayerJoinData
    {
        public string auth;
        public string game_id;
        public string room_id;
        public int client_id;
    }
    public struct TransactionCreateResponse
    {
        public bool success;
        public int client_id;
        public float balance;
    }
    [Serializable]
    public struct TransactionCreateData
    {
        public float price_used;
        public string auth;
        public int client_id;
        public string game_id;
        public float won_amount;
        public string room_id;
    }

}
