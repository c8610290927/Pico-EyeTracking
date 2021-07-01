using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Neurorehab.Scripts.Udp
{
    /// <summary>
    /// <see cref="StringValues"/> class. Stores the information received using the UDP Protocol. Each Instance of a <see cref="StringValues"/> stores one message with the format: <para>[$]trackingCategory|adicionalInformation,[$$]device,[$$$]label,informationType,x,y,z,{...};</para>
    /// </summary>
    [Serializable]
    public class StringValues : ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<string> _values;

        [SerializeField]
        private string _informationType;
        [SerializeField]
        private string _informationLabel;
        [SerializeField]
        private string _informationCategory;
        [SerializeField]
        private string _id;
        [SerializeField]
        private DateTime _lastTimeReceived;
        private Dictionary<string, string> _parameters;


        #region dictionary serialization
        /// <summary>
        /// <see cref="Parameters"/> keys. Used for serialization only.
        /// </summary>
        [SerializeField]
        private List<string> _parametersKeys;
        /// <summary>
        /// <see cref="Parameters"/> values. Used for serialization only.
        /// </summary>
        [SerializeField]
        private List<string> _parametersValues;

        /// <summary>
        /// Converts the <see cref="Parameters"/> dictionary in two lists (<see cref="_parametersKeys"/> and <see cref="_parametersValues"/>) for serialization.
        /// </summary>
        public void OnBeforeSerialize()
        {
            _parametersKeys = Parameters.Keys.ToList();
            _parametersValues = Parameters.Values.ToList();
        }

        /// <summary>
        /// Converts the two deserialized lists (<see cref="_parametersKeys"/> and <see cref="_parametersValues"/>) and populates the <see cref="Parameters"/> with them.
        /// </summary>
        public void OnAfterDeserialize()
        {
            Parameters = new Dictionary<string, string>();

            for (var i = 0; i < _parametersValues.Count; i++)
            {
                var key = _parametersKeys[i];
                var value = _parametersValues[i];

                Parameters.Add(key, value);
            }
        }

        #endregion

        /// <summary>
        /// Values received
        /// </summary>
        public List<string> Values
        {
            get { return _values; }
            set { _values = value; }
        }

        /// <summary>
        /// The type of information received. Can be rotation, position, etc
        /// </summary>
        public string InformationType
        {
            get { return _informationType; }
            set { _informationType = value; }
        } // rotation

        /// <summary>
        /// The label that identifies this information. Can be joint names, signal names, etc
        /// </summary>
        public string InformationLabel
        {
            get { return _informationLabel; }
            set { _informationLabel = value; }
        } // waist

        /// <summary>
        /// The category that identifies the information. Can be tracking, button, digital, etc
        /// </summary>
        public string InformationCategory
        {
            get { return _informationCategory; }
            set { _informationCategory = value; }
        } // tracking

        /// <summary>
        /// The Id of the device that these values belong to.
        /// </summary>
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Represents the last time the information was updated
        /// </summary>
        public DateTime LastTimeReceived
        {
            get { return _lastTimeReceived; }
            set { _lastTimeReceived = value; }
        }

        /// <summary>
        /// The parameters that identifies the information. Can be side=left for example.
        /// </summary>
        public Dictionary<string, string> Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }

        /// <summary>
        /// Returns a string combination of the information: Id_Label_Type
        /// </summary>
        public string Key
        {
            get { return Id + "_" + InformationLabel + "_" + InformationType; }
        }
        
        public StringValues(List<string> values, string informationType, string informationLabel,
            string informationCategory, string id, DateTime lastTimeReceived, Dictionary<string, string> parameters)
        {
            Values = values;
            InformationType = informationType;
            InformationLabel = informationLabel;
            InformationCategory = informationCategory;
            Id = id;
            LastTimeReceived = lastTimeReceived;
            Parameters = parameters;
        }
    }
}