using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recrystallization
{

    public enum NeighborhoodMethod
    {
        VonNeumann =0,
        Moore =1,
        HexagonalLeft = 2,
        HexagonalRight = 3,
        HexagonalRandom = 4,
        PentagonalRandom = 5
    }

    public enum GenerateSeedsMethod
    {
        Random =0,
        Evenly = 1,
        RandomR = 2,
        ClickMethod = 3
    }

    public enum BoundaryValue
    {
        Periodic =0,
        NonPeriodical =1
    }

    public enum SimulationStatus
    {
        Growthing = 1,
        EndGrowth = 2,
        Recrystallizationing = 3,
        EndRecrystallization = 4,
        MC = 5,
        EndMC =6
    }

}
