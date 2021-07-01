using System;
using System.Collections.Generic;
using System.Linq;
using Neurorehab.Scripts.Devices.Abstracts;
using Neurorehab.Scripts.Udp;

namespace Neurorehab.Scripts.Devices.Data
{
    /// <summary>
    ///  Represents a set of data belonging to a emotiv device id
    /// </summary>
    public class EmotivData : GenericDeviceData, IInitializeOwnParameters
    {
        public EmotivData(string id, string deviceName, StringValues values) : base(id, deviceName, values)
        {
        }

        /// <summary>
        /// Adds the <see cref="value"/> to its respective queue with key as <see cref="label"/> and as "max_<see cref="label"/>. The second is to be used in the Specific <see cref="EmotivUnity"/> demo
        /// This function also updates the <see cref="GenericDeviceData.LastUpdate"/> property.
        /// </summary>
        /// <param name="label">The label to which this float value belongs to.</param>
        /// <param name="value">The float value for this label.</param>
        protected override void SetFloat(string label, float value)
        {
            LastUpdate = DateTime.Now;
            UpdateFloatDictionaries(label);

            var @lock = FloatLocks[label];
            var queue = FloatQueues[label];
            var queueMax = FloatQueues["max_" + label];
            var maxLock = FloatLocks["max_" + label];

            lock (@lock)
            {
                queue.Enqueue(new UdpValue
                {
                    LastTimeUpdated = DateTime.Now,
                    Value = value
                });

                while (queue.Count > 1)
                    queue.Dequeue();
            }

            lock (maxLock)
            {
                if (queueMax.Count != 0 && value > queueMax.Last().Value)
                {
                    queueMax.Dequeue();
                    queueMax.Enqueue(new UdpValue
                    {
                        LastTimeUpdated = DateTime.Now,
                        Value = value
                    });
                }
            }
        }

        /// <summary>
        /// Adds the given label and a "max_<see cref="label"/>" to the <see cref="GenericDeviceData.FloatQueues"/> dictionary if it is not already there.
        /// </summary>
        /// <param name="label">The label to be added.</param>
        protected override void UpdateFloatDictionaries(string label)
        {
            if (FloatQueues.ContainsKey(label)) return;

            FloatLocks.Add(label, new object());
            FloatQueues.Add(label, new Queue<UdpValue>());
            FloatQueues.Add("max_" + label, new Queue<UdpValue>());
            FloatQueues["max_" + label].Enqueue(new UdpValue());
            FloatLocks.Add("max_" + label, new object());
        }

        public void InitializeOwnParameters()
        {
            
        }
    }
}