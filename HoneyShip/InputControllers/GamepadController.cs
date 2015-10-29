using Microsoft.Xna.Framework;
using SlimDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HoneyShip
{
    class GamepadController : InputController
    {
        Vector2 shipDelta;
        Joystick joystick = null;
        JoystickState js;
        public GamepadController()
        {
            InitializeGamepad();
        }

        private void InitializeGamepad()
        {
            // make sure that DirectInput has been initialized
            DirectInput dinput = new DirectInput();

            // search for devices
            foreach (DeviceInstance device in dinput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly))
            {
                // create the device
                try
                {
                    joystick = new SlimDX.DirectInput.Joystick(dinput, device.InstanceGuid);
                    break;
                }
                catch (DirectInputException)
                {
                }
            }

            if (joystick == null)
            {
                MessageBox.Show("There are no joysticks attached to the system.");
                return;
            }

            foreach (DeviceObjectInstance deviceObject in joystick.GetObjects())
            {
                if ((deviceObject.ObjectType & ObjectDeviceType.Axis) != 0)
                    joystick.GetObjectPropertiesById((int)deviceObject.ObjectType).SetRange(-1000, 1000);

                //UpdateControl(deviceObject);
            }

            // acquire the device
            joystick.Acquire();
        }

        public bool Quit
        {
            get
            {
                return false;
            }
        }


        public Vector2 ShipMovement(float rotationAngle)
        {
            shipDelta = Vector2.Zero;
            if (js.X == 1000)
            {
                shipDelta += new Vector2((float)Math.Cos((rotationAngle) + MathHelper.PiOver2), (float)Math.Sin((rotationAngle + MathHelper.PiOver2)));
            }
            if (js.X == -1000)
            {
                shipDelta += new Vector2((float)Math.Cos((rotationAngle) - MathHelper.PiOver2), (float)Math.Sin((rotationAngle - MathHelper.PiOver2)));
            }
            if (js.Y == -1000)
            {
                shipDelta += new Vector2((float)Math.Cos((rotationAngle)), (float)Math.Sin((rotationAngle)));
            }
            if (js.Y == 1000)
            {
                shipDelta += new Vector2(-(float)Math.Cos((rotationAngle)), -(float)Math.Sin((rotationAngle)));
            }
            if (js.IsPressed(7))
            {
                shipDelta = shipDelta * 2f;
            }
            return shipDelta;
        }

        public bool isShooting
        {
            get
            {
                return js.IsPressed(4);
            }
        }

        public void Update(float dt)
        {
            js = joystick.GetCurrentState();
        }
    }
}
