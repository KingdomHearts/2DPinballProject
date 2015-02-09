using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    class LevelController : GameObject
    {
        public LevelController()
        {

        }

        public void LevelState(/*enum of andere mogelijk om level te bepalen*/)
        {
        }

        private void LevelReader(/*enum of andere mogelijk om level te bepalen*/)
        {
            //Vanuit LevelState het bestand meesturen dat hierin wordt opgehaald.
        }

        private void LevelBuilder(/* array van XML */)
        {
            //De array wordt hierin uitgelezen en gebouwd. Vanuit deze wordt de bijpassende CS aangeroepen waar de rest van de events wordt uitgevoerd.
        }
    }
}
