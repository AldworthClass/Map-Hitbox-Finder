# Project Title
This application's purpose is to allow you to use your mouse to draw collision rectangles for your game, and provide you with the coordinates and dimensions of each.

## Installation
Simply clone or download the program, and open in in Visual Studio.
You may need to change the target framework if you have a newer version of VS.

## Usage
<ins>SETUP</ins>

1 - Open up this solution in Visual Studio (or another IDE of your choice).  
2 - Use the MGCB tool to add your game map/background image (if you have one).  
3 - Open Game1.cs in the Solution Explorer.  
4 - In LoadContent(), load your gameworld texture into 'worldTexture' (uncomment out the line of code to do this).  
5 - In Draw(), uncomment out the line of code that draws the world texture (scaled to worldRect).  
6 - Set the Rectangle 'worldRect' to the size you want your game world to be.  
7 -You may scale 'window' to the size that you want your game window to be as well.  
8 - Draw your background image using the included _spriteBatch.Draw().  

<ins>USE PROGRAM</ins>

In game, press 'I' to toggle instructions on/off.

1 - Run the program.  
2 - Left click and drag to create rectangles on your map where you want them to be.  
3 - Right click on a rectangle to delete it. 
4 - Change color: B=Black, Y=Yellow, R=Red, G=Green  
5 - Use WASD to pan the map around.  
6 - Hit Spacebar to bring game window back to origin.  
7 - Hold left 'Ctrl' and scroll mouse wheel to zoom in/out.  
8 - Press 'Z' to return to normal zoom.  
9 - Press 'ENTER' to print the list of rectangles to the output console.  

## Troubleshooting
If you can't open the MGCB tool, you are likley using a newer version of Monogame than I used when making this program.  To fix this:

1 - Go to the "Project" menu.  
2 - Select Map Hitbox Finder Properties.  
3 - In the Application-General section, under Target Framework, select the framework you have installed.  
4 - Close the properties window, save your project, close it, re-open it, try running it once, and try opening the MGCB tool again.  