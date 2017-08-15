using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MementoPattern
{

    class GameStateCaretaker
    {
        private GameState gameState;

        public void SetGameState(GameState state)
        {
            this.gameState = state;
        }

        public GameState GetGameState()
        {
            return this.gameState;
        }
    }
}
