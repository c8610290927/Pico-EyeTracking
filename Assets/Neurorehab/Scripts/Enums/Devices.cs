using Neurorehab.Scripts.Devices.Abstracts;
using Neurorehab.Scripts.Udp;

namespace Neurorehab.Scripts.Enums
{
    /// <summary>
    /// An enum containing all currently known devices used in the Rehablab Control Panel.
    /// </summary>
    public enum Devices
    {
        neurosky,
        leapmotion,
        tobiieyex,
        kinectdetected,
        kinect,
        bioplux,
        oculus,
        emotiv,
        bitalino,
        zephyr
    }

    /// <summary>
    /// Represets all the Tracking Types the UDP protocol can send (The tracking type is the [$] identifier). <para>This identifier is in the following format: [$]&lt;trackingType&gt;|&lt;parameter1&gt;=&lt;value&gt;|&lt;parameter1&gt;=&lt;value&gt;...,[$$]...</para><para>When parsing the UDP string, to get all the parameters of the tracking type, the tracking type itself must be ignored. This enum contains all the tracking types used in the UDP protocol. Consult <see cref="UdpGenericTranslator.TranslateData"/></para> for an example of usage.
    /// </summary>
    public enum TrackingType
    {
        tracking,
        button,
        eeg,
        analog,
        digital
    }

    /// <summary>
    /// Currently known Tracking Types parameters used in the Rehablab Controll Panel.
    /// </summary>
    public enum TrackingTypeParameter
    {
        /// <summary>
        /// The id of the device this data belongs to
        /// </summary>
        id,
        /// <summary>
        /// The side this object represents. For example, in Leapmotion it is used to distinguish between left and right hand.
        /// </summary>
        side,
        /// <summary>
        /// Signals that this element is the main element received from that device. In Kinect, this represents the closest Skeleton to it.
        /// </summary>
        main
    }

    /// <summary>
    /// Contains all known values for tracking type parameters in the Rehablab Controll Panel.
    /// </summary>
    public enum TrackingTypeParameterValue
    {
        /// <summary>
        /// Used for the side parameter.
        /// </summary>
        left,
        /// <summary>
        /// Used for the side parameter.
        /// </summary>
        right
    }

    /// <summary>
    /// All the labels received from TobiiEyex device.
    /// </summary>
    public enum TobiiEyeX
    {
        gazedisplay,
        gazegui,
        gazescreen,
        gazeviewport
    }

    /// <summary>
    /// All the labels received from Kinect device. Used for both Kinect and Kinect2
    /// </summary>
    public enum KinectBones
    {
        waist,
        torso,
        neck,
        head,
        leftshoulder,
        leftelbow,
        leftwrist,
        rightshoulder,
        rightelbow,
        rightwrist,
        lefthip,
        leftknee,
        leftankle,
        righthip,
        rightknee,
        rightankle,
        body
    }


    /// <summary>
    /// All the labels received from Kinect2 face api
    /// </summary>
    public enum KinectFace
    {
        happy,
        engaged,
        wearingglasses,
        lefteyeclosed,
        righteyeclosed,
        mouthopen,
        mouthmoved,
        lookingaway
    }

    /// <summary>
    /// All the labels received from Kinect gesture api
    /// </summary>
    public enum KinectGestures
    {
        leftwave,
        rightwave
    }

    /// <summary>
    /// Information type Categories received from all devices in controll panel.
    /// </summary>
    public enum InformationType
    {
        unknown,
        /// <summary>
        /// Represents a rotation. Currently, the <see cref="GenericDeviceData"/> recognizes rotations with 3 coordinates (Euler angles) and 4 coordinates (quaternions)
        /// </summary>
        rotation,
        /// <summary>
        /// Represents a position. Can be used for both 2D and 3D positions.
        /// </summary>
        position,
        /// <summary>
        /// Represents a bool. Can be any of the recognized values in the default bool parser.
        /// <para> The following values strings are recognized as true: "true", "T", "YES", "TRUE" and "1".</para>
        /// </summary>
        @bool,
        /// <summary>
        /// Represents a numerical value. Can be an integer or a floating point number, positive or negative.
        /// </summary>
        value,
        /// <summary>
        /// Represents a numerical value. Can be an integer or a floating point number, positive or negative.
        /// </summary>
 
        /// <summary>
        /// Represents a sequence of values that must the read in the received order. Each value can be an integer or a floating point positive or negative number. Example of a sample: 
        /// <para>"1,2,-3,4.987,6"</para>
        /// </summary>
        sample,
        //status
    }

    /// <summary>
    /// Represents all the labels received from the Bioplux device.
    /// </summary>
    public enum Bioplux
    {
        d0,
        d1,
        d2,
        d3,
        d4,
        d5,
        d6,
        d7
    }
    
    /// <summary>
    /// Represents all the labels received from the Bitalino device.
    /// </summary>
    public enum Bitalino
    {
        d0,
        d1,
        d2,
        d3,
        battery,
        a0,
        a1,
        a3,
        a4
    }

    /// <summary>
    /// Represents all the labels received from the Emotiv device.
    /// </summary>
    public enum Emotiv
    {
        counter,
        af4,
        f3,
        f4,
        f7,
        f8,
        fc5,
        fc6,
        t7,
        t8,
        p7,
        p8,
        o1,
        o2,
        gyro,
        longtermexcitement,
        shorttermexcitement,
        meditation,
        frustration,
        boredom,
        eyebrowextent,
        smileextent,
        upperfacepower,
        lowerfacepower,
        blink,
        lookingup,
        lookingdown,
        lookingleft,
        lookingright,
        eyelocationx,
        eyelocationy
    }

    /// <summary>
    /// Represents all the labels received from the Leapmotion device.
    /// </summary>
    public enum LeapMotionBones
    {
        thumb_bone1,
        thumb_bone2,
        thumb_bone3,
        index_bone1,
        index_bone2,
        index_bone3,
        middle_bone1,
        middle_bone2,
        middle_bone3,
        ring_bone1,
        ring_bone2,
        ring_bone3,
        pinky_bone1,
        pinky_bone2,
        pinky_bone3,
        forearm,
        palm
    }

    /// <summary>
    /// Represents all the labels received from the Bioplux gesture api.
    /// </summary>
    public enum LeapMotionGestures
    {
        grab
    }

    /// <summary>
    /// Represents all the labels received from the Neurosky device.
    /// </summary>
    public enum Neurosky
    {
        lowgamma,
        meditation,
        theta,
        highalpha,
        poorsignal,
        lowalpha,
        attention,
        highbeta,
        lowbeta,
        highgamma,
        delta
    }

    /// <summary>
    /// Represents all the labels received from the Oculus Rift device.
    /// </summary>
    public enum OculusRift
    {
        gyro
    }

    /// <summary>
    /// Represents all the labels received from the Zephyr device.
    /// </summary>
    public enum Zephyr
    {
        rtorperiod,
        breathing_waveform,
        ecg_waveform,
        hr,
        br,
        hrconf,
        brconf,
        hrv,
        skintemp,
        posture,
        gsr,
        systconf
    }
}