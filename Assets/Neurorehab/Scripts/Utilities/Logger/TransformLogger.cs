using UnityEngine;

namespace Neurorehab.Scripts.Utilities.Logger
{
    /// <summary>
    /// Must be used in a scene containing the Logger. Creates two logging files, the POSITIONS and the ROTATIONS. The POSITIONS file holds all the vector3 for the positions. The ROTATIONS contains all the quaternions for the ROTATIONS. Uses the fixed update in order to keep a steady framerate for the logging.
    /// </summary>
    public class TransformLogger : MonoBehaviour
    {
        private Transform _myTransform;

        private void Awake()
        {
            _myTransform = transform;
        }

        private void FixedUpdate()
        {
            if (Logger.Instantiated == false) return;

            Logger.Instance.WriteLine(_myTransform.position.ToString(), "POSITIONS");
            Logger.Instance.WriteLine(_myTransform.rotation.ToString(), "ROTATIONS");
        }
    }
}