using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Neurorehab.Scripts.Devices.Data;
using Neurorehab.Scripts.Udp;
using Neurorehab.Scripts.Utilities;
using UnityEngine;

namespace Neurorehab.Scripts.Devices.Abstracts
{
    /// <summary>
    /// The controller of all the <see cref="GenericDeviceData"/>. Responsible for creating, deleting and updating all the <see cref="GenericDeviceData"/> according to what is receiving by UDP.
    /// </summary>
    public abstract class GenericDeviceController : MonoBehaviour, ISmoothSettings, IPositionMultiplier
    {
        /// <summary>
        /// Number of samples to be used for the smoothing.
        /// </summary>
        [Range(1, 30)]
        [SerializeField]
        [Tooltip("Number of samples to be used for the smoothing ")]
        private int _numberOfSamples = 1;

        /// <summary>
        /// Position multiplyer used to convert "real world" position to "ingame" position.
        /// </summary>
        [SerializeField]
        [Range(0, 50)]
        [Tooltip("Position multiplyer used to convert 'real world' position to 'ingame' position")]
        private float _positionMultiplier = 1f;

        /// <summary>
        /// The frequency the controller checks for new devices and destroys old ones.
        /// </summary>
        public readonly float RefreshRate = .1f;

        /// <summary>
        /// The Transform where the prefabs of this device should be childed.
        /// </summary>
        public Transform PrefabParent;
        /// <summary>
        /// A list of prefabs used for this device. Usually only the first index is used, but in some cases, like in leapmotion, you need more than one prefab (left and right hands are diferent).
        /// </summary>
        public List<GameObject> Prefabs;

        /// <summary>
        /// The time it takes for the device to be destroyed after the last message received.
        /// </summary>
        [Tooltip("The time it takes for the device to be destroyed after the last message received.")]
        public float TimeToLive = 1f;

        /// <summary>
        /// A Dictionary containing all the <see cref="GenericDeviceData"/> of this controller. The Id of the device (received by UDP) is used as a key for this dictionary.
        /// </summary>
        public Dictionary<string, GenericDeviceData> DevicesData { get; private set; }

        /// <summary>
        /// The name of this device.
        /// </summary>
        protected string DeviceName { get; set; }

        /// <summary>
        /// Public property for the position multiplier. This is used to convert "real world" position to "ingame" position. In most cases, this multipliyer is suficient and no calibration is required.
        /// </summary>
        public float PositionMultiplier
        {
            get { return _positionMultiplier; }
            set { _positionMultiplier = value; }
        }

        /// <summary>
        /// Number of samples in the smoothing algorithm.
        /// </summary>
        public int NumberOfSamples
        {
            get { return _numberOfSamples; }
            set { _numberOfSamples = value; }
        }

        /// <summary>
        /// If the number of sample is greater than 1, then the Smoothing algorithm is on.
        /// </summary>
        public bool Smoothing
        {
            get { return NumberOfSamples > 1; }
        }

        /// <summary>
        /// Initializes the devices data and destroys all the <see cref="PrefabParent"/>'s children
        /// </summary>
        protected virtual void Awake()
        {
            DevicesData = new Dictionary<string, GenericDeviceData>();
            if(PrefabParent != null)
                PrefabParent.transform.DestroyChildren();
        }

        protected virtual void Start()
        {
            UdpReceiver.Instance.Init();
            StartCoroutine(CheckIfUnityObjectExists());
        }

        /// <summary>
        /// A Coroutine used to destroy outdated GameObjects and instantiate new GameObjects. Runs every <see cref="RefreshRate"/> seconds.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator CheckIfUnityObjectExists()
        {
            while (true)
            {
                DestroyOld();

                if (UdpGenericTranslator.Devices.ContainsKey(DeviceName))
                {
                    var genericDevice = UdpGenericTranslator.Devices[DeviceName];

                    CreateNewUnityObject(genericDevice);
                    CleanOldObjects(genericDevice);
                }
                else
                    CleanAllWithName(DeviceName);

                yield return new WaitForSecondsRealtime(RefreshRate);
            }
        }

        /// <summary>
        /// Removes from the <see cref="DevicesData"/> entries that are no long present in the <see cref="GenericDevice"/> of this device.
        /// </summary>
        /// <param name="genericDevice"></param>
        protected void CleanOldObjects(GenericDevice genericDevice)
        {
            var idsToKill = new List<string>();

            var ids = genericDevice.GetDeviceIds();
            foreach (var processData in DevicesData)
            {
                if (processData.Value.DeviceName == genericDevice.DeviceName && ids.Contains(processData.Key) == false)
                    idsToKill.Add(processData.Key);
            }

            foreach (var key in idsToKill)
            {
                //Debug.Log("CleanOldObjects -> Removing device data: " + key);
                DevicesData.Remove(key);
            }
        }

        /// <summary>
        /// Removes all <see cref="GenericDeviceData"/> from <see cref="DevicesData"/> with the name received in the parameters
        /// </summary>
        /// <param name="deviceName">The name of the device to be removed from <see cref="DevicesData"/></param>
        protected void CleanAllWithName(string deviceName)
        {
            var idsToKill = new List<string>();

            foreach (var genericDeviceData in DevicesData)
            {
                if(genericDeviceData.Value.DeviceName == deviceName)
                    idsToKill.Add(genericDeviceData.Key);
            }

            foreach (var key in idsToKill)
            {
                //Debug.Log("CleanAllWithName -> Removing device data: " + key);
                DevicesData.Remove(key);
            }
        }
        
