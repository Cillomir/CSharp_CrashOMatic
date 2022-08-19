/***************************************************
* Lab 03: Crash-O-Matic
* 
* This lab demonstrates a culmination of features from the
*   CMPE 2300 course: Object-Orientated Programming. It 
*   utilizes a derived class based on the GDI Drawer to
*   render a background image. An abstract class is created
*   to create various types of Cars which will be animated
*   on screen and affected by user selection.
* ...
* 
* Author: Joel Leckie
* CMPE 2300 – OE01: Spring 2022
**************************************************/


using GDIDrawer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using System.Windows.Forms;

namespace J_Leckie_Lab03_CrashOMatic
{
    public partial class Form1 : Form
    {
        // initialize a list of car types to be randomized from (tank is randomized separately)
        enum CarTypes
        {
            HAmbulance,
            VSedan,
            HSedan,
            VTruck,
            HTruck,
            VBike, 
            HBike
        };

        // declare a list of vehicles
        List<Car> allCars;

        // declare a stopwatch to measure seconds passed
        System.Diagnostics.Stopwatch timePassed;

        // add a generic game start sound
        SoundPlayer start = new SoundPlayer(Properties.Resources.start);

        // add a collision tracker
        public int collisionCount;

        // add a pause option (will be based on a right mouse click
        private bool paused;

        // add an agent to randomize a tank appearance and create a pause before the next car is generated
        private int tankCount;
        private int nextTank;
        private int nextCar;

        public Form1()
        {
            InitializeComponent();
            
            // initialize list of vehicles and stopwatch
            allCars = new List<Car>();
            timePassed = new System.Diagnostics.Stopwatch();
            // subscribe to the right mouse click to pause the gameplay
            Car.canvas.MouseRightClick += Canvas_MouseRightClick;
            paused = true;

            // a comparison canvas in grayscale
            //CDrawer color = new Background(Properties.Resources.Crash);
            //CDrawer noColor = new Background(Properties.Resources.Crash, false);
        }

        private void AniTimer_Tick(object sender, EventArgs e)
        {
            // return if things aren't ready
            if (Car.canvas == null) return;

            // add a new random vehicle every 2 seconds (3 seconds if a tank just generated)
            if (timePassed.ElapsedMilliseconds >= nextCar)
            {
                timePassed.Restart();

                // get a tank after a random number of cars have generated
                tankCount++;
                if (tankCount == nextTank)
                {
                    allCars.Add(new HTank(speed: (Car.randVal.Next(2) == 1) ? 5 : -5));
                    tankCount = 0;
                    nextTank = Car.randVal.Next(12, 15);
                    // generate the next car after 3 seconds
                    nextCar = 3000;
                }
                else
                {
                    // get a random vehicle from the enum of eligible vehicles
                    int carType = Car.randVal.Next(Enum.GetValues(typeof(CarTypes)).Length);
                    // generate the next car after 2 seconds
                    nextCar = 2000;

                    switch ((CarTypes)carType)
                    {
                        // generate the selected vehicle going in a random direction (left or right, up or down)
                        case CarTypes.HAmbulance:
                            allCars.Add(Car.randVal.Next(2) == 1 ? new HAmbulance() : new HAmbulance(-7));
                            break;
                        case CarTypes.VSedan:
                            allCars.Add(new VSedan(Car.randVal.Next(2) == 1 ? Car.randVal.Next(5, 11) : Car.randVal.Next(-11, -5)));
                            break;
                        case CarTypes.HSedan:
                            allCars.Add(new HSedan(Car.randVal.Next(2) == 1 ? Car.randVal.Next(5, 11) : Car.randVal.Next(-11, -5)));
                            break;
                        case CarTypes.VTruck:
                            allCars.Add(new VTruck(Car.randVal.Next(2) == 1 ? Car.randVal.Next(5, 11) : Car.randVal.Next(-11, -5)));
                            break;
                        case CarTypes.HTruck:
                            allCars.Add(new HTruck(Car.randVal.Next(2) == 1 ? Car.randVal.Next(5, 11) : Car.randVal.Next(-11, -5)));
                            break;
                        case CarTypes.VBike:
                            allCars.Add(new VBike(Car.randVal.Next(2) == 1 ? Car.randVal.Next(8, 14) : Car.randVal.Next(-14, -8)));
                            break;
                        case CarTypes.HBike:
                            allCars.Add(new HBike(Car.randVal.Next(2) == 1 ? Car.randVal.Next(8, 14) : Car.randVal.Next(-14, -8)));
                            break;
                    }
                }
            }

            // toggle the speed of a car that is clicked
            if (Car.canvas.GetLastMouseLeftClick(out Point pos))
                if (!paused) // make sure the game isn't paused, no cheating!
                {
                    allCars.ForEach(car => { if (car.PointOnCar(pos) == true) car.ToggleSpeed(); });
                }

            // prepare the canvas for all the action below
            Car.canvas.Clear();
            
            // iterate and move each car
            allCars.ForEach(car => car.Move());

            // increment the score and remove any cars that made it outside the canvas
            allCars.Where(Car.OutOfBounds).ToList().ForEach(car => car.GetSafeScore());
            allCars.RemoveAll(Car.OutOfBounds);

            // find cars involved in a collision, but not tanks (tanks are immune to crashes)
            List<Car> collisions = allCars.Where(car => allCars.Any(car2 => car.Equals(car2) && !ReferenceEquals(car, car2))).ToList();
            collisions.RemoveAll(car => car is HTank);


            // if a tank exists, check for tank shells
            if (allCars.Any(car => car is HTank))
            {
                // each shell, each car, equals, add to collisions with no duplicates
                foreach (TankShell shell in HTank.shells)
                {
                    List<Car> tanked = allCars.Where(car => shell.Equals(car)).ToList();
                    collisions = collisions.Union(tanked).ToList();
                }
                // remove shells that hit a car or went off the screen
                HTank.shells.RemoveAll(shell => allCars.Any(car => shell.Equals(car)));
                HTank.shells.RemoveAll(TankShell.OutOfBounds);
            }

            // iterate through the cars that have crashed and remove them after 10 ticks
            collisions.ForEach(car => car.Crash());
            allCars.Where(car => car.crashed >= 10).ToList().ForEach(car => { collisionCount++; car.GetHitScore(); }) ;
            allCars.RemoveAll(car => car.crashed >= 10);

            // render each car that remains
            allCars.ForEach(car => car.ShowCar());
            Car.canvas.Render();

            // display the new score and collision count
            Invoke(new Action(() => lbl_score.Text = "Score: " + Car.score.ToString()));
            Invoke(new Action(() => lbl_collisions.Text = "Vehicles Lost: " + collisionCount.ToString()));

            // end game if score goes negative or more than 10 cars are lost
            if ((Car.score < 0) || (collisionCount > 10)) EndGame();
        }

