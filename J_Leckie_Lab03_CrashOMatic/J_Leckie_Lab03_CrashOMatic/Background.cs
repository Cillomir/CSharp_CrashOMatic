/***************************************************
* Lab 03: Derived Background
* 
* This portion of the lab is drawn from ICA 13 to
*   utilize a derived class of the GDI Drawer. It
*   sets an image as the background for the drawing
*   canvas.
* 
* Author: Joel Leckie
* CMPE 2300 – OE01: Spring 2022
**************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GDIDrawer;

namespace J_Leckie_Lab03_CrashOMatic
{
    // Use a derived class to generate a background image
    internal class Background : CDrawer
    {
        public Background(Bitmap image, bool isColor = true)
            : base(image, false)
        {
            if (isColor)
            {
                // set the back-buffer pixels to the pixels in the supplied bitmap
                for (int y = 0; y < image.Height; y++)
                    for (int x = 0; x < image.Width; x++)
                        base.SetBBPixel(x, y, image.GetPixel(x, y));
            }
            else
            {
                // set the back-buffer pixels to a grayscale version of the supplied bitmap
                for (int y = 0; y < image.Height; y++)
                    for (int x = 0; x < image.Width; x++)
                    {
                        int pixelVal = (int)Math.Floor((decimal)(image.GetPixel(x, y).R + image.GetPixel(x, y).G + image.GetPixel(x, y).B) / 3);
                        Color pixel = Color.FromArgb(pixelVal, pixelVal, pixelVal);
                        base.SetBBPixel(x, y, pixel);
                    }
            }
            base.Render();
        }
    }
}
