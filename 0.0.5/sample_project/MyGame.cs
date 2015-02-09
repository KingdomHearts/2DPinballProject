using System;
using GXPEngine;
using System.Drawing;
using GXPEngine.OpenGL;

public class MyGame : Game
{
	public MyGame () : base(800, 600, false, true)
	{
        LevelController LC = new LevelController();
        AddChild(LC);
	}

	void Update () {
	}

	static void Main() {
		new MyGame().Start();
	}
}

