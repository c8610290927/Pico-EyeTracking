using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Neurorehab.Scripts.Enums;
using Neurorehab.Scripts.Udp;
using Neurorehab.Scripts.Utilities;
using UnityEngine;

namespace Neurorehab.Scripts.Devices.Abstracts
{
    /// <summary>
    /// Represents a set of data belonging to a generic device. All the data are stored in dictionaries of &lt;string,
    ///  Queue&lt;T&gt;&gt; where T is the data type, for instance, floats or Quaternions. These values are stored in
    ///  queues in case the final user desires to apply a smoothing algorithm. The queues store the last 
    /// <see cref="GenericDeviceController.NumberOfSamples"/> received.
    /// </summary>
    public class GenericDeviceData
    {
        /// <summary>
        /// The first <see cref="StringValues"/> received by this data. Used for serialization, when reloading the data.
        /// </summary>
        public StringValues StringValuesSample { get; set; }

        /// <summary>
        /// The name of the <see cref="GenericDeviceController"/> that this data belongs to.
        /// </summary>
        public string DeviceName { get; private set; }
        /// <summary>
        /// The <see cref="GenericDeviceController"/> of this set of data.
        /// </summary>
        public GenericDeviceController Controller { get; set; }
        /// <summary>
        /// The id of the <see cref="GenericDevice"/> that this data belongs to.
        /// </summary>
        public string Id { get; private set; }
        /// <summary>
        /// The last time this set of data was updated.
        /// </summary>
        public DateTime LastUpdate { get; set; }
        /// <summary>
        /// The unity GameObject that represents this set of data.
        /// </summary>
        public GameObject UnityObject { get; set; }

        /// <summary>
        /// Dictionary of parameters received in this data set.
        /// </summary>
        protected IDictionary<string, string> Parameters { get; set; }
        /// <summary>
        /// The locks that protect the <see cref="Parameters"/> queues.
        /// </summary>
        protected IDictionary<string, object> ParametersLocks { get; set; }

        /// <summary>
        /// Dictionary of float values received in this data set.
        /// </summary>
        protected IDictionary<string, Queue<UdpValue>> FloatQueues { get; set; }
        /// <summary>
        /// The locks that protect the <see cref="FloatQueues"/> queues.
        /// </summary>
        protected IDictionary<string, object> FloatLocks { get; set; }

        /// <summary>
        /// Dictionary of Vector3 positions received in this data set. Vector3 can store 2D and 3D positions.
        /// </summary>
        protected IDictionary<string, Queue<UdpPosition>> PositionQueues { get; set; }
        /// <summary>
        /// The locks that protect the <see cref="PositionQueues"/> queues.
        /// </summary>
        protected IDictionary<string, object> PositionLocks { get; set; }
        /// <summary>
        /// Dictionary of Quaternion rotations values received in this data set. Even if the received rotation is an
        ///  Euler angle, the final stored value will always be a Quaternion.
        /// </summary>
        protected IDictionary<string, Queue<UdpRotation>> RotationQueues { get; set; }
        /// <summary>
        /// The locks that protect the <see cref="RotationQueues"/> queues.
        /// </summary>
        protected IDictionary<string, object> RotationLocks { get; set; }
        /// <summary>
        /// Dictionary of Samples values received in this data set. The samples differ from the other dictionary, 
        /// since it is a List of values instead of a Queue of values. This is because the samples represent a set of
        ///  data that must be read and interpreted in a given order, so we leave to the final user to decide what to
        ///  do with it. Unlike the other dictionaries, there is no default smoothing algorithm for these values.
        /// </summary>
        protected IDictionary<string, UdpSample> SampleList { get; set; }
        /// <summary>
        /// The locks that protect the <see cref="SampleList"/> samples.
        /// </summary>
        protected IDictionary<string, object> SampleLocks { get; set; }
        /// <summary>
        /// Dictionary of boolean values received in this data set. This dictionary is also an exception, since boolean
        ///  values are discreat binary values (true or false) and there is no reason to make an average of a boolean
        ///  value.
        /// </summary>
        protected IDictionary<string, UdpBoolean> BooleanProperties { get; set; }
        /// <summary>
        /// The locks that protect the <see cref="BooleanProperties"/> values.
        /// </summary>
        protected IDictionary<string, object> BooleanLocks { get; set; }

