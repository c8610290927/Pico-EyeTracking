using System;
using Neurorehab.Scripts.Devices.Abstracts;
using Neurorehab.Scripts.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Neurorehab.Scripts.GUI
{
    /// <summary>
    /// Responsible for managing the PositionMultiplier and SmoothingSettings sliders in the GUI
    /// </summary>
    public class SliderManager : MonoBehaviour
    {
        /// <summary>
        /// A Reference to the <see cref="IPositionMultiplier"/> component in the gameobject
        /// </summary>
        private IPositionMultiplier _posSettings;
        /// <summary>
        /// A Reference to the <see cref="ISmoothSettings"/> component in the gameobject
        /// </summary>
        private ISmoothSettings _smoothSettings;

        [Header("Position GUI Gameobjects")]
        public Slider PosSlider;
        public Text PosSliderValue;

        [Header("Smoothing GUI Gameobjects")]
        public Slider SmoothSlider;
        public Text SmoothSliderValue;


        private void Awake()
        {
            _smoothSettings = GameObject.FindGameObjectWithTag(Tags.Component.ToString())
                .GetComponent<ISmoothSettings>();

            _posSettings = GameObject.FindGameObjectWithTag(Tags.Component.ToString())
                .GetComponent<IPositionMultiplier>();
        }

        /// <summary>
        /// INitializes both sliders in the GUI according to the <see cref="_smoothSettings"/> values and <see cref="_posSettings"/> values
        /// </summary>
        private void Start()
        {
            if (SmoothSlider != null)
            {
                SmoothSlider.maxValue = 30;
                SmoothSlider.minValue = 1;
                SmoothSlider.wholeNumbers = true;

                SmoothSlider.value = _smoothSettings.NumberOfSamples;
                SmoothSliderValue.text = _smoothSettings.NumberOfSamples.ToString();
            }

            if (PosSlider != null)
            {
                PosSlider.maxValue = 50;
                PosSlider.minValue = 1;
                PosSlider.wholeNumbers = true;

                PosSlider.value = _posSettings.PositionMultiplier;
                PosSliderValue.text = _posSettings.PositionMultiplier.ToString();
            }
        }

        /// <summary>
        /// Updates the values in the <see cref="_smoothSettings"/> according to the <see cref="SmoothSlider"/> changes
        /// </summary>
        public void UpdateSmoothValues()
        {
            if (Time.frameCount <= 1) return;
            if (_smoothSettings == null) return;
            _smoothSettings.NumberOfSamples = Convert.ToInt32(SmoothSlider.value);
            SmoothSliderValue.text = SmoothSlider.value.ToString();
        }

        /// <summary>
        /// Updates the values in the <see cref="_posSettings"/> according to the <see cref="PosSlider"/> changes
        /// </summary>
        public void UpdatePositionValues()
        {
            if (Time.frameCount <= 1) return;
            if (_posSettings == null) return;
            _posSettings.PositionMultiplier = Convert.ToInt32(PosSlider.value);
            PosSliderValue.text = PosSlider.value.ToString();
        }
    }
}