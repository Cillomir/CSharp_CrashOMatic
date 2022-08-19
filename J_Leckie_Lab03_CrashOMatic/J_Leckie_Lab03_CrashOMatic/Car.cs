/***************************************************
* Lab 03: Car Class
* 
* This portion of the lab is to create an abstract
*   class that can then be used to create several
*   variances of a car object. It also has a tank
*   and a separate class for the tank shells.
* 
* Author: Joel Leckie
* CMPE 2300 – OE01: Spring 2022
**************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using System.Drawing;
using GDIDrawer;

namespace J_Leckie_Lab03_CrashOMatic
{
    // create a new interface with a single method that returns nothing and accepts nothing
    internal interface IAnimateable
    {
        void Animate();
    }

    #region Base Car
    // create an abstract base class
    public abstract class Car
    {
        // add a static PicDrawer reference with added manual property
        private static CDrawer Canvas;
        internal static CDrawer canvas
        {
            // normal get
            get => Canvas;
            // set closes an existing PicDrawer and replaces it
            set
            {
                if (Canvas != null) Canvas.Close();
                Canvas = new Background(Properties.Resources.Crash);
            }
        }

        // add a public static Random as an automatic property
        public static Random randVal { get; set; }

        // add 4 static readonly collections to hold lane X and Y values
        internal static readonly List<int> Up = new List<int>() { 270, 590 };
        internal static readonly List<int> Down = new List<int>() { 170, 490 };
        internal static readonly List<int> Left = new List<int>() { 164 };
        internal static readonly List<int> Right = new List<int>() { 260 };

        // add protected integer members
        protected int X, Y, Width, Height, Speed;

        // add a protected bool
        protected bool HalfSpeed = false;

        // add a count for crash timer
        private int Crashed;
        internal int crashed { get => Crashed; set => Crashed = value; }

        // add a value for the vehicles value in points
        private static int TotalScore;
        internal static int score { get => TotalScore; set => TotalScore = value; }

        // initialize some generic game sounds 
        private SoundPlayer newCar = new SoundPlayer(Properties.Resources.engineStart);
        private SoundPlayer crash = new SoundPlayer(Properties.Resources.crash1);

        // add a static constructor to initialize the static properties (?)
        static Car()
        {
            canvas = null;
            randVal = new Random();
            TotalScore = 0;
        }

        // add a constructor accepting speed, width, and height of a car
        public Car(int speed, int width, int height)
        {
            Speed = speed;
            Height = height;
            Width = width;
            // add additional crash count value
            Crashed = 0;
            // play a new car sfx
            newCar.Play();
        }

        // add a public abstract method to return a rectangle type (will be fulfilled by derived classes)
        public abstract Rectangle GetRect();
        //return new Rectangle(X, Y, Width, Height);

        // using the NVI pattern, create public and protected abstract set of methods to show the car
        public void ShowCar() => VShowCar();
        protected abstract void VShowCar();

        // using the NVI pattern, create public and protected abstract set of methods to move the car
        public void Move() => VMove();
        protected abstract void VMove();

        // create scoring and penalty methods which can be overridden for each vehicle type
        public void GetSafeScore() => VSafeScore();
        protected abstract void VSafeScore();
        public void GetHitScore() => VHitScore();
        protected abstract void VHitScore();

        // override Equals() to return true if the rectangles of the two cars are overlapping
        public override bool Equals(object obj)
        {
            if (!(obj is Car other)) return false;
            return this.GetRect().IntersectsWith(other.GetRect());
        }

        // override GetHashCode() to return 0
        public override int GetHashCode() => 0;

        // add a method that accepts a point and return true if the point is in the cars rectangle
        public bool PointOnCar(Point click) => this.GetRect().Contains(click);

        // add a static helper predicate to return true if the car is out of the canvas bounds
        public static bool OutOfBounds(Car car)
        {
            Rectangle boundary = new Rectangle(-20, -20, Canvas.ScaledWidth + 20, Canvas.ScaledHeight + 20);
            return !boundary.IntersectsWith(car.GetRect());
        }

        // add a method to toggle between half speed and full speed
        public void ToggleSpeed() => HalfSpeed = !HalfSpeed;

        // add a method for when a car has collided with something
        public void Crash()
        {
            // if it is the first frame of the crash, play a crash sound
            if (crashed == 0) crash.Play();
            crashed++;
        }

        // add a method to animate a star over the crashed vehicle
        public void ShowCrash()
        {
            crashed++;
            canvas.AddLine(X, Y, X + Width, Y + Height, Color.Red);
            canvas.AddLine(X, Y + Height, X + Width, Y, Color.Red);
            for (double i = 0; i < 60; i++) 
            {
                canvas.AddLine(new Point(X + (int)(Width * 0.5), Y + (int)(Height * 0.5)), crashed * 3, i * 4.0, Color.HotPink);
            }
        }
    } // end of abstract class Car()
    #endregion Base Car

    #region Horizontal Car
    // derive a new abstract class HorizontalCar
    public abstract class HorizontalCar : Car
    {
        // add a constructor with speed, width, and height parameters
        public HorizontalCar(int speed, int width, int height)
            : base(speed, width, height)
        {
            // negative speed starts off the right of the screen in a random left lane
            if (speed < 0) { X = canvas.ScaledWidth; Y = Left[randVal.Next(Left.Count)]; }
            // positive speed starts off the left of the screen in a random right lane
            else if (speed > 0) { X = 0 - width; Y = Right[randVal.Next(Right.Count)]; }
        }

        // override VMove() to increment X by the speed
        protected override void VMove()
        {
            if (crashed < 2)
            {
                if (HalfSpeed) X += Speed / 2;
                else X += Speed;
            }
        }
    }
    #endregion Horizontal Car

    #region Vertical Car
    // derive a new abstract class VerticalCar
    public abstract class VerticalCar : Car
    {
        // add a constructor with speed, width, and height parameters
        public VerticalCar(int speed, int width, int height)
            : base(speed, width, height)
        {
            // negative speed starts off the bottom of the screen in a random up lane
            if (speed < 0) { Y = canvas.ScaledHeight; X = Up[randVal.Next(Up.Count)]; }
            // positive speed starts off the top of the screen in a random right lane
            else if (speed > 0) { Y = 0 - height; X = Down[randVal.Next(Down.Count)]; }
        }

        // override VMove() to increment X by the speed
        protected override void VMove()
        {
            if (crashed < 2)
            {
                if (HalfSpeed) Y += Speed / 2;
                else Y += Speed;
            }
        }
    }
    #endregion Vertical Car

    #region Horizontal Ambulance
    // derive a new concrete class HAmbulance
    public class HAmbulance : HorizontalCar, IAnimateable
    {
        // build this similar to the VSedan
        // add a protected color member initialized to a random color
        protected Color color = RandColor.GetColor();

        // add an extra member to animate flashing lights
        internal bool lightOn = false;

        // Add a constructor with speed, width (default 40), and height (default 70)
        public HAmbulance(int speed = 7, int width = 90, int height = 40)
            : base(speed, width, height)
        {
        }

        public override Rectangle GetRect() => new Rectangle(X, Y, Width, Height);

        protected override void VShowCar()
        {
            // animate the lights and body bounce
            Animate();

            // tires
            canvas.AddCenteredEllipse(X + (int)(Width * 0.1), Y, (int)(Width * 0.2), (int)(Height * 0.2), Color.Black);
            canvas.AddCenteredEllipse(X + (int)(Width * 0.9), Y, (int)(Width * 0.2), (int)(Height * 0.2), Color.Black);
            canvas.AddCenteredEllipse(X + (int)(Width * 0.1), Y + Height, (int)(Width * 0.2), (int)(Height * 0.2), Color.Black);
            canvas.AddCenteredEllipse(X + (int)(Width * 0.9), Y + Height, (int)(Width * 0.2), (int)(Height * 0.2), Color.Black);

            if (Speed > 0) // headlights, going right
            {
                canvas.AddCenteredEllipse(X + (int)(Width * 0.9), Y + (int)(Height * 0.2), (int)(Width * 0.3), (int)(Height * 0.4), Color.LightYellow, 1, Color.Black);
                canvas.AddCenteredEllipse(X + (int)(Width * 0.9), Y + (int)(Height * 0.8), (int)(Width * 0.3), (int)(Height * 0.4), Color.LightYellow, 1, Color.Black);
            }
            else if (Speed < 0) // headlights, going left
            {
                canvas.AddCenteredEllipse(X + (int)(Width * 0.1), Y + (int)(Height * 0.2), (int)(Width * 0.3), (int)(Height * 0.4), Color.LightYellow, 1, Color.Black);
                canvas.AddCenteredEllipse(X + (int)(Width * 0.1), Y + (int)(Height * 0.8), (int)(Width * 0.3), (int)(Height * 0.4), Color.LightYellow, 1, Color.Black);
            }

            // body
            canvas.AddRectangle(GetRect(), color, 1, Color.Black);

            if (Speed > 0) // windshield & lights, going right
            {
                canvas.AddRectangle(X + (int)(Width * 0.5), Y + (int)(Height * 0.1), (int)(Width * 0.3), (int)(Height * 0.8), Color.SlateGray, 1, Color.DarkSlateGray);
                canvas.AddCenteredRectangle(X + (int)(Width * 0.4), Y + (int)(Height * 0.25), (int)(Width * 0.1), (int)(Height * 0.4),
                    lightOn ? Color.Blue : Color.White, 1, Color.Black);
                canvas.AddCenteredRectangle(X + (int)(Width * 0.4), Y + (int)(Height * 0.75), (int)(Width * 0.1), (int)(Height * 0.4),
                    lightOn ? Color.Red : Color.White, 1, Color.Black);
            }
            else if (Speed < 0) // windshield & lights, going left
            {
                canvas.AddRectangle(X + (int)(Width * 0.1), Y + (int)(Height * 0.1), (int)(Width * 0.3), (int)(Height * 0.8), Color.Gray, 1, Color.DarkSlateGray);
                canvas.AddCenteredRectangle(X + (int)(Width * 0.6), Y + (int)(Height * 0.75), (int)(Width * 0.1), (int)(Height * 0.4),
                    lightOn ? Color.Red : Color.White, 1, Color.Black);
                canvas.AddCenteredRectangle(X + (int)(Width * 0.6), Y + (int)(Height * 0.25), (int)(Width * 0.1), (int)(Height * 0.4),
                    lightOn ? Color.Blue : Color.White, 1, Color.Black);
            }
            // crashed
            if (crashed > 0) ShowCrash();
        }

        public void Animate()
        {
            lightOn = !lightOn;
            if (lightOn) Y += 2;
            else Y -= 2;
        }

        // implemented scoring for safely crossing the intersection or penalty for a collision
        protected override void VSafeScore() => score += 225;
        protected override void VHitScore() => score -= 350;
    }
    #endregion Horizontal Ambulance

    #region Vertical Sedan
    // derive a new concrete class VSedan
    public class VSedan : VerticalCar
    {
        // add a protected color member initialized to a random color
        protected Color color = RandColor.GetColor();

        // Add a constructor with speed, width (default 40), and height (default 70)
        public VSedan (int speed, int width = 40, int height = 70)
            : base(speed, width, height)
        {
        }

        public override Rectangle GetRect() => new Rectangle(X, Y, Width, Height);

        protected override void VShowCar()
        {
            // tires
            canvas.AddCenteredEllipse(X, Y + (int)(Height * 0.1), (int)(Width * 0.2), (int)(Height * 0.2), Color.Black);
            canvas.AddCenteredEllipse(X + Width, Y + (int)(Height * 0.1), (int)(Width * 0.2), (int)(Height * 0.2), Color.Black);
            canvas.AddCenteredEllipse(X, Y + (int)(Height * 0.9), (int)(Width * 0.2), (int)(Height * 0.2), Color.Black);
            canvas.AddCenteredEllipse(X + Width, Y + (int)(Height * 0.9), (int)(Width * 0.2), (int)(Height * 0.2), Color.Black);
            
            if (Speed > 0) // headlights going down
            {
                canvas.AddCenteredEllipse(X + (int)(Width * 0.2), Y + (int)(Height * 0.9), (int)(Width * 0.4), (int)(Height * 0.3), Color.LightYellow, 1, Color.Black);
                canvas.AddCenteredEllipse(X + (int)(Width * 0.8), Y + (int)(Height * 0.9), (int)(Width * 0.4), (int)(Height * 0.3), Color.LightYellow, 1, Color.Black);
            }
            else if (Speed < 0) // headlights going up
            {
                canvas.AddCenteredEllipse(X + (int)(Width * 0.2), Y + (int)(Height * 0.1), (int)(Width * 0.4), (int)(Height * 0.3), Color.LightYellow, 1, Color.Black);
                canvas.AddCenteredEllipse(X + (int)(Width * 0.8), Y + (int)(Height * 0.1), (int)(Width * 0.4), (int)(Height * 0.3), Color.LightYellow, 1, Color.Black);
            }
            
            // body
            canvas.AddRectangle(GetRect(), color, 1, Color.Black);

            if (Speed > 0) // windshield going down
                canvas.AddRectangle(X + (int)(Width * 0.1), Y + (int)(Height * 0.5), (int)(Width * 0.8), (int)(Height * 0.3), Color.SlateGray, 1, Color.DarkSlateGray);
            else if (Speed < 0) // windshield going up
                canvas.AddRectangle(X + (int)(Width * 0.1), Y + (int)(Height * 0.1), (int)(Width * 0.8), (int)(Height * 0.3), Color.Gray, 1, Color.DarkSlateGray);

            // crashed
            if (crashed > 0) ShowCrash();
        }

        // implemented scoring for safely crossing the intersection or penalty for a collision
        protected override void VSafeScore() => score += 40;
        protected override void VHitScore() => score -= 60;

    }
    #endregion Vertical Sedan

    #region Horizontal Sedan
    // derive a new concrete class HSedan
    public class HSedan : HorizontalCar
    {
        // add a protected color member initialized to a random color
        protected Color color = RandColor.GetColor();

        // Add a constructor with speed, width (default 70), and height (default 40)
        public HSedan(int speed, int width = 70, int height = 40)
            : base(speed, width, height)
        {
        }

        public override Rectangle GetRect() => new Rectangle(X, Y, Width, Height);

        protected override void VShowCar()
        {
            // tires
            canvas.AddCenteredEllipse(X + (int)(Width * 0.1), Y, (int)(Width * 0.2), (int)(Height * 0.2), Color.Black);
            canvas.AddCenteredEllipse(X + (int)(Width * 0.1), Y + Height, (int)(Width * 0.2), (int)(Height * 0.2), Color.Black);
            canvas.AddCenteredEllipse(X + (int)(Width * 0.9), Y, (int)(Width * 0.2), (int)(Height * 0.2), Color.Black);
            canvas.AddCenteredEllipse(X + (int)(Width * 0.9), Y + Height, (int)(Width * 0.2), (int)(Height * 0.2), Color.Black);

            if (Speed > 0) // headlights going right
            {
                canvas.AddCenteredEllipse(X + (int)(Width * 0.9), Y + (int)(Height * 0.2), (int)(Width * 0.3), (int)(Height * 0.4), Color.LightYellow, 1, Color.Black);
                canvas.AddCenteredEllipse(X + (int)(Width * 0.9), Y + (int)(Height * 0.8), (int)(Width * 0.3), (int)(Height * 0.4), Color.LightYellow, 1, Color.Black);
            }
            else if (Speed < 0) // headlights going left
            {
                canvas.AddCenteredEllipse(X + (int)(Width * 0.1), Y + (int)(Height * 0.2), (int)(Width * 0.3), (int)(Height * 0.4), Color.LightYellow, 1, Color.Black);
                canvas.AddCenteredEllipse(X + (int)(Width * 0.1), Y + (int)(Height * 0.8), (int)(Width * 0.3), (int)(Height * 0.4), Color.LightYellow, 1, Color.Black);
            }

            // body
            canvas.AddRectangle(GetRect(), color, 1, Color.Black);

            if (Speed > 0) // windshield going right
                canvas.AddRectangle(X + (int)(Width * 0.6), Y + (int)(Height * 0.1), (int)(Width * 0.3), (int)(Height * 0.8), Color.SlateGray, 1, Color.DarkSlateGray);
            else if (Speed < 0) // windshield going left
                canvas.AddRectangle(X + (int)(Width * 0.1), Y + (int)(Height * 0.1), (int)(Width * 0.3), (int)(Height * 0.8), Color.Gray, 1, Color.DarkSlateGray);

            // crashed
            if (crashed > 0) ShowCrash();
        }

        // implemented scoring for safely crossing the intersection or penalty for a collision
        protected override void VSafeScore() => score += 40;
        protected override void VHitScore() => score -= 60;
    }

    #endregion Horizontal Sedan

    #region Vertical Truck
    // derive a new concrete class VTruck
    public class VTruck : VerticalCar, IAnimateable
    {
        // add a protected color member initialized to a random color
        protected Color color = RandColor.GetColor();

        // add a variable to hold the animation frame
        private int flashers = 0;

        // Add a constructor with speed, width (default 40), and height (default 80)
        public VTruck(int speed, int width = 40, int height = 80)
            : base(speed, width, height)
        {
        }

        public override Rectangle GetRect() => new Rectangle(X, Y, Width, Height);

        protected override void VShowCar()
        {
            // update the animation
            Animate();

            // tires
            canvas.AddCenteredEllipse(X, Y + (int)(Height * 0.1), (int)(Width * 0.2), (int)(Height * 0.2), Color.Black);
            canvas.AddCenteredEllipse(X + Width, Y + (int)(Height * 0.1), (int)(Width * 0.2), (int)(Height * 0.2), Color.Black);
            canvas.AddCenteredEllipse(X, Y + (int)(Height * 0.9), (int)(Width * 0.2), (int)(Height * 0.2), Color.Black);
            canvas.AddCenteredEllipse(X + Width, Y + (int)(Height * 0.9), (int)(Width * 0.2), (int)(Height * 0.2), Color.Black);

            if (Speed > 0) // headlights going down
            {
                canvas.AddCenteredEllipse(X + (int)(Width * 0.2), Y + (int)(Height * 0.9), (int)(Width * 0.4), (int)(Height * 0.3), Color.LightYellow, 1, Color.Black);
                canvas.AddCenteredEllipse(X + (int)(Width * 0.8), Y + (int)(Height * 0.9), (int)(Width * 0.4), (int)(Height * 0.3), Color.LightYellow, 1, Color.Black);
            }
            else if (Speed < 0) // headlights going up
            {
                canvas.AddCenteredEllipse(X + (int)(Width * 0.2), Y + (int)(Height * 0.1), (int)(Width * 0.4), (int)(Height * 0.3), Color.LightYellow, 1, Color.Black);
                canvas.AddCenteredEllipse(X + (int)(Width * 0.8), Y + (int)(Height * 0.1), (int)(Width * 0.4), (int)(Height * 0.3), Color.LightYellow, 1, Color.Black);
            }

            // body
            canvas.AddRectangle(GetRect(), color, 1, Color.Black);

            if (Speed > 0) // windshield and box, going down
            {
                canvas.AddRectangle(X + (int)(Width * 0.1), Y + (int)(Height * 0.6), (int)(Width * 0.8), (int)(Height * 0.3), Color.SlateGray, 1, Color.DarkSlateGray);
                canvas.AddRectangle(X + (int)(Width * 0.1), Y + (int)(Height * 0.1), (int)(Width * 0.8), (int)(Height * 0.4), this.color, 2, Color.Brown);
            }
            else if (Speed < 0) // windshield and box going up
            {
                canvas.AddRectangle(X + (int)(Width * 0.1), Y + (int)(Height * 0.1), (int)(Width * 0.8), (int)(Height * 0.3), Color.Gray, 1, Color.DarkSlateGray);
                canvas.AddRectangle(X + (int)(Width * 0.1), Y + (int)(Height * 0.5), (int)(Width * 0.8), (int)(Height * 0.4), this.color, 2, Color.Brown);
            }

            // lights on the truck box to animate
            if (Speed > 0) // lights going down
            {
                canvas.AddCenteredEllipse(X + (int)(Width * 0.2), Y + (int)(Height * 0.5), (int)(Width * 0.2), (int)(Height * 0.1), 
                    (flashers > 9 && flashers <= 12) ? Color.Gold : Color.LightYellow, 1, Color.DarkOrange);
                canvas.AddCenteredEllipse(X + (int)(Width * 0.4), Y + (int)(Height * 0.5), (int)(Width * 0.2), (int)(Height * 0.1), 
                    (flashers > 6 && flashers <= 9) ? Color.Gold : Color.LightYellow, 1, Color.DarkOrange);
                canvas.AddCenteredEllipse(X + (int)(Width * 0.6), Y + (int)(Height * 0.5), (int)(Width * 0.2), (int)(Height * 0.1), 
                    (flashers > 3 && flashers <= 6) ? Color.Gold : Color.LightYellow, 1, Color.DarkOrange);
                canvas.AddCenteredEllipse(X + (int)(Width * 0.8), Y + (int)(Height * 0.5), (int)(Width * 0.2), (int)(Height * 0.1), 
                    (flashers <= 3) ? Color.Gold : Color.LightYellow, 1, Color.DarkOrange);
            }
            else if (Speed < 0) // lights going up
            {
                canvas.AddCenteredEllipse(X + (int)(Width * 0.2), Y + (int)(Height * 0.6), (int)(Width * 0.2), (int)(Height * 0.1), 
                    (flashers <= 3) ? Color.Gold : Color.LightYellow, 1, Color.DarkOrange);
                canvas.AddCenteredEllipse(X + (int)(Width * 0.4), Y + (int)(Height * 0.6), (int)(Width * 0.2), (int)(Height * 0.1), 
                    (flashers > 3 && flashers <= 6) ? Color.Gold : Color.LightYellow, 1, Color.DarkOrange);
                canvas.AddCenteredEllipse(X + (int)(Width * 0.6), Y + (int)(Height * 0.6), (int)(Width * 0.2), (int)(Height * 0.1), 
                    (flashers > 6 && flashers <= 9) ? Color.Gold : Color.LightYellow, 1, Color.DarkOrange);
                canvas.AddCenteredEllipse(X + (int)(Width * 0.8), Y + (int)(Height * 0.6), (int)(Width * 0.2), (int)(Height * 0.1), 
                    (flashers > 9 && flashers <= 12) ? Color.Gold : Color.LightYellow, 1, Color.DarkOrange);
            }

            // crashed
            if (crashed > 0) ShowCrash();
        }

        // implemented scoring for safely crossing the intersection or penalty for a collision
        protected override void VSafeScore() => score += 60;
        protected override void VHitScore() => score -= 75;

        // implement animation effect
        public void Animate()
        {
            if (flashers == 12) flashers = 1;
            else flashers += 1;
        }

    }
    #endregion Vertical Truck

    #region Horizontal Truck
    // derive a new concrete class HTruck
    public class HTruck : HorizontalCar, IAnimateable
    {
        // add a protected color member initialized to a random color
        protected Color color = RandColor.GetColor();

        // add a variable to hold the animation frame
        private int flashers = 0;

        // Add a constructor with speed, width (default 40), and height (default 80)
        public HTruck(int speed, int width = 80, int height = 40)
            : base(speed, width, height)
        {
        }

        public override Rectangle GetRect() => new Rectangle(X, Y, Width, Height);

        protected override void VShowCar()
        {
            // animate the lights
            Animate();

            // tires
            canvas.AddCenteredEllipse(X + (int)(Width * 0.1), Y, (int)(Width * 0.2), (int)(Height * 0.2), Color.Black);
            canvas.AddCenteredEllipse(X + (int)(Width * 0.1), Y + Height, (int)(Width * 0.2), (int)(Height * 0.2), Color.Black);
            canvas.AddCenteredEllipse(X + (int)(Width * 0.9), Y, (int)(Width * 0.2), (int)(Height * 0.2), Color.Black);
            canvas.AddCenteredEllipse(X + (int)(Width * 0.9), Y + Height, (int)(Width * 0.2), (int)(Height * 0.2), Color.Black);

            if (Speed > 0) // headlights going right
            {
                canvas.AddCenteredEllipse(X + (int)(Width * 0.9), Y + (int)(Height * 0.2), (int)(Width * 0.3), (int)(Height * 0.4), Color.LightYellow, 1, Color.Black);
                canvas.AddCenteredEllipse(X + (int)(Width * 0.9), Y + (int)(Height * 0.8), (int)(Width * 0.3), (int)(Height * 0.4), Color.LightYellow, 1, Color.Black);
            }
            else if (Speed < 0) // headlights going left
            {
                canvas.AddCenteredEllipse(X + (int)(Width * 0.1), Y + (int)(Height * 0.2), (int)(Width * 0.3), (int)(Height * 0.4), Color.LightYellow, 1, Color.Black);
                canvas.AddCenteredEllipse(X + (int)(Width * 0.1), Y + (int)(Height * 0.8), (int)(Width * 0.3), (int)(Height * 0.4), Color.LightYellow, 1, Color.Black);
            }

            // body
            canvas.AddRectangle(GetRect(), color, 1, Color.Black);

            if (Speed > 0) // windshield and box, going right
            {
                canvas.AddRectangle(X + (int)(Width * 0.6), Y + (int)(Height * 0.1), (int)(Width * 0.3), (int)(Height * 0.8), Color.SlateGray, 1, Color.DarkSlateGray);
                canvas.AddRectangle(X + (int)(Width * 0.1), Y + (int)(Height * 0.1), (int)(Width * 0.4), (int)(Height * 0.8), this.color, 2, Color.Brown);
            }
            else if (Speed < 0) // windshield and box going left
            {
                canvas.AddRectangle(X + (int)(Width * 0.1), Y + (int)(Height * 0.1), (int)(Width * 0.3), (int)(Height * 0.8), Color.Gray, 1, Color.DarkSlateGray);
                canvas.AddRectangle(X + (int)(Width * 0.5), Y + (int)(Height * 0.1), (int)(Width * 0.4), (int)(Height * 0.8), this.color, 2, Color.Brown);
            }

            // lights on the truck box to animate
            if (Speed > 0) // lights going right
            {
                canvas.AddCenteredEllipse(X + (int)(Width * 0.5), Y + (int)(Height * 0.2), (int)(Width * 0.1), (int)(Height * 0.2), 
                    (flashers <= 3) ? Color.Gold : Color.LightYellow, 1, Color.DarkOrange);
                canvas.AddCenteredEllipse(X + (int)(Width * 0.5), Y + (int)(Height * 0.4), (int)(Width * 0.1), (int)(Height * 0.2), 
                    (flashers > 3 && flashers <= 6) ? Color.Gold : Color.LightYellow, 1, Color.DarkOrange);
                canvas.AddCenteredEllipse(X + (int)(Width * 0.5), Y + (int)(Height * 0.6), (int)(Width * 0.1), (int)(Height * 0.2), 
                    (flashers > 6 && flashers <= 9) ? Color.Gold : Color.LightYellow, 1, Color.DarkOrange);
                canvas.AddCenteredEllipse(X + (int)(Width * 0.5), Y + (int)(Height * 0.8), (int)(Width * 0.1), (int)(Height * 0.2), 
                    (flashers > 9 && flashers <= 12) ? Color.Gold : Color.LightYellow, 1, Color.DarkOrange);
            }
            else if (Speed < 0) // lights going left
            {
                canvas.AddCenteredEllipse(X + (int)(Width * 0.6), Y + (int)(Height * 0.2), (int)(Width * 0.1), (int)(Height * 0.2), 
                    (flashers > 9 && flashers <= 12) ? Color.Gold : Color.LightYellow, 1, Color.DarkOrange);
                canvas.AddCenteredEllipse(X + (int)(Width * 0.6), Y + (int)(Height * 0.4), (int)(Width * 0.1), (int)(Height * 0.2), 
                    (flashers > 6 && flashers <= 8) ? Color.Gold : Color.LightYellow, 1, Color.DarkOrange);
                canvas.AddCenteredEllipse(X + (int)(Width * 0.6), Y + (int)(Height * 0.6), (int)(Width * 0.1), (int)(Height * 0.2), 
                    (flashers > 3 && flashers <= 6) ? Color.Gold : Color.LightYellow, 1, Color.DarkOrange);
                canvas.AddCenteredEllipse(X + (int)(Width * 0.6), Y + (int)(Height * 0.8), (int)(Width * 0.1), (int)(Height * 0.2), 
                    (flashers <= 3) ? Color.Gold : Color.LightYellow, 1, Color.DarkOrange);
            }

            // crashed
            if (crashed > 0) ShowCrash();
        }

        // implemented scoring for safely crossing the intersection or penalty for a collision
        protected override void VSafeScore() => score += 60;
        protected override void VHitScore() => score -= 75;

        public void Animate()
        {
            if (flashers == 12) flashers = 1;
            else flashers += 1;
        }
    }
    #endregion Horizontal Truck

    #region Vertical Bike
    // derive a new concrete class VTruck
    public class VBike : VerticalCar
    {
        // add a protected color member initialized to a random color
        protected Color color = RandColor.GetColor();

        // Add a constructor with speed, width (default 40), and height (default 80)
        public VBike(int speed, int width = 10, int height = 40)
            : base(speed, width, height)
        {
            X += 15;
        }

        public override Rectangle GetRect() => new Rectangle(X, Y, Width, Height);

        protected override void VShowCar()
        {
            if (Speed > 0) // headlights going down
            {
                canvas.AddCenteredEllipse(X + (int)(Width * 0.2), Y + (int)(Height * 0.9), (int)(Width * 0.4), (int)(Height * 0.5), Color.LightYellow, 1, Color.Black);
                canvas.AddCenteredEllipse(X + (int)(Width * 0.8), Y + (int)(Height * 0.9), (int)(Width * 0.4), (int)(Height * 0.5), Color.LightYellow, 1, Color.Black);
            }
            else if (Speed < 0) // headlights going up
            {
                canvas.AddCenteredEllipse(X + (int)(Width * 0.2), Y + (int)(Height * 0.1), (int)(Width * 0.4), (int)(Height * 0.5), Color.LightYellow, 1, Color.Black);
                canvas.AddCenteredEllipse(X + (int)(Width * 0.8), Y + (int)(Height * 0.1), (int)(Width * 0.4), (int)(Height * 0.5), Color.LightYellow, 1, Color.Black);
            }

            // tires
            canvas.AddCenteredEllipse(X + (int)(Width * 0.5), Y, (int)(Width * 0.6), (int)(Height * 0.3), Color.Black);
            canvas.AddCenteredEllipse(X + (int)(Width * 0.5), Y + Height, (int)(Width * 0.6), (int)(Height * 0.3), Color.Black);

            // body
            canvas.AddRectangle(GetRect(), color, 1, Color.Black);

            if (Speed > 0) // windshield going down
            {
                canvas.AddRectangle(X, Y + (int)(Height * 0.6), (Width), (int)(Height * 0.4), Color.SlateGray, 1, Color.DarkSlateGray);
            }
            else if (Speed < 0) // windshield going up
            {
                canvas.AddRectangle(X, Y, (Width), (int)(Height * 0.4), Color.Gray, 1, Color.DarkSlateGray);
            }

            // crashed
            if (crashed > 0) ShowCrash();

        }

        protected override void VSafeScore() => score += 85;
        protected override void VHitScore() => score -= 125;

    }

    #endregion Vertical Bike

    #region Horizontal Bike
    public class HBike : HorizontalCar
    {
        // add a protected color member initialized to a random color
        protected Color color = RandColor.GetColor();

        // Add a constructor with speed, width (default 40), and height (default 80)
        public HBike(int speed, int width = 40, int height = 10)
            : base(speed, width, height)
        {
            Y += 15;
        }

        public override Rectangle GetRect() => new Rectangle(X, Y, Width, Height);

        protected override void VShowCar()
        {
            if (Speed > 0) // headlights going right
            {
                canvas.AddCenteredEllipse(X + (int)(Width * 0.9), Y + (int)(Height * 0.2), (int)(Width * 0.5), (int)(Height * 0.4), Color.LightYellow, 1, Color.Black);
                canvas.AddCenteredEllipse(X + (int)(Width * 0.9), Y + (int)(Height * 0.8), (int)(Width * 0.5), (int)(Height * 0.4), Color.LightYellow, 1, Color.Black);
            }

            else if (Speed < 0) // headlights going left
            {
                canvas.AddCenteredEllipse(X + (int)(Width * 0.1), Y + (int)(Height * 0.2), (int)(Width * 0.5), (int)(Height * 0.4), Color.LightYellow, 1, Color.Black);
                canvas.AddCenteredEllipse(X + (int)(Width * 0.1), Y + (int)(Height * 0.8), (int)(Width * 0.5), (int)(Height * 0.4), Color.LightYellow, 1, Color.Black);
            }

            // tires
            canvas.AddCenteredEllipse(X, Y + (int)(Height * 0.5), (int)(Width * 0.3), (int)(Height * 0.6), Color.Black);
            canvas.AddCenteredEllipse(X + Width, Y + (int)(Height * 0.5), (int)(Width * 0.3), (int)(Height * 0.6), Color.Black);

            // body
            canvas.AddRectangle(GetRect(), color, 1, Color.Black);

            if (Speed > 0) // windshield and box, going right
            {
                canvas.AddRectangle(X + (int)(Width * 0.6), Y, (int)(Width * 0.3), (int)(Height), Color.SlateGray, 1, Color.DarkSlateGray);
            }
            else if (Speed < 0) // windshield and box going left
            {
                canvas.AddRectangle(X + (int)(Width * 0.1), Y, (int)(Width * 0.3), (int)(Height), Color.Gray, 1, Color.DarkSlateGray);
            }

            // crashed
            if (crashed > 0) ShowCrash();
        }

        protected override void VSafeScore() => score += 80;
        protected override void VHitScore() => score -= 125;

    }
    #endregion Horizontal Bike

    #region Tank
    // for funsies create a tank to roll through the intersection
    public class HTank : HorizontalCar, IAnimateable
    {
        // add a protected color member initialized to a random color
        private Color color = Color.DarkOliveGreen;

        // a static list of shells, so it persists through any generation of tanks
        internal static List<TankShell> shells;

        // set a field for the swivel distance and swivel direction of the tank barrel
        internal double swivel = 0.0; internal int swivelDirection = 1;

        // set a field for the animation of the tank treads
        private int tread = 0;

        // add a static constructor for the list of shells
        static HTank() => shells = new List<TankShell>();

        // Add a constructor with speed, width (default 40), and height (default 80)
        public HTank(int speed = 5, int width = 160, int height = 120)
            : base(speed, width, height)
        {
            // adjust the super wide tank to fit across all three lanes of traffic
            if (speed > 0) Y -= 90;
            else Y += 10;
        }

        // override the GetRect to return the tanks bounding rectangle
        public override Rectangle GetRect() => new Rectangle(X, Y, Width, Height);

        // override the virtual ShowCar to animate, draw, and render the tank features to the canvas
        protected override void VShowCar()
        {
            Animate();

            // body
            canvas.AddRectangle(GetRect(), color, 1, Color.Black);

            // treads
            canvas.AddCenteredEllipse(X + 5, Y - 5, 20, 20, Color.DarkGreen, 1, Color.Black);
            canvas.AddCenteredEllipse(X + Width - 5, Y - 5, 20, 20, Color.DarkGreen, 1, Color.Black);
            canvas.AddCenteredEllipse(X + 5, Y + Height + 5, 20, 20, Color.DarkGreen, 1, Color.Black);
            canvas.AddCenteredEllipse(X + Width - 5, Y + Height + 5, 20, 20, Color.DarkGreen, 1, Color.Black);
            canvas.AddCenteredRectangle(X + (int)(Width * 0.5), Y - 5, Width, 20, Color.DarkGreen, 1, Color.Black);
            canvas.AddCenteredRectangle(X + (int)(Width * 0.5), Y + Height + 5, Width, 20, Color.DarkGreen, 1, Color.Black);

            // tread lines
            for (int i = 0; i < 160; i += 20)
            {
                canvas.AddLine((Speed > 0) ? X + i + tread: X + i + 20 - tread, Y - 14,
                    (Speed > 0) ? X + i + tread : X + i + 20 - tread, Y + 4, Color.MediumAquamarine, 2);
            }
            for (int i = 0; i < 160; i += 20)
            {
                canvas.AddLine((Speed > 0) ? X + i + tread : X + i + 20 - tread, Y + Height - 4, 
                    (Speed > 0) ? X + i + tread : X + i + 20 - tread, Y + Height + 14, Color.MediumAquamarine, 2);
            }

            // turret
            canvas.AddCenteredEllipse(X + (int)(Width * 0.5), Y + (int)(Height * 0.5), (int)(Width * 0.4), (int)(Height * 0.3), Color.DarkCyan, 1, Color.DarkGreen);

            // barrel
            if (Speed > 0) // barrel going right
            {
                canvas.AddCenteredEllipse(X + 120, Y + (int)(Height * 0.5) + (int)swivel, 60, 12, Color.Green, 1, Color.Black);
                canvas.AddCenteredEllipse(X + 185, Y + (int)(Height * 0.5) + (int)swivel, 40, 10, Color.Green, 1, Color.Black);
                canvas.AddCenteredEllipse(X + 190, Y + (int)(Height * 0.5) + (int)swivel, 20, 6, Color.DarkSeaGreen);
                canvas.AddCenteredRectangle(X + 165, Y + (int)(Height * 0.5) + (int)swivel, 60, 10, Color.Green, 1, Color.Black);
            }
            else if (Speed < 0) // barrel going left
            {
                canvas.AddCenteredEllipse(X + 40, Y + (int)(Height * 0.5) + (int)swivel, 60, 12, Color.Green, 1, Color.Black);
                canvas.AddCenteredEllipse(X - 25, Y + (int)(Height * 0.5) + (int)swivel, 40, 10, Color.Green, 1, Color.Black);
                canvas.AddCenteredEllipse(X - 30, Y + (int)(Height * 0.5) + (int)swivel, 20, 6, Color.DarkSeaGreen);
                canvas.AddCenteredRectangle(X - 5, Y + (int)(Height * 0.5) + (int)swivel, 60, 10, Color.Green, 1, Color.Black);
            }

            // randomly fire a new shell and add it to the list of shells
            if (Car.randVal.Next(20) == 0) 
                shells.Add(new TankShell(new Point(Speed > 0 ? X + 190 : X - 30, Y + (int)(Height * 0.5) + (int)swivel), Speed > 0 ? 1 : -1));
            // animate the movement of each shell in the list
            shells.ForEach(shell => shell.Animate());
        }

            // the tank cannot crash, so no hit score, but bonus for each tank that gets through without ending the game
            protected override void VSafeScore() => score += 350;
            protected override void VHitScore() => score -= 0;

        // the tank is immune to crashing
        public new void Crash() { return; }

        // animate the movement of the barrel (a swivel would be nice, but currently it just slides)
        public void Animate()
        {
            // bound the barrel so it rotates back and forth
            if (swivel < -45)
            {
                swivel = -45;
                swivelDirection *= -1;
            }
            else if (swivel > 45)
            {
                swivel = 45;
                swivelDirection *= -1;
            }
            // move the barrel to the next position
            swivel += (Car.randVal.NextDouble() * 2 + 2) * swivelDirection;

            // increment the treads
            tread = (tread == 20) ? 0: tread + 2;
        }
    }

    #endregion Tank

    #region Tank Shell
    // for extra funsies, have the tank shoot shells at the various other vehicles
    internal class TankShell : IAnimateable
    {
        // set a field and property for the current position of the shell
        private PointF Pos;
        public PointF pos { get => Pos; set => Pos = value; }

        // set a private field for the origin of the shell (for the barrel fire)
        private Point origin;

        // set a private field for the direction and a frame count of the shell
        private int direction;
        private int count = 0;

        // create a private list of shell points to draw a polygon bullet shape
        private List<PointF> shellPoint;

        // initialize a sfx for when the shell is first shot
        internal SoundPlayer shot = new SoundPlayer(Properties.Resources.tankShot);

        // create a constructor to build each new shell
        public TankShell(Point position, int direction)
        {
            // set the origin, current position, and direction based on the tanks data
            Pos.X = origin.X = position.X;
            Pos.Y = origin.Y = position.Y;
            this.direction = direction;

            // initialize the list of shell points and play the sfx
            shellPoint = new List<PointF>();
            shot.Play();
        }

        // return a bounding box for where shell is located
        public Rectangle GetRect() => new Rectangle((int)Pos.X - 10, (int)Pos.Y - 5, 20, 10);

        /*// override Equals() to return true if the shell hit a vehicle
        public override bool Equals(object obj)
        {
            if (!(obj is Car otherC))
                if (!(obj is TankShell otherS)) return false;
                else return this.GetRect().IntersectsWith(otherS.GetRect());
            else return this.GetRect().IntersectsWith(otherC.GetRect());
        }

        public override int GetHashCode() => (int)Pos.X * 10 + (int)Pos.Y * 500;
        */

        // borrowing the car equals to test shell collisions with cars
        public override bool Equals(object obj)
        {
            if (!(obj is Car other)) return false;
            return this.GetRect().IntersectsWith(other.GetRect());
        }

        // override GetHashCode() to return 0
        public override int GetHashCode() => 0;

        // add a helper predicate to check if the shell is off the canvas
        public static bool OutOfBounds(TankShell shell)
        {
            Rectangle boundary = new Rectangle(-20, -20, Car.canvas.ScaledWidth + 20, Car.canvas.ScaledHeight + 20);
            return !boundary.IntersectsWith(shell.GetRect());
        }

        // add some movement to the shell to pull it away from the tank
        public void Animate()
        {
            // clear the list of points and add points based on the shells current position
            shellPoint.Clear();
            shellPoint.Add(new PointF(Pos.X + 10 * direction, Pos.Y));
            shellPoint.Add(new PointF(Pos.X + 4 * direction, Pos.Y - 4));
            shellPoint.Add(new PointF(Pos.X - 2 * direction, Pos.Y - 5));
            shellPoint.Add(new PointF(Pos.X - 10 * direction, Pos.Y - 5));
            shellPoint.Add(new PointF(Pos.X - 10 * direction, Pos.Y + 5));
            shellPoint.Add(new PointF(Pos.X - 2 * direction, Pos.Y + 5));
            shellPoint.Add(new PointF(Pos.X + 4 * direction, Pos.Y + 4));

            // add a burst from the tank barrel for the first few frames
            if (count < 8)
            {
                Car.canvas.AddPolygon(origin.X + (direction * (30 + count * 3)), origin.Y - count - 4, count + 2, 12, FillColor: Color.Red);
                if (count > 3) Car.canvas.AddPolygon(origin.X + (direction * (30 + count * 3)), origin.Y - count - 1, count - 1, 12, FillColor: Color.Firebrick);
            }

            // render a polygon of a bullet shape based on the shells points
            Car.canvas.AddSolidPolygon(shellPoint, Color.Yellow, 1, Color.Black);
            Car.canvas.Render();

            // move the shell to the next position and increment the frame count
            Pos.X += direction * 12;
            count++;
        }
    }
    #endregion
}
