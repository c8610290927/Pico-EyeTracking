using System;
using System.IO;
using System.Linq;
//using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using InTheHand.Devices.Bluetooth;
using InTheHand.Devices.Bluetooth.Rfcomm;
using InTheHand.Devices.Enumeration;
using LabData;
using System.Threading;

namespace LabDevice
{
    public enum BluetoothGetType
    {
        Pick,
        Read
    }
    public class BlueToothController:DeviceBase
    {

        private BluetoothDevice _bluetoothDevice;
        private BlueToothDeviceConfig _config;
        private Stream _stream;
        Thread _readThread;
        bool _keepReading;
        //第一个代表信号质量(0 to 200)，第二个代表心率（0 to 255）
        public delegate void GetHeartDataDelegate(int[] heart); //定义一个委托
        public GetHeartDataDelegate GetHeartHandler;
        //原始数据（-32768 to 32767）
        public delegate void GetRawDataDelegate(int raw); //定义一个委托
        public GetRawDataDelegate GetRawHandler;
        public int count = 0;


        public void Init()
        {

            _config = LabTools.GetConfig<BlueToothDeviceConfig>();
            DeviceType = DeviceType.BlueTooth;
          
            switch (_config.BluetoothGetType)
            {
                case BluetoothGetType.Pick:
                    var deviceInformation = PickDevice();
                    if (deviceInformation == null) { throw new InvalidDataException("Fail to retrieve device information - is the device turned on? (if so, try pairing it in Windows and try again)"); }
                    _bluetoothDevice = BluetoothDevice.FromDeviceInformation(deviceInformation);

                    break;
                case BluetoothGetType.Read:
                    _bluetoothDevice = BluetoothDevice.FromBluetoothAddress(ulong.Parse(_config.BluetoothAddress, System.Globalization.NumberStyles.HexNumber));

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        public bool StartConnect()
        {
            if (_bluetoothDevice.ConnectionStatus == BluetoothConnectionStatus.Disconnected)
            {
                Debug.Log("Bluetooth Connected");
                var result = _bluetoothDevice.GetRfcommServices(BluetoothCacheMode.Cached);
                var services = result.Services;


                /* find requested service and open connection*/
                foreach (var current in services)
                {
                    if (current.ServiceId == RfcommServiceId.SerialPort)
                    {
                        _stream = current.OpenStream();
                    }
                }

                return true;
            }

            return false;
        }


        public void GetData()
        {
            _keepReading = true;
            _readThread = new Thread(GetBluetoothData);
            _readThread.Start();
        }


        public void GetBluetoothData()
        {
            while (_keepReading)
            {
                if (_stream != null && _bluetoothDevice.ConnectionStatus == BluetoothConnectionStatus.Connected)
                {
                   // _rawDataBuilder.Clear();
                    var buffer = new byte[1024];
                    int read = _stream.Read(buffer, 0, buffer.Length);
                    if (read != 0)
                    {
                        for (int i = 0; i < read; i++)
                        {
                            if(buffer[i] == 0xaa && i+5<read)
                            {
                               
                                if (buffer[i+1] == 0x04 && buffer[i + 2] == 0x80)
                                {
                                    int raw = buffer[i + 4] * 256 + buffer[i + 5];
                                    if (raw >= 32768) raw= raw - 65536;
                                    count++;
                                    GetRawHandler?.Invoke(raw);
                                    i += 6;
                                }
                                else if(buffer[i + 1] == 0x12 && buffer[i + 2] == 0x02 && buffer[i + 4] == 0x03)
                                {
                                    int[] heart = new int[2];
                                    heart[0] = buffer[i + 3];
                                    heart[1] = buffer[i + 5];
                                    GetHeartHandler?.Invoke(heart);
                                    i += 6;
                                }
                            }
                            
                        }

                            /*for (int i = 1; i < read; i++)
                            {
                                if( i+2 <read && buffer[i] == 0xaa && buffer[i+1]==0x04 && buffer[i + 2] == 0x80)
                                {
                                    if(i+ 6< read && buffer[i+2]==0x80)
                                    {
                                        int[] raw = new int[2];
                                        raw[0] = buffer[i + 3] * 256 + buffer[i+4];
                                        if (raw[0] >= 32768) raw[0] = raw[0] - 65536;
                                         raw[1] = buffer[i + 5] * 256 + buffer[i + 6];
                                        if (raw[1] >= 32768) raw[1] = raw[1] - 65536;
                                        count += 2;
                                        if (GetHeartHandler != null)
                                        {
                                            GetHeartHandler(raw);
                                        }
                                        //Debug.Log("raw data: " + raw[0] + "****"+ raw[1]);
                                    }
                                    i = i + 7;


                                }

                                _rawDataBuilder.AppendFormat("0x{0:X2} ", buffer[i]);
                            }
                            Debug.Log("Received: " + _rawDataBuilder);*/
                        }

                   // BluetoothData = _rawDataBuilder.ToString();
                    //todo 处理读取的数据
                }
            }
        }

        public void StopConnect()
        {
            _stream?.Close();
            _keepReading = false;
            _readThread = null;
            GetHeartHandler = null;
            GetRawHandler = null;
        }


        private DeviceInformation PickDevice()
        {
            var picker = new DevicePicker();
            var deviceInfo = picker.PickSingleDevice();
            Debug.Log(deviceInfo.Id);
            return deviceInfo;
        }
    }
}
