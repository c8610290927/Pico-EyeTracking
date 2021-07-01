using System.Collections.Generic;
using System.Linq;
using Neurorehab.Scripts.Devices.Abstracts;

namespace Neurorehab.Scripts.Udp
{
    public class Notifier
    {
        /// <summary>
        /// Lock to protect the <see cref="_listeners"/> list.
        /// </summary>
        private readonly object _listenersLock = new object();
        
        /// <summary>
        /// Private field containing all the listeners for this device. Listeners are <see cref="GenericDeviceData"/> instances that must be notified when new data is received so they can interpret it.
        /// </summary>
        private readonly List<GenericDeviceData> _listeners;

        public Notifier()
        {
            _listeners = new List<GenericDeviceData>();
        }

        /// <summary>
        /// Support function called from <see cref="GenericDevice.AddValue"/>. It is called every time a value is added or updated in the <see cref="GenericDevice.Values"/> dictionary. It will call <see cref="GenericDeviceData.ProcessData"/> from a specific <see cref="GenericDeviceData"/> implementation, passing the last received <see cref="StringValues"/>.
        /// </summary>
        /// <param name="values">The latest <see cref="StringValues"/> received.</param>
        public void NotifyListeners(StringValues values)
        {
            lock (_listenersLock)
            {
                foreach (var listener in _listeners.Where(l => l.Id == values.Id)) // filter by id
                {
                    listener.ProcessData(values);
                }
            }
        }
        /// <summary>
        /// Adds the received <see cref="GenericDeviceData"/> to the listeners list.
        /// </summary>
        /// <param name="newListener">The <see cref="GenericDeviceData"/> to be added to the listeners list.</param>
        public void AddListener(GenericDeviceData newListener)
        {
            lock (_listenersLock)
            {
                if (_listeners.Contains(newListener) == false)
                    _listeners.Add(newListener);
            }
        }

        /// <summary>
        /// Removes the listener with the received id from the <see cref="_listeners"/> list
        /// </summary>
        /// <param name="id">The id of the listener to remove</param>
        public void RemoveListener(string id)
        {
            lock (_listenersLock)
            {
                var gdd = _listeners.First(l => l.Id == id);
                //Debug.Log("IsReceiving = false " + gdd.DeviceName);
                gdd.IsReceiving = false;
                _listeners.Remove(gdd);
            }
        }
    }
}