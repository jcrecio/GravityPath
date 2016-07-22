namespace GravityPath.Services
{
    using System.Linq;
    using Enumeration;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input.Touch;

    public class InputManager
    {
        private static InputManager _inputManager;

        private static InputType _inputType;
        public static InputType InputType
        {
            get { return _inputType; }
        }

        public static InputManager GetInstance(InputType inputType)
        {
            var manager = _inputManager ?? (_inputManager = new InputManager(inputType));
            
            var input = InputManager.InputType;
            if (!input.Equals(inputType))
            {
                throw new Exceptions.InputException("Input Manager is already assigned to a specific input type.");
            }
            return manager;
        }

        private InputManager(InputType inputType)
        {
            _inputType = inputType;
        }

        public int GetUserTouch()
        {
            var touchCollection = TouchPanel.GetState();

            var numberOfTouches = touchCollection.Count;

            switch (numberOfTouches)
            {
                case 1:
                {
                    return this.InputFromOneTouch(touchCollection[0]);
                }
                case 2:
                {
                    var position1 = touchCollection[0].Position;
                    var position2 = touchCollection[1].Position;

                    if (this.LocationsAtSameHalf(position1, position2))
                    {
                        return this.InputFromOneTouch(touchCollection[0]);
                    }
                    else
                    {
                        return (int) InputTouch.NeutralMovement;
                    }
                }
            }

            return 0;
        }

        private int InputFromOneTouch(TouchLocation touchLocation)
        {
            var position = touchLocation.Position;
            if (position.Equals(Vector2.Zero)) return -1;

            return position.X <= 240 ? 0 : 1;
        }

        private bool LocationsAtSameHalf(Vector2 touchLocation1, Vector2 touchLocation2)
        {
            return touchLocation1.X <= 240 && touchLocation2.X <= 240 ||
                   touchLocation1.X >= 241 && touchLocation2.X >= 241;
        }

        public int  GetTouchOption()
        {
            var position = TouchPanel.GetState().FirstOrDefault().Position;
            if (position.Equals(Vector2.Zero)) return -1;
            return 1;
        }
    }
}