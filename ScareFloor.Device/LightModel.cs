using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace ScareFloor.Device
{
    public class LightModel
    {
        public LightModel(GpioController gpio, int pinNumber)
        {
            pin = gpio.OpenPin(pinNumber);
            pinValue = GpioPinValue.Low;
            pin.Write(pinValue);
            pin.SetDriveMode(GpioPinDriveMode.Output);
        }

        public void SetPinValue(GpioPinValue value)
        {
            pin.Write(value);
            pinValue = value;
        }

        public void TogglePinValue()
        {
            if (pinValue == GpioPinValue.High)
            {
                pinValue = GpioPinValue.Low;
            }
            else if (pinValue == GpioPinValue.Low)
            {
                pinValue = GpioPinValue.High;
            }
            pin.Write(pinValue);
        }

        public GpioPin pin { get; set; }
        public GpioPinValue pinValue { get; set; }
    }
}
