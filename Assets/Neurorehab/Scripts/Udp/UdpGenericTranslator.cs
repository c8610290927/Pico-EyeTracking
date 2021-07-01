using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Neurorehab.Scripts.Enums;
using UnityEngine;
using Logger = Neurorehab.Scripts.Utilities.Logger.Logger;

namespace Neurorehab.Scripts.Udp
{
    /// <summary>
    /// Responsible for parsing the UDP information into <see cref="StringValues"/>.
    /// </summary>
    public static class UdpGenericTranslator
    {
        /// <summary>
        /// The thread that it will be used to keep the <see cref="Devices"/> dictionary
        /// </summary>
        private static Thread _updateDevices;
        
        /// <summary>
        /// The lock used to protect the <see cref="Devices"/> dictionary for multi-thread use
        /// </summary>
        private static readonly object DevicesLock = new object();
        
        /// <summary>
        /// Backing field for <see cref="Devices"/> property
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private static readonly Dictionary<string, GenericDevice> _devices = new Dictionary<string, GenericDevice>();

        /// <summary>
        /// The publicly accessable  property
        /// </summary>
        public static Dictionary<string, GenericDevice> Devices
        {
            get
            {
                lock (DevicesLock)
                    return _devices;
            }
        }

        /// <summary>
        /// The <see cref="Devices"/> Values
        /// </summary>
        public static List<GenericDevice> DevicesValues
        {
            get
            {
                lock (DevicesLock)
                    return Devices.Values.ToList();
            }
        }

        /// <summary>
        /// Initiates the Generic Translation pipeline. This funcion creates and adds a <see cref="Udp.GenericDevice"/> to the <see cref="Devices"/> list for each new device received. Besides that, the string 'data' received is splited into several arrays (tracking type, device type, information and aditional information). These arrays are then sent to the <see cref="ParseData"/> function which stores them into <see cref="StringValues"/>. It is created one string value per 'data' string and the value is added to the device it belongs. <para>The Id is a mandatory parameter for this architecture, so if no Id is received via UDP, this function will set its id to "unkown".</para>
        /// </summary>
        /// <param name="data">A message string containing all the data received via UDP</param>
        public static void TranslateData(string data)
        {
            //Debug.Log(data);

            if (Logger.Instantiated)
                Logger.Instance.WriteLine(data + ";", "PROTOCOL_DATA");

            string[] splitCifroes = {"[$]", "[$$]", "[$$$]", ";"};
            var spliDataCifroes = data.Split(splitCifroes, StringSplitOptions.RemoveEmptyEntries);

            string[] splitVirgulas = {",", " "};

            var trackingType = spliDataCifroes[0].Split(splitVirgulas, StringSplitOptions.RemoveEmptyEntries);
            var additionalInfo = trackingType[0].Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            var deviceType = spliDataCifroes[1].Split(splitVirgulas, StringSplitOptions.RemoveEmptyEntries);
            var information = spliDataCifroes[2].Split(splitVirgulas, StringSplitOptions.RemoveEmptyEntries);

            var id = "unknown";
            var category = additionalInfo[0];
            var parametersList = new Dictionary<string, string>();
            var enumNames = Enum.GetNames(typeof(TrackingType));

            foreach (var trackingTypeParameter in additionalInfo)
            {
                if (enumNames.Any(s => s == trackingTypeParameter)) continue;

                var parameter = trackingTypeParameter.Split(new[] {"="}, StringSplitOptions.RemoveEmptyEntries);

                if (parameter[0] != TrackingTypeParameter.id.ToString())
                    parametersList.Add(parameter[0], parameter[1]);
                else
                    id = parameter[1];
            }

            if (AddDevice(deviceType[0]) == false) return;

            var device = Devices[deviceType[0]];

            device.AddValue(ParseData(category, information, id, parametersList));
        }

        /// <summary>
        /// Initializes the <see cref="UdpGenericTranslator"/>. This creates a new thread that runs each 2000ms to clean the dictionaries of old data.
        /// </summary>
        public static void InitializeTranslator()
        {
            _updateDevices = new Thread(CleanDictionary) {IsBackground = true};
            _updateDevices.Start();
        }

        /// <summary>
        /// Runs each 2000ms and removes every <see cref="GenericDevice"/> where their newest is more than 2 second old.
        /// </summary>
        private static void CleanDictionary()
        {
            while (true)
            {
                var devicesToKill = new List<string>();

                lock (DevicesLock)
                {
                    foreach (var genericDevice in Devices)
                        if ((DateTime.Now - genericDevice.Value.LastTimeReceived).Seconds > 2)
                            devicesToKill.Add(genericDevice.Key);

                    foreach (var key in devicesToKill)
                        Devices.Remove(key);
                }

                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Parses the data into a <see cref="StringValues"/> instance. 
        /// </summary>
        /// <param name="category">The string category which this information belongs to</param>
        /// <param name="information">The list of strings containing the information itself (exemple: 1,0,0,0)</param>
        /// <param name="id">The Id of the device that these values belong to.</param>
        /// <param name="parameters">A dicttionay of aditional information received with this data (example: ["side"]["left"])</param>
        /// <returns>The <see cref="StringValues"/> instance containing all the received information</returns>
        private static StringValues ParseData(string category, IList<string> information, string id,
            Dictionary<string, string> parameters)
        {
            var values = new List<string>();
            var label = information[0];
            var type = information[1];
            var time = DateTime.Now;

            for (var i = 2; i < information.Count; i++)
                values.Add(information[i]);

            return new StringValues(values, type, label, category, id, time, parameters);
        }

        /// <summary>
        /// Adds the device to the devices list and returns true if it succedes.
        /// </summary>
        /// <param name="name">The name of the device received via UDP</param>
        /// <returns>Returns true if the devices was added to the <see cref="Devices"/> dictionary</returns>
        private static bool AddDevice(string name)
        {
            lock (DevicesLock)
            {
                if (Devices.ContainsKey(name) == false)
                    Devices.Add(name, new GenericDevice(name));
            }

            return true;
        }

        /// <summary>
        /// Returns all the names of the devices in the <see cref="Devices"/> dictionary.
        /// </summary>
        /// <returns>A list of string containing all the names of the devices in the <see cref="Devices"/> dictionary. These are also the keyes for the dictionary.</returns>
        public static List<string> GetAllDevices()
        {
            lock (DevicesLock)
            {
                return Devices.Keys.ToList();
            }
        }
    }
}