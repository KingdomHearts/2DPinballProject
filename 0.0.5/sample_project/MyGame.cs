using System;
using GXPEngine;
using System.Drawing;
using GXPEngine.OpenGL;

public class MyGame : Game
{	
	Canvas canvas1;
	Canvas canvas2;

	float counter = 0;

	public MyGame () : base(800, 600, false, true)
	{
		canvas1 = createCanvas ();
		canvas2 = createCanvas ();
		//canvas2.blendMode = BlendMode.MULTIPLY;

		AddChild (canvas1);
	}

	Canvas createCanvas() {
		//create a canvas
		Canvas canvas = new Canvas(300, 300);
		canvas.SetOrigin (150, 150);
		canvas.x = width / 2;
		canvas.y = height / 2;

		//add some content
		canvas.graphics.FillRectangle(new SolidBrush(Color.Red), new Rectangle(0, 0, 150, 150));
		canvas.graphics.FillRectangle(new SolidBrush(Color.Blue), new Rectangle(150, 0, 150, 150));
		canvas.graphics.FillRectangle(new SolidBrush(Color.Yellow), new Rectangle(0, 150, 150, 150));
		canvas.graphics.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(150, 150, 150, 150));

		//add canvas to display list
		//AddChild(canvas);
		return canvas;
	}
	
	void Update () {
		//empty
		//Console.WriteLine ("Last frame time in ms:{0}, TARGET FPS: {1}, REAL FPS:{2}",Time.deltaTime, targetFps, currentFps);
		//canvas1.rotation++;
		//canvas2.rotation--;

		canvas1.x = (float)(width / 2 + 200 * Math.Sin (counter++ / 100.0f));
	}

	static void Main() {
		new MyGame().Start();
	}
}