        // pause and unpause the gameplay on a right mouse click
        private void Canvas_MouseRightClick(Point pos, CDrawer dr)
        {
            if (!paused)
            {
                // pause the intervals, pause the stopwatch, enable the start button
                AniTimer.Enabled = false;
                timePassed.Stop();
                Invoke(new Action(() => btn_Start.Enabled = true));
                paused = true;
                Car.canvas.AddText("Paused", 24, Color.DeepSkyBlue);
                Car.canvas.Render();
            }
            else
            {
                // start the intervals, start the stopwatch, disable the start button
                AniTimer.Enabled = true;
                timePassed.Start();
                Invoke(new Action(() => btn_Start.Enabled = false));
                paused = false;
            }
        }

        // start a new game, resetting all the applicable variables
        private void btn_Start_Click(object sender, EventArgs e)
        {
            // set the intervals, stopwatch, and pause menu
            AniTimer.Enabled = true;
            timePassed.Restart();
            paused = false;
            // play an opening sfx
            start.Play();
            // clear the lists and set initial variables
            allCars.Clear();
            collisionCount = 0;
            nextTank = Car.randVal.Next(15, 20);
            nextCar = 2000;
            tankCount = 0;
            Car.score = 0;
            // disable the start button
            btn_Start.Enabled = false;

        }
      
        private void EndGame()
        {
            // stop the intervals, pause the gameplay, stop the stopwatch
            AniTimer.Enabled = false;
            paused = true;
            timePassed.Stop();
            // enable the start button
            Invoke(new Action(() => btn_Start.Enabled = true));

            // display a game over screen and reason the game ended, render to the canvas
            Car.canvas.AddText("Game Over", 24, Car.canvas.ScaledWidth / 2 - 200, Car.canvas.ScaledHeight / 2 - 75, 400, 50, Color.Maroon);
            if (Car.score < 0) 
                Car.canvas.AddText("Your score went below 0", 20, Car.canvas.ScaledWidth / 2 - 300, Car.canvas.ScaledHeight / 2 + 25, 600, 40, Color.Firebrick);
            else 
                Car.canvas.AddText("Too many cars crashed", 20, Car.canvas.ScaledWidth / 2 - 300, Car.canvas.ScaledHeight / 2 + 25, 600, 40, Color.Firebrick);
            Car.canvas.Render();
        }
    }
}
