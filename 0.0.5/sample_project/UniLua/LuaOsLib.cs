
namespace UniLua
{
	using System.Diagnostics;

	internal class LuaOSLib
	{
		public const string LIB_NAME = "os";

		public static int OpenLib( ILuaState lua )
		{
			NameFuncPair[] define = new NameFuncPair[]
			{
			};

			lua.L_NewLib( define );
			return 1;
		}

	}
}