        /// <summary>
        /// Thread responsible for keeping all the dictionaries clean.
        /// </summary>
        protected Thread CleanDictionariesThread { get; set; }

        public bool IsReceiving { get; set; }

        /// <summary>
        /// Constructor. Receives the <see cref="CpDebugger.Udp.GenericDevice"/> id and name. Besidest initializing all
        ///  the dictionaries properties, the constructor also subscribe this <see cref="GenericDeviceData"/> to its 
        /// respective <see cref="Notifier._listeners"/> list.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="deviceName"></param>
        public GenericDeviceData(string id, string deviceName, StringValues values)
        {
            StringValuesSample = values;
            Id = id;
            DeviceName = deviceName;
            IsReceiving = true;

            Parameters = new Dictionary<string, string>();
            ParametersLocks = new Dictionary<string, object>();
            RotationQueues = new Dictionary<string, Queue<UdpRotation>>();
            RotationLocks = new Dictionary<string, object>();
            BooleanProperties = new Dictionary<string, UdpBoolean>();
            BooleanLocks = new Dictionary<string, object>();
            FloatQueues = new Dictionary<string, Queue<UdpValue>>();
            FloatLocks = new Dictionary<string, object>();
            PositionQueues = new Dictionary<string, Queue<UdpPosition>>();
            PositionLocks = new Dictionary<string, object>();
            SampleList = new Dictionary<string, UdpSample>();
            SampleLocks = new Dictionary<string, object>();

            LastUpdate = DateTime.Now;
            CleanDictionariesThread = new Thread(CleanDictionaries) { IsBackground = true };
            CleanDictionariesThread.Start();


            ProcessParameters(values.Parameters);

            if (UdpGenericTranslator.Devices.ContainsKey(deviceName))
                UdpGenericTranslator.Devices[deviceName].AddListener(this);

            //Debug.Log("creating: " + deviceName);
        }

