using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace Neurorehab.Scripts.Udp
{
    /// <summary>
    /// UdpReceiver class. Responsible for listening to a UDP Port and receiving the message. The message is then sent to the <see cref="UdpGenericTranslator"/> for processing.
    /// </summary>
    public class UdpReceiver : MonoBehaviour
    {
        /// <summary>
        /// Singleton property
        /// </summary>
        public static UdpReceiver Instance { get; set; }

        /// <summary>
        /// True if there is already an instance of this script in the scene
        /// </summary>
        private static bool _iAlreadyExist = false;
        /// <summary>
        /// The IPEndPoint representing all IPs.
        /// </summary>
        private IPEndPoint _anyIp = new IPEndPoint(IPAddress.Loopback, 0);
        /// <summary>
        /// Reference for the UdpClient.
        /// </summary>
        private UdpClient _client;
        /// <summary>
        /// Used to check if there is any UDP connections active.
        /// </summary>
        private bool _isConnected;

        /// <summary>
        /// The port to be listened for the receiver.
        /// </summary>
        [Tooltip("The port to be listened for the receiver. Change this value before starting the game.")]
        public int Port = 1202;

        /// <summary>
        /// Awake function. Sets this object to DontDestroyOnLoad and initiate the listening process
        /// </summary>
        private void Awake()
        {
            print("_iAlreadyExist: " + _iAlreadyExist);
            if (_iAlreadyExist)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            _iAlreadyExist = true;


            DontDestroyOnLoad(this);
            //Init();
        }

        /// <summary>
        /// Initiate the listening process. This processes used the Microsoft UdpClient class and works asynchronously (multi-threaded). it also initializes the <see cref="UdpGenericTranslator"/>
        /// </summary>
        public void Init()
        {  
            if (_isConnected) return;

            Debug.Log("Init");
            _isConnected = true;
            UdpGenericTranslator.InitializeTranslator();

            _anyIp.Port = Port;
            _client = new UdpClient(Port) {Client = {ReceiveBufferSize = 8192}};
            _client.BeginReceive(AsyncRcvData, _client);
        }

        /// <summary>
        /// The AsyncResult callback for the BegingReceive function. It receives a message, splits it by ";" and send each line to the <see cref="UdpGenericTranslator"/>. It also transforms the message to all lower case.
        /// </summary>
        /// <param name="res">the async result of the begin receive function</param>
        private void AsyncRcvData(IAsyncResult res)
        {
            if (_isConnected == false || _client == null) return;
            byte[] data;

            try
            {
                data = _client.EndReceive(res, ref _anyIp);
            }
            catch (Exception e)
            {
                Debug.LogError(e + e.StackTrace);
                _client.BeginReceive(AsyncRcvData, _client);
                return;
            }
            _client.BeginReceive(AsyncRcvData, _client);

            var message = Encoding.UTF8.GetString(data);

            if (message == string.Empty) return;

            //Debug.Log(message);
            try
            {
                message = message.ToLower();
                var split = message.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in split)
                    if(line.Length > 1)
                        UdpGenericTranslator.TranslateData(line);
            }
            catch (Exception e)
            {
                Debug.Log(message);
                Debug.LogError(e.Message);
                Debug.LogError(e.StackTrace);
            }
        }

        private void OnDestroy()
        {
            Stop();
        }
        
        private void OnApplicationQuit()
        {
            Stop();
        }

        /// <summary>
        /// Closes any open UDP connection.
        /// </summary>
        public void Stop()
        {
            if (_client == null) return;

            //_isConnected = false;
            _client.Client.Close();
            _client.Close();
            _client = null;
        }
    }
}