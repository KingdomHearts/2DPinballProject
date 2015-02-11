using System;
using GXPEngine;
using System.Drawing;
using GXPEngine.OpenGL;
using UniLua;

public class MyGame : Game
{
    LevelController LC;
	public MyGame () : base(960, 540, false, true)
	{
        LC = new LevelController(game.width,game.height);
        //HHIIIEIEIEIHEIEHIEHEOIHFL:DAHFL:SDHF:LSDKHF:LH
	}

	void Update () {
        AddChild(LC);
	}

	static void Main() {
		new MyGame().Start();
	}
}

