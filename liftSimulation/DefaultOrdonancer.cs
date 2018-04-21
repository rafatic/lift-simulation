using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace liftSimulation
{
    public class DefaultOrdonancer : FloorOrdonancer
    {

        public DefaultOrdonancer(int heading = 0, int currentFloor = 0) : base(heading, currentFloor)
        {

        }

        public override List<int> Sort(List<int> floors)
        {
            floors.Sort();
            return floors;
        }
    }
}