        /// <summary>
        /// Removes all data from the dictionaries that are older than <see cref="GenericDeviceController.TimeToLive"/>
        /// </summary>
        private void CleanDictionaries()
        {
            //sleeps in the beggining to give time for the software to stabilize
            Thread.Sleep(2000);

            while (true)
            {
                var keysToKill = new List<string>();

                //Debug.Log(Controller.TimeToLive);

                #region floats

                foreach (var floatQueueEntry in FloatQueues)
                {
                    if (floatQueueEntry.Key.Contains("max_"))
                        continue;

                    lock (FloatLocks[floatQueueEntry.Key])
                    {
                        while (floatQueueEntry.Value.Count != 0 && (DateTime.Now - floatQueueEntry.Value.Peek().LastTimeUpdated).Seconds >= Controller.TimeToLive)
                        {
                            floatQueueEntry.Value.Dequeue();
                        }

                        if (floatQueueEntry.Value.Count == 0)
                            keysToKill.Add(floatQueueEntry.Key);
                    }
                }

                foreach (var key in keysToKill)
                {
                    FloatQueues.Remove(key);
                    FloatLocks.Remove(key);
                }

                #endregion floats

                keysToKill.Clear();

                #region positions

                foreach (var positionQueueEntry in PositionQueues)
                {
                    lock (PositionLocks[positionQueueEntry.Key])
                    {
                        while (positionQueueEntry.Value.Count != 0 && (DateTime.Now - positionQueueEntry.Value.Peek().LastTimeUpdated).Seconds >= Controller.TimeToLive)
                        {
                            positionQueueEntry.Value.Dequeue();
                        }

                        if (positionQueueEntry.Value.Count == 0)
                            keysToKill.Add(positionQueueEntry.Key);
                    }
                }

                foreach (var key in keysToKill)
                {
                    PositionQueues.Remove(key);
                    PositionLocks.Remove(key);
                }

                #endregion positions

                keysToKill.Clear();

                #region rotations

                foreach (var rotationsQueueEntry in RotationQueues)
                {
                    lock (RotationLocks[rotationsQueueEntry.Key])
                    {
                        while (rotationsQueueEntry.Value.Count != 0 && (DateTime.Now - rotationsQueueEntry.Value.Peek().LastTimeUpdated).TotalSeconds >= Controller.TimeToLive)
                        {
                            rotationsQueueEntry.Value.Dequeue();
                        }

                        if (rotationsQueueEntry.Value.Count == 0)
                            keysToKill.Add(rotationsQueueEntry.Key);
                    }
                }

                foreach (var key in keysToKill)
                {
                    RotationQueues.Remove(key);
                    RotationLocks.Remove(key);
                }

                #endregion rotations

                keysToKill.Clear();

                #region samples

                foreach (var sampleQueueEntry in SampleList)
                {
                    lock (SampleLocks[sampleQueueEntry.Key])
                    {
                        if ((DateTime.Now - sampleQueueEntry.Value.LastTimeUpdated).Seconds >= Controller.TimeToLive)
                        {
                            keysToKill.Add(sampleQueueEntry.Key);
                        }
                    }
                }

                foreach (var key in keysToKill)
                {
                    SampleList.Remove(key);
                    SampleLocks.Remove(key);
                }

                #endregion samples

                keysToKill.Clear();

                #region booleans

                foreach (var booleanQueueEntry in BooleanProperties)
                {
                    lock (BooleanLocks[booleanQueueEntry.Key])
                    {
                        if ((DateTime.Now - booleanQueueEntry.Value.LastTimeUpdated).Seconds >= Controller.TimeToLive)
                        {
                            keysToKill.Add(booleanQueueEntry.Key);
                        }
                    }
                }

                foreach (var key in keysToKill)
                {
                    BooleanProperties.Remove(key);
                    BooleanLocks.Remove(key);
                }

                #endregion booleans
                
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Called from the <see cref="GenericDevice.NotifyListeners"/>. This function executes the 
        /// generic process
        /// data algorithms for the data received from the Rehablab Controll Panel application. 
        /// </summary>
        /// <param name="values">The <see cref="StringValues"/> containing the latest raw data
        /// received.</param>
        public void ProcessData(StringValues values)
        {
            var infoType = Parser.StringToEnum<InformationType>(values.InformationType);

            switch (infoType)
            {
                case InformationType.rotation:
                    ProcessRotation(values);
                    break;
                case InformationType.position:
                    ProcessPosition(values);
                    break;
                case InformationType.@bool:
                    ProcessBoolean(values);
                    break;
                case InformationType.value:
                    ProcessFloat(values);
                    break;
                case InformationType.sample:
                    ProcessSample(values);
                    break;
                case InformationType.unknown:
                    break;
            }
        }

        /// <summary>
        /// Get a list of all labels existing in this set of data.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetLabelsList()
        {
            var labels = new List<string>();

            labels.AddRange(RotationQueues.Keys.ToList());

            foreach (var key in PositionQueues.Keys)
            {
                if(labels.Contains(key)) continue;
                labels.Add(key);
            }

            foreach (var key in BooleanProperties.Keys)
            {
                if (labels.Contains(key)) continue;
                labels.Add(key);
            }

            foreach (var key in FloatQueues.Keys)
            {
                if (labels.Contains(key)) continue;
                labels.Add(key);
            }

            foreach (var key in SampleList.Keys)
            {
                if (labels.Contains(key)) continue;
                labels.Add(key);
            }
            return labels;
        }

        /// <summary>
        /// Returns true if any of the dictionaries has any value
        /// </summary>
        /// <returns></returns>
        public bool HasAnyValue()
        {
            return HasRotations() || HasPositions() || HasFloats() || HasBooleans() || HasSamples();
        }

        #region parameters
        
        /// <summary>
        /// Processes all the device parameters
        /// </summary>
        /// <param name="parameters"></param>
        private void ProcessParameters(Dictionary<string, string> parameters)
        {
            foreach (var parameter in parameters)
            {
                if (ContainsParameter(parameter.Key)) continue;
                SetParameter(parameter);
            }
        }

        /// <summary>
        /// Returns the device parameter according to the given key
        /// </summary>
        /// <param name="key">the dictionary key for this parameter</param>
        /// <returns></returns>
        public virtual string GetParameter(string key)
        {
            if (ContainsParameter(key) == false) return "";

            var @lock = ParametersLocks[key];


            lock (@lock)
                return Parameters[key];
            
        }

        /// <summary>
        /// Adds the parameter to its paramters dictionary. Can be redefined in children class to change smoothing logic.
        /// </summary>
        /// <param name="param">The KeyValuePair parameter</param>
        protected virtual void SetParameter(KeyValuePair<string, string> param)
        {
            UpdateParameterDictionaries(param.Key);

            var @lock = ParametersLocks[param.Key];

            lock (@lock)
            {
                Parameters[param.Key] = param.Value;
            }
                
        }

        /// <summary>
        /// Checks if this set of data has parameter information about the given string.
        /// </summary>
        /// <param name="key">The key to search for the parameter</param>
        /// <returns>Returns true if the <see cref="Parameters"/> dictionary has any key equals to the given label.</returns>
        public bool ContainsParameter(string key)
        {
            return Parameters.ContainsKey(key);
        }

        /// <summary>
        /// Adds the given parameter to the <see cref="Parameters"/> dictionary if it is not present.
        /// </summary>
        /// <param name="key">The parameter to be added.</param>
        protected virtual void UpdateParameterDictionaries(string key)
        {
            lock (ParametersLocks)
            {
                if (ContainsParameter(key)) return;

                ParametersLocks.Add(key, new object());
                Parameters.Add(key, "");
            }
        }

        #endregion

        #region rotations

        /// <summary>
        /// Generic process rotation function. Can be redefined in children classes to change the parsing logic. The generic function can parse 3 floats (Euler angles, representing x,y,z) into Quaternions. It can also do a direct translation from 4 floats (Quaternion coordinates x,y,z,w) to a Quaternion instance. Calls the <see cref="SetRotation"/> function to add this rotation to the <see cref="RotationQueues"/> dictionary.
        /// </summary>
        /// <param name="values">The <see cref="StringValues"/> containing the raw data to be parsed.</param>
        protected virtual void ProcessRotation(StringValues values)
        {
            if (values.Values.Count == 4)
                SetRotation(
                    values.InformationLabel,
                    new Quaternion(
                        float.Parse(values.Values[0]),
                        float.Parse(values.Values[1]),
                        float.Parse(values.Values[2]),
                        float.Parse(values.Values[3])
                    ));
            else if (values.Values.Count == 3)
                SetRotation(
                    values.InformationLabel,
                    Quaternion.Euler(
                        float.Parse(values.Values[0]),
                        float.Parse(values.Values[1]),
                        float.Parse(values.Values[2])
                    )
                );
        }

        /// <summary>
        /// Returns the Quaternion representing the rotation of the given label. Can be redefined in children classes to change the smoothing logic. Current smoothing logic is as follows:<para><example> <see cref="GenericDeviceController.Smoothing"/> ? <see cref="Smoother.Average(Queue{Quaternion})"/> : queue.LastOrDefault()</example></para>
        /// </summary>
        /// <param name="label">The label to get the rotation.</param>
        /// <returns>A Quaternion representing the rotation of the given label.</returns>
        public virtual Quaternion GetRotation(string label)
        {
            if (RotationLocks.ContainsKey(label) == false) return Quaternion.identity;

            var @lock = RotationLocks[label];
            var queue = RotationQueues[label];

            lock (@lock)
            {
                return Controller.Smoothing ? Smoother.Average(queue) : queue.Count == 0 ? Quaternion.identity : queue.Last().Rotation;
            }
        }

        /// <summary>
        /// Adds the Quaternion rotation to its respective queue. Can be redefined in children class to change smoothing logic. This function also updates the <see cref="LastUpdate"/> property.
        /// </summary>
        /// <param name="label">The label to which this rotation belongs to.</param>
        /// <param name="value">The Quaternion rotation of this label.</param>
        protected virtual void SetRotation(string label, Quaternion value)
        {
            LastUpdate = DateTime.Now;
            UpdateRotationDictionaries(label);

            var @lock = RotationLocks[label];
            var queue = RotationQueues[label];

            lock (@lock)
            {
                queue.Enqueue(new UdpRotation
                {
                    LastTimeUpdated = DateTime.Now,
                    Rotation = value
                });
                while (Controller != null && queue.Count > Controller.NumberOfSamples)
                    queue.Dequeue();
            }
        }

        /// <summary>
        /// Checks if this set of data has rotation information about the given label.
        /// </summary>
        /// <param name="label">The label to search for rotations</param>
        /// <returns>Returns true if the <see cref="RotationQueues"/> dictionary has any key equals to the given label.</returns>
        public bool ContainsRotation(string label)
        {
            return RotationQueues.ContainsKey(label);
        }

        /// <summary>
        /// returns True if the rotations dictionary has any value
        /// </summary>
        /// <returns></returns>
        public bool HasRotations()
        {
            return RotationQueues.Count > 0;
        }

        /// <summary>
        /// Adds the given label to the <see cref="RotationQueues"/> dictionary if it is not present.
        /// </summary>
        /// <param name="label">The label to be added.</param>
        protected virtual void UpdateRotationDictionaries(string label)
        {
            lock (RotationLocks)
            {
                if (ContainsRotation(label)) return;

                RotationLocks.Add(label, new object());
                RotationQueues.Add(label, new Queue<UdpRotation>());
            } 
        }

        #endregion rotations

        #region booleans

        /// <summary>
        /// Generic process boolean function. Can be redefined in children classes. Calls the <see cref="SetBoolean"/> function to add this boolean to the <see cref="BooleanProperties"/> dictionary.
        /// </summary>
        /// <param name="values">The <see cref="StringValues"/> containing the raw data to be parsed.</param>
        protected virtual void ProcessBoolean(StringValues values)
        {
            SetBoolean(values.InformationLabel, BoolParser.GetValue(values.Values[0]));
        }

        /// <summary>
        /// Returns the boolean value of the given label. Can be redefined in children classes.
        /// </summary>
        /// <param name="label">The label to get the boolean</param>
        /// <returns>A boolean value of the given label.</returns>
        public virtual bool GetBoolean(string label)
        {
            if (BooleanProperties.ContainsKey(label) == false) return false;

            return BooleanProperties[label].Boolean;
        }

        /// <summary>
        /// Adds the boolean value to its respective queue. Can be redefined in children class. This function also updates the <see cref="LastUpdate"/> property.
        /// </summary>
        /// <param name="label">The label to which this boolean belongs to.</param>
        /// <param name="value">The boolean value of this label.</param>
        protected virtual void SetBoolean(string label, bool value)
        {
            LastUpdate = DateTime.Now;
            UpdateBooleanDictionaries(label);

            lock (BooleanLocks[label])
            {
                BooleanProperties[label] = new UdpBoolean
                {
                    Boolean = value,
                    LastTimeUpdated = DateTime.Now
                };
            }
        }

        /// <summary>
        /// Checks if this set of data has boolean information about the given label.
        /// </summary>
        /// <param name="label">The label to search for booleans</param>
        /// <returns>Returns true if the <see cref="BooleanProperties"/> dictionary has any key equals to the given label.</returns>
        public bool ContainsBoolean(string label)
        {
            return BooleanProperties.ContainsKey(label);
        }
        
        /// <summary>
        /// returns True if the booleans dictionary has any value
        /// </summary>
        /// <returns></returns>
        public bool HasBooleans()
        {
            return BooleanProperties.Count > 0;
        }

        /// <summary>
        /// Adds the given label to the <see cref="BooleanProperties"/> dictionary if it is not present.
        /// </summary>
        /// <param name="label">The label to be added.</param>
        public virtual void UpdateBooleanDictionaries(string label)
        {
            lock (BooleanLocks)
            {
                if (ContainsBoolean(label)) return;

                BooleanLocks.Add(label, new object());
                BooleanProperties.Add(label, new UdpBoolean());
            }
        }

        #endregion booleans

        #region floats

        /// <summary>
        /// Generic process float function. Can be redefined in children classes to change the parsing logic. 
        /// </summary>
        /// <param name="values">The <see cref="StringValues"/> containing the raw data to be parsed.</param>
        protected virtual void ProcessFloat(StringValues values)
        {
            var value = 0f;
            if (values.Values.Count > 0)
                value = float.Parse(values.Values[0]);

            SetFloat(values.InformationLabel, value);
        }

        /// <summary>
        /// Returns the float value of the given label. Can be redefined in children classes to change the smoothing logic. Current smoothing logic is as follows:<para><example> <see cref="GenericDeviceController.Smoothing"/> ? <see cref="Smoother.Average(Queue{Vector3})"/> : queue.LastOrDefault()</example></para>
        /// </summary>
        /// <param name="label">The label to get the float value.</param>
        /// <returns>A float value of the given label.</returns>
        public virtual float GetFloat(string label)
        {
            if (FloatQueues.ContainsKey(label) == false) return 0f;

            var @lock = FloatLocks[label];
            var queue = FloatQueues[label];

            lock (@lock)
            {
                return Controller.Smoothing ? Smoother.Average(queue) : queue.Count == 0 ? 0f : queue.Last().Value;
            }
        }

        /// <summary>
        /// Adds the float value to its respective queue. Can be redefined in children class to change smoothing logic. This function also updates the <see cref="LastUpdate"/> property.
        /// </summary>
        /// <param name="label">The label to which this float value belongs to.</param>
        /// <param name="value">The float value of this label.</param>
        protected virtual void SetFloat(string label, float value)
        {
            LastUpdate = DateTime.Now;
            UpdateFloatDictionaries(label);

            var @lock = FloatLocks[label];
            var queue = FloatQueues[label];

            lock (@lock)
            {
                queue.Enqueue(new UdpValue
                {
                    LastTimeUpdated = DateTime.Now,
                    Value = value
                });
                while (Controller != null && queue.Count > Controller.NumberOfSamples)
                    queue.Dequeue();
            }
        }

        /// <summary>
        /// Checks if this set of data has any float value for the given label.
        /// </summary>
        /// <param name="label">The label to search for rotations</param>
        /// <returns>Returns true if the <see cref="RotationQueues"/> dictionary has any key equals to the given label.</returns>
        public bool ContainsFloat(string label)
        {
            return FloatQueues.ContainsKey(label);
        }
        
        /// <summary>
        /// returns True if the float dictionary has any value
        /// </summary>
        /// <returns></returns>
        public bool HasFloats()
        {
            return FloatQueues.Count > 0;
        }

        /// <summary>
        /// Adds the given label to the <see cref="FloatQueues"/> dictionary if it is not present.
        /// </summary>
        /// <param name="label">The label to be added.</param>
        protected virtual void UpdateFloatDictionaries(string label)
        {
            lock (FloatLocks)
            {
                if (ContainsFloat(label)) return;

                FloatLocks.Add(label, new object());
                FloatQueues.Add(label, new Queue<UdpValue>());
            }
        }

        #endregion floats

        #region position

        /// <summary>
        /// Generic process position function. Can be redefined in children classes to change the parsing logic. The generic function can parse 3 floats (3D position, represented by x, y and z) into Vector3. It can also convert 2D positions (x,y) into a vector 3 with z = 0f. Calls the <see cref="SetPosition"/> function to add this position to the <see cref="PositionQueues"/> dictionary.
        /// </summary>
        /// <param name="values">The <see cref="StringValues"/> containing the raw data to be parsed.</param>
        protected virtual void ProcessPosition(StringValues values)
        {
            var x = 0f;
            var y = 0f;
            var z = 0f;

            if (values.Values.Count < 2) return;

            float.TryParse(values.Values[0], out x);
            float.TryParse(values.Values[1], out y);

            if (values.Values.Count > 2)
                float.TryParse(values.Values[2], out z);

            SetPosition(values.InformationLabel, new Vector3(x, y, z));
        }

        /// <summary>
        /// Returns the Vector3 representing the position of the given label. Can be redefined in children classes to change the smoothing logic. Current smoothing logic is as follows:<para><example> <see cref="GenericDeviceController.Smoothing"/> ? <see cref="Smoother.Average(System.Collections.Generic.Queue{Neurorehab.Scripts.Devices.UdpPosition})"/> : queue.LastOrDefault()</example></para>
        /// </summary>
        /// <param name="label">The label to get the rotation.</param>
        /// <returns>A Vector3 representing the rotation of the given label.</returns>
        public virtual Vector3 GetPosition(string label)
        {
            if (PositionLocks.ContainsKey(label) == false) return Vector3.zero;

            var @lock = PositionLocks[label];
            var queue = PositionQueues[label];

            lock (@lock)
            {
                return (Controller.Smoothing ? 
                    Smoother.Average(queue) : queue.Count == 0 ? 
                    Vector3.zero : queue.Last().Position) * Controller.PositionMultiplier;
            }
        }

        /// <summary>
        /// Adds the Vector3 positionto its respective queue. Can be redefined in children class to change smoothing logic. This function also updates the <see cref="LastUpdate"/> property.
        /// </summary>
        /// <param name="label">The label to which this position belongs to.</param>
        /// <param name="value">The Vector3 position of this label.</param>
        protected virtual void SetPosition(string label, Vector3 value)
        {
            LastUpdate = DateTime.Now;
            UpdatePositionDictionaries(label);

            var @lock = PositionLocks[label];
            var queue = PositionQueues[label];

            lock (@lock)
            {
                queue.Enqueue(new UdpPosition
                {
                    LastTimeUpdated = DateTime.Now,
                    Position = value
                });
                while (Controller != null && queue.Count > Controller.NumberOfSamples)
                    queue.Dequeue();
            }
        }
        
        /// <summary>
        /// Checks if this set of data has position information about the given label.
        /// </summary>
        /// <param name="label">The label to search for positions</param>
        /// <returns>Returns true if the <see cref="PositionQueues"/> dictionary has any key equals to the given label.</returns>
        public bool ContainsPosition(string label)
        {
            return PositionQueues.ContainsKey(label);
        }

        /// <summary>
        /// returns True if the positions dictionary has any value
        /// </summary>
        /// <returns></returns>
        public bool HasPositions()
        {
            return PositionQueues.Count > 0;
        }

        /// <summary>
        /// Adds the given label to the <see cref="PositionQueues"/> dictionary if it is not present.
        /// </summary>
        /// <param name="label">The label to be added.</param>
        protected virtual void UpdatePositionDictionaries(string label)
        {
            lock (PositionLocks)
            {
                if (ContainsPosition(label)) return;

                PositionLocks.Add(label, new object());
                PositionQueues.Add(label, new Queue<UdpPosition>());
            }
        }

        #endregion positions

        #region samples

        /// <summary>
        /// Generic process sample function. Can be redefined in children classes. The generic function parses the list of strings representing a series of float values into a list of floats. Calls the <see cref="SetSample"/> function to add this sample to the <see cref="SampleList"/> dictionary.
        /// </summary>
        /// <param name="values">The <see cref="StringValues"/> containing the raw data to be parsed.</param>
        protected virtual void ProcessSample(StringValues values)
        {
            var sampleValues = values.Values.Select(float.Parse).ToList();

            SetSample(values.InformationLabel, sampleValues);
        }

        /// <summary>
        /// Returns the sample list representing the last received sample of the given label. Can be redefined in children classes.
        /// </summary>
        /// <param name="label">The label to get the sample.</param>
        /// <returns>A float list representing the sample of the given label.</returns>
        public virtual List<float> GetSample(string label)
        {
            lock (SampleLocks[label])
                return SampleList.ContainsKey(label) ? SampleList[label].Sample : new List<float>();
        }

        /// <summary>
        /// Adds the list of floats sample to its respective queue. Can be redefined in children class. This function also updates the <see cref="LastUpdate"/> property.
        /// </summary>
        /// <param name="label">The label to which this sample belongs to.</param>
        /// <param name="value">The Quaternion sample of this label.</param>
        public virtual void SetSample(string label, List<float> values)
        {
            LastUpdate = DateTime.Now;
            UpdateSampleDictionaries(label);

            var @lock = SampleLocks[label];

            lock (@lock)
            {
                SampleList[label] = new UdpSample
                {
                    LastTimeUpdated = DateTime.Now,
                    Sample = values
                };
            }
        }

        /// <summary>
        /// Checks if this set of data has sample information about the given label.
        /// </summary>
        /// <param name="label">The label to search for samples</param>
        /// <returns>Returns true if the <see cref="SampleList"/> dictionary has any key equals to the given label.</returns>
        public bool ContainsSample(string label)
        {
            return SampleList.ContainsKey(label);
        }

        /// <summary>
        /// returns True if the samples dictionary has any value
        /// </summary>
        /// <returns></returns>
        public bool HasSamples()
        {
            return SampleList.Count > 0;
        }

        /// <summary>
        /// Adds the given label to the <see cref="SampleList"/> dictionary if it is not present.
        /// </summary>
        /// <param name="label">The label to be added.</param>
        public virtual void UpdateSampleDictionaries(string label)
        {
            lock (SampleLocks)
            {
                if (ContainsSample(label)) return;

                SampleList.Add(label, new UdpSample());
                SampleLocks.Add(label, new object());
            }
        }

        #endregion samples

        /// <summary>
        /// Stops all currently running threads on this object
        /// </summary>
        public void StopThreads()
        {
            CleanDictionariesThread.Abort();
        }
    }
}