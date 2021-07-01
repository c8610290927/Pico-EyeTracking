using Neurorehab.Scripts.Devices.Abstracts;
using Neurorehab.Scripts.Devices.Data;
using Neurorehab.Scripts.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Neurorehab.Device_TobiiEyeX.Scripts
{
    /// <summary>
    /// Responsible for updating the Unity Tobii data according to its <see cref="TobiiEyeXData"/>.
    /// </summary>
    public class TobiiEyeXUnity : GenericDeviceUnity
    {
        /// <summary>
        /// The Scene Canvas height resolution -> canvasScaler.referenceResolution.y;
        /// </summary>
        private float _canvasScaleHeight;

        /// <summary>
        /// The Scene Canvas width resolution -> canvasScaler.referenceResolution.x;
        /// </summary>
        private float _canvasScaleWidth;

        /// <summary>
        /// Current screen height resolution
        /// </summary>
        private float _height;

        /// <summary>
        /// Current screen width resolution
        /// </summary>
        private float _width;

        [Header("Settings")]
        public Text GazeDisplay;
        public Text GazeGui;
        public Text GazeScreen;
        public Text GazeViewport;
        [Header("Eye tracker settings")]
        public GameObject tracker;
        public bool LookToGazeDisplay = true;
        public bool LookToGazeGUI;
        public bool LookToGazeScreen;
        public bool LookToGazeViewport;


        /// <summary>
        /// Initializes <see cref="_width"/>, <see cref="_height"/>, <see cref="_canvasScaleHeight"/> and <see cref="_canvasScaleWidth"/>
        /// </summary>
        private void Awake()
        {
            var canvasScaler = GameObject.FindGameObjectWithTag(Tags.Canvas.ToString()).GetComponent<CanvasScaler>();
            _width = Screen.currentResolution.width;
            _height = Screen.currentResolution.height;

            _canvasScaleWidth = canvasScaler.referenceResolution.x;
            _canvasScaleHeight = canvasScaler.referenceResolution.y;
        }

        /// <summary>
        /// Updates the gameobject option according to its <see cref="GenericDeviceUnity.GenericDeviceData"/> 
        /// </summary>
        private void Update()
        {
            UpdateGuiValues();
            UpdateTrackValues();
        }

        private void UpdateGuiValues()
        {
            var gazedisplay = GenericDeviceData.GetPosition(Neurorehab.Scripts.Enums.TobiiEyeX.gazedisplay.ToString());
            GazeDisplay.text = "GAZEDISPLAY - X: " + gazedisplay.x + "; Y: " + gazedisplay.y;

            var gazegui = GenericDeviceData.GetPosition(Neurorehab.Scripts.Enums.TobiiEyeX.gazegui.ToString());
            GazeGui.text = "GAZEGUI - X: " + gazegui.x + "; Y: " + gazegui.y;

            var gazeviewport = GenericDeviceData.GetPosition(Neurorehab.Scripts.Enums.TobiiEyeX.gazeviewport.ToString());
            GazeViewport.text = "GAZEVIEWPORT - X: " + gazeviewport.x + "; Y: " + gazeviewport.y;

            var gazescreen = GenericDeviceData.GetPosition(Neurorehab.Scripts.Enums.TobiiEyeX.gazescreen.ToString());
            GazeScreen.text = "GAZESCREEN - X: " + gazescreen.x + "; Y: " + gazescreen.y;
        }

        private void UpdateTrackValues()
        {
            var trackerPosition = Vector2.zero;

            if (LookToGazeDisplay)
                trackerPosition = GenericDeviceData.GetPosition(Neurorehab.Scripts.Enums.TobiiEyeX.gazedisplay.ToString());
            else if (LookToGazeGUI)
                trackerPosition = GenericDeviceData.GetPosition(Neurorehab.Scripts.Enums.TobiiEyeX.gazegui.ToString());
            else if (LookToGazeScreen)
                trackerPosition = GenericDeviceData.GetPosition(Neurorehab.Scripts.Enums.TobiiEyeX.gazescreen.ToString());
            else if (LookToGazeViewport)
                trackerPosition = GenericDeviceData.GetPosition(Neurorehab.Scripts.Enums.TobiiEyeX.gazeviewport.ToString());

            var pos = new Vector3(
                trackerPosition.x / _width * _canvasScaleWidth - _canvasScaleWidth / 2,
                (trackerPosition.y / _height * _canvasScaleHeight - _canvasScaleHeight / 2) * -1, 0);

            tracker.transform.localPosition = pos;
        }
    }
}