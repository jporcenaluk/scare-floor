using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace ScareFloor.Device
{
    public class LightControl
    {
        public enum LightMeterStatus { Filled, PartiallyFilled, Empty }
        private GpioController Gpio { get; set; }
        private IList<LightModel> Lights { get; set; }

        private IList<int> AvailablePinList { get; set; } = new List<int>()
        {
            4,5,6,12,13
        };

        public LightControl(GpioController gpio)
        {
            Gpio = gpio;
            InitializeLights();
        }

        private void InitializeLights()
        {
            Lights = new List<LightModel>();
            foreach (var pin in AvailablePinList)
            {
                Lights.Add(new LightModel(Gpio, pin));
            }
            Lights.OrderBy(s => s.pin.PinNumber);
        }

        public void TurnLightOn(int ledPin)
        {
            var light = Lights.Where(s => s.pin.PinNumber == ledPin).SingleOrDefault();
            if (light != null)
            {
                light.SetPinValue(GpioPinValue.High);
            }
        }

        public void TurnLightOff(int ledPin)
        {
            var light = Lights.Where(s => s.pin.PinNumber == ledPin).SingleOrDefault();
            if (light != null)
            {
                light.SetPinValue(GpioPinValue.Low);
            }
        }

        public void ToggleLight(int ledPin)
        {
            var light = Lights.Where(s => s.pin.PinNumber == ledPin).SingleOrDefault();
            if (light != null)
            {
                light.TogglePinValue();
            }
        }

        public void TurnAllLightsOn()
        {
            foreach (var light in Lights)
            {
                light.SetPinValue(GpioPinValue.High);
            }
        }

        public void TurnAllLightsOff()
        {
            foreach (var light in Lights)
            {
                light.SetPinValue(GpioPinValue.Low);
            }
        }

        public LightMeterStatus GetLightMeterStatus()
        {
            if (Lights.All(s => s.pinValue == GpioPinValue.High))
            {
                return LightMeterStatus.Filled;
            }
            else if (Lights.All(s => s.pinValue == GpioPinValue.Low))
            {
                return LightMeterStatus.Empty;
            }
            else
            {
                return LightMeterStatus.PartiallyFilled;
            }
        }

        /// <summary>
        /// Sets the next-highest light value to on.
        /// </summary>
        public void IncrementLightOn()
        {
            foreach (var light in Lights)
            {
                if (light.pinValue == GpioPinValue.Low)
                {
                    light.SetPinValue(GpioPinValue.High);
                    return;
                }
            }
        }
    }
}
