using System;
using GXPEngine;
using System.Drawing;
using GXPEngine.OpenGL;
using UniLua;

public class MyGame : Game
{
	public MyGame () : base(960, 540, false, true)
	{
        LevelController LC = new LevelController();
        AddChild(LC);
        //HHIIIEIEIEIHEIEHIEHEOIHFL:DAHFL:SDHF:LSDKHF:LH
	}

	void Update () {
	}

	static void Main() {
		new MyGame().Start();
	}
}

