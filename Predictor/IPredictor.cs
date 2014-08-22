using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Predictor
{
    public interface IPredictor
    {
        void Configure(IEnumerable<Game> games, IEnumerable<Player> players);
        double Predict(Game g);
        void PrintDebug();
    }
}