        /// <summary>
        /// Support function, usually called from <see cref="CheckIfUnityObjectExists"/>. First it creates a new <see cref="GenericDeviceData"/> according to the <see cref="GenericDevice"/> received as a parameter. Then, it instantiate a Unity object for each new detection for devices of this type.
        /// </summary>
        /// <param name="genericDevice">The device being checked.</param>
        protected virtual void CreateNewUnityObject(GenericDevice genericDevice)
        {
            foreach (var values in genericDevice.GetNewDetections(DevicesData.Keys.ToList()))
            {
                var genericDeviceData = CreateGenericDeviceData(genericDevice.DeviceName, values);
                AddDeviceDataToList(values.Id, genericDeviceData);

                InstantiateUnityObject(genericDeviceData);
            }
        }

        /// <summary>
        /// It instantiates a Unity Object using the Prefabs list as a support. The default prefab is the prefab at index 0 of <see cref="Prefabs"/> but it can be redefined as any other device when calling this function.
        /// </summary>
        /// <param name="genericDeviceData">The <see cref="GenericDeviceData"/> containing the data for this device.</param>
        /// <param name="index">The index for the <see cref="Prefabs"/> list.</param>
        protected virtual void InstantiateUnityObject(GenericDeviceData genericDeviceData, int index = 0)
        {
            var prefabInstance = Instantiate(Prefabs[index]);
            var deviceUnity = prefabInstance.GetComponent<GenericDeviceUnity>();

            deviceUnity.SetDeviceData(genericDeviceData);
            prefabInstance.transform.SetParent(PrefabParent, false);
            genericDeviceData.UnityObject = prefabInstance;
        }

        /// <summary>
        /// Instanitiates the apropriated <see cref="GenericDeviceData"/> based on the <see cref="GenericDevice"/> received.
        /// </summary>
        /// <param name="deviceName">The <see cref="GenericDevice"/> that contains all the <see cref="StringValues"/> with the information received via UDP for this device.</param>
        /// <param name="values">The <see cref="StringValues"/> being process now.</param>
        /// <returns>The <see cref="GenericDeviceData"/> instantiated inside the function.</returns>
        public virtual GenericDeviceData CreateGenericDeviceData(string deviceName, StringValues values)
        {
            GenericDeviceData genericDeviceData;

            if (deviceName == Enums.Devices.neurosky.ToString())
            {
                genericDeviceData = new NeuroskyData(values.Id, deviceName, values)
                {
                    Controller = this
                };
            }
            else if (deviceName == Enums.Devices.tobiieyex.ToString())
            {
                NumberOfSamples = 5;
                genericDeviceData = new TobiiEyeXData(values.Id, deviceName, values)
                {
                    Controller = this
                };
            }
            else if (deviceName == Enums.Devices.kinect.ToString())
            {
                genericDeviceData = new KinectData(values.Id, deviceName, values)
                {
                    Controller = this
                };
            }
            else if (deviceName == Enums.Devices.bioplux.ToString())
            {
                genericDeviceData = new BiopluxData(values.Id, deviceName, values)
                {
                    Controller = this
                };
            }
            else if (deviceName == Enums.Devices.oculus.ToString())
            {
                genericDeviceData = new OculusRiftData(values.Id, deviceName, values)
                {
                    Controller = this
                };
            }
            else if (deviceName == Enums.Devices.emotiv.ToString())
            {
                genericDeviceData = new EmotivData(values.Id, deviceName, values)
                {
                    Controller = this
                };
            }
            else if (deviceName == Enums.Devices.bitalino.ToString())
            {
                genericDeviceData = new BitalinoData(values.Id, deviceName, values)
                {
                    Controller = this
                };
            }
            else if (deviceName == Enums.Devices.zephyr.ToString())
            {
                genericDeviceData = new ZephyrData(values.Id, deviceName, values)
                {
                    Controller = this
                };
            }
            else if (deviceName == Enums.Devices.leapmotion.ToString())
            {
                genericDeviceData = new LeapMotionData(values.Id, deviceName, values)
                {
                    Controller = this
                };
            }
            else
            {
                genericDeviceData = new GenericDeviceData(values.Id, deviceName, values)
                {
                    Controller = this
                };
            }
            
            return genericDeviceData;
        }

        /// <summary>
        /// Adds the received <see cref="GenericDeviceData"/> to the <see cref="DevicesData"/> dictionary.
        /// </summary>
        /// <param name="key">The key to be used in the <see cref="DevicesData"/> dictionary.</param>
        /// <param name="deviceData">The <see cref="GenericDeviceData"/> to be added to the <see cref="DevicesData"/> dictionary.</param>
        protected virtual void AddDeviceDataToList(string key, GenericDeviceData deviceData)
        {
            if (DevicesData.ContainsKey(key) == false)
                DevicesData.Add(key, deviceData);
        }

        /// <summary>
        /// Destroys the GameObjects associated with <see cref="GenericDeviceData"/> that are older than <see cref="TimeToLive"/>
        /// </summary>
        protected virtual void DestroyOld()
        {
            foreach (var genericDeviceData in DevicesData.Values)
            {
                if((DateTime.Now - genericDeviceData.LastUpdate).TotalSeconds > TimeToLive)
                    DestroyImmediate(genericDeviceData.UnityObject);
            }
        }
        
        /// <summary>
        /// Kills all threads running in the <see cref="Abstracts.GenericDeviceData"/> on this object
        /// </summary>
        private void OnApplicationQuit()
        {
            foreach (var genericDeviceData in DevicesData.Values)
            {
                genericDeviceData.StopThreads();
            }
        }
    }
}