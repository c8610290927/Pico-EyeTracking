using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using Neurorehab.Scripts.Devices.Abstracts;

namespace Neurorehab.Scripts.Udp
{
    public class GenericDevice
    {

        /// <summary>
        /// <see cref="Notifier"/> manager for this <see cref="GenericDevice"/>
        /// </summary>
        private Notifier _notifier { get; set; }

        /// <summary>
        /// Backing field for <see cref="Values"/>. A Dictionary of &lt;string, <see cref="StringValues"/>&gt;. Contains all the data received for this particular device type. It can have more than one physical device stored in it. They key of the dictionary is a string with the format Id_InformationLabel_InformationType. <para>Example: "123123_waist_rotation".</para> 
        /// </summary>
        protected readonly Dictionary<string, StringValues> _values;
        /// <summary>
        /// Lock to proctect the <see cref="_values"/> dictionary in multithread access.
        /// </summary>
        private readonly object _valuesLock = new object();

        /// <summary>
        /// DateTime storing the last time any <see cref="StringValues"/> was added to the <see cref="Values"/> dictionary. It represents the last time this device received any kind of data and it is used to determine if it must be destroyed or not.
        /// </summary>
        public DateTime LastTimeReceived { get; set; }

        /// <summary>
        /// Public property for <see cref="Values"/>. A Dictionary of &lt;string, <see cref="StringValues"/>&gt;. Contains all the data received for this particular device type. It can have more than one physical device stored in it. They key of the dictionary is a string with the format Id_InformationLabel_InformationType.<para> Example: "123123_waist_rotation".</para>
        /// </summary>
        private Dictionary<string, StringValues> Values { get { return _values; } }

        /// <summary>
        /// String containing the device name. 
        /// </summary>
        public string DeviceName { get; private set; }
        
        /// <summary>
        /// How long are the data kept in the dictionaries. Expressed in seconds
        /// </summary>
        private float TimeToLive { get; set; }

        /// <summary>
        /// Public constructor.
        /// </summary>
        /// <param name="deviceName">The name of this device.</param>
        public GenericDevice(string deviceName)
        {
            _values = new Dictionary<string, StringValues>();
            DeviceName = deviceName;

            TimeToLive = 1/60f;

            var updateDeviceValues = new Thread(CleanDictionary) { IsBackground = true };
            updateDeviceValues.Start();

            _notifier = new Notifier();
        }

        /// <summary>
        /// Adds a <see cref="StringValues"/> to the <see cref="Values"/> dictionary. If there is already a <see cref="StringValues"/> with the same key, the function will update its value instead. It also updates the last time it received data and calls <see cref="Notifier.NotifyListeners"/>.
        /// </summary>
        /// <param name="value"><see cref="StringValues"/> to be added to the <see cref="Values"/> dictionary.</param>
        public void AddValue(StringValues value)
        {
            lock (_valuesLock)
            {
                if (Values.ContainsKey(value.Key) == false)
                    Values.Add(value.Key, value);
                else
                    Values[value.Key] = value;
            }

            LastTimeReceived = DateTime.Now;

            _notifier.NotifyListeners(value);
        }

        /// <summary>
        /// Gets the <see cref="StringValues"/> associeted with the composite key received in the parameters. Returns null if there is no value for the specified key. <para>The key is "id_label_type".</para>
        /// </summary>
        /// <param name="id">The Id to be used in the key. Example "123".</param>
        /// <param name="label">The label to be used in the key. Example "waist".</param>
        /// <param name="type">The type to be used in the key. Example "rotation".</param>
        /// <returns>returns a <see cref="StringValues"/> containing the values of the provited key. If there is no value for that key, it will return null.</returns>
        [CanBeNull]
        protected StringValues GetValues(string id, string label, string type)
        {
            var key = id + "_" + label + "_" + type;

            lock (_valuesLock)
            {
                if (Values.ContainsKey(key)) return Values[key];
            }

            return null;
        }

        /// <summary>
        /// Gets all the ids of the physical devices connected to this <see cref="CpDebugger.Udp.GenericDevice"/> that are still active. For example, it will return the keys for all kinect skeletons received via UDP that are still active.
        /// </summary>
        /// <returns>Returns a list of strings containing all the Ids. If there is no Ids for the given device, it will return an empty list.</returns>
        public virtual List<string> GetDeviceIds()
        {
            var idList = new List<string>();

            lock (_valuesLock)
            {
                foreach (var value in Values.Values)
                {
                    if (idList.Contains(value.Id)) continue;
                    idList.Add(value.Id);
                }
            }
            return idList;
        }

        /// <summary>
        /// Gets all the Categories of data received by the given Id. 
        /// </summary>
        /// <param name="id">The device Id to search for its categories.</param>
        /// <returns>return a list of strings with all categories. If no category is found, it will return an empty list.</returns>
        public List<string> GetCategoryById(string id)
        {
            var categoryList = new List<string>();
            lock (_valuesLock)
            {
                foreach (var value in Values.Values)
                {
                    if (value.Id != id || categoryList.Contains(value.InformationCategory)) continue;

                    categoryList.Add(value.InformationCategory);
                }
            }
            return categoryList;
        }

        /// <summary>
        /// Gets the list of all Labels belonging to a certain device in the specified category. 
        /// </summary>
        /// <param name="id">The device Id to search for its categories.</param>
        /// <param name="category">The category to search for its labels.</param>
        /// <returns>Returns a list of strings containing all the Labels of this search</returns>
        public List<string> GetLabel(string id, string category)
        {
            var labelList = new List<string>();
            lock (_valuesLock)
            {
                foreach (var value in Values.Values)
                {
                    if (value.Id != id) continue;
                    if (value.InformationCategory != category || labelList.Contains(value.InformationLabel)) continue;
                    labelList.Add(value.InformationLabel);
                }
            }
            return labelList;
        }

        /// <summary>
        /// Gets the list of all data type associated with this label.
        /// </summary>
        /// <param name="id">The device Id to search for its categories.</param>
        /// <param name="category">The category to search for its labels.</param>
        /// <param name="label">The label to search for its types.</param>
        /// <returns>Returns a list of string contaning all the Types of this search</returns>
        public List<string> GetTypeList(string id, string category, string label)
        {
            var typeList = new List<string>();
            lock (_valuesLock)
            {
                foreach (var value in Values.Values)
                {
                    if (value.Id != id) continue;
                    if (value.InformationCategory != category) continue;
                    if (value.InformationLabel != label || typeList.Contains(value.InformationType)) continue;
                    typeList.Add(value.InformationType);
                }
            }
            return typeList;
        }

        /// <summary>
        /// Returns a list containing all <see cref="StringValues.Values"/> for this search.
        /// </summary>
        /// <param name="id">The device Id to search for its categories.</param>
        /// <param name="category">The category to search for its labels.</param>
        /// <param name="label">The label to search for its types.</param>
        /// <param name="type">The type to search for its values</param>
        /// <returns>Returns a list of strings containing all the Values of this search.</returns>
        public List<string> GetValuesList(string id, string category, string label, string type)
        {
            var valuesList = new List<string>();

            lock (_valuesLock)
            {
                foreach (var value in Values.Values)
                {
                    if (value.Id != id) continue;
                    if (value.InformationCategory != category) continue;
                    if (value.InformationLabel != label) continue;
                    if (value.InformationType != type) continue;
                    valuesList.AddRange(value.Values);
                }
            }
            return valuesList;
        }

        /// <summary>
        /// Gets all the parameters associated with the corrent selections.
        /// </summary>
        /// <param name="id">The device Id to search for its categories.</param>
        /// <param name="category">The category to search for its labels.</param>
        /// <param name="label">The label to search for its types.</param>
        /// <param name="type">The type to search for its values</param>
        /// <returns>Returns a list of all the parameters associated with the corrent selections. These parameters can be, for instance, the side of the hand of which the LeapMotion bone belongs to: "side=left"</returns>
        public Dictionary<string, string> GetParameterList(string id, string category, string label, string type)
        {
            var paramsList = new Dictionary<string, string>();

            lock (_valuesLock)
            {
                foreach (var value in Values.Values)
                {
                    if (value.Id != id) continue;
                    if (value.InformationCategory != category) continue;
                    if (value.InformationLabel != label) continue;
                    if (value.InformationType != type) continue;
                    paramsList = value.Parameters;
                }
            }
            return paramsList;
        }

        /// <summary>
        /// Runs outside of the Main Thread. This function keeps the <see cref="Values"/> dictionary clean. It will delete entries that are more than 2 seconds old and remove its associated listeners from the <see cref="Notifier._listeners"/> list. <para>This thread runs forever and is executed once every 2 seconds.</para>
        /// </summary>
        private void CleanDictionary()
        {
            while (true)
            {
                var valuesToKill = new List<StringValues>();

                lock (_valuesLock)
                {
                    valuesToKill.AddRange(Values.Values.Where(value => (DateTime.Now - value.LastTimeReceived).TotalSeconds > TimeToLive));
                }

                foreach (var value in valuesToKill)
                {
                    lock (_valuesLock)
                    {
                        //if (Values.Count == 1 && (DateTime.Now - value.LastTimeReceived).TotalSeconds < 5) continue;

                        Values.Remove(value.Key);
                    }

                    List<string> allIds;
                    lock (_valuesLock)
                        allIds = GetDeviceIds();

                    if (allIds.Contains(value.Id)) continue;

                    _notifier.RemoveListener(value.Id);

                }

                Thread.Sleep((int)(TimeToLive * 1000));
            }
        }

        /// <summary>
        /// Adds a <see cref="GenericDeviceData"/> to the list of <see cref="_notifier"/> 
        /// </summary>
        /// <param name="genericDeviceData"></param>
        public void AddListener(GenericDeviceData genericDeviceData)
        {
            _notifier.AddListener(genericDeviceData);
        }

        /// <summary>
        /// Gets a list of <see cref="StringValues"/> from new Ids. This function is called from <see cref="GenericDeviceController"/> class and it is used to create new <see cref="GenericDeviceData"/> instance for the new Ids.
        /// </summary>
        /// <param name="knownKeys">A list of all ids already known by the <see cref="GenericDeviceController"/>. If an empty is is received, then all the Ids will be returned.</param>
        /// <returns>A list of <see cref="StringValues"/> containing all the Values which their Id is diferent from all the Ids received in the parameters.</returns>
        public virtual List<StringValues> GetNewDetections(List<string> knownKeys)
        {
            var newDetections = new List<StringValues>();
            foreach (var deviceId in UdpGenericTranslator.Devices[DeviceName].GetDeviceIds())
            {
                if (knownKeys.Contains(deviceId)) continue;

                var category = GetCategoryById(deviceId).FirstOrDefault();
                var label = GetLabel(deviceId, category).FirstOrDefault();
                var type = GetTypeList(deviceId, category, label).FirstOrDefault();
                if (type == "") continue;

                newDetections.Add(GetValues(deviceId, label, type));
            }

            return newDetections;
        }


        public StringValues GetLastReceivedStringValues(string id)
        {
            var values = new List<StringValues>(_values.Values);

            return values.OrderByDescending(sv => sv.LastTimeReceived).First(val => val.Id == id);
        }
    }
}