using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace liftSimulation
{
    public class LinearScanOrdonancer : FloorOrdonancer
    {
        public LinearScanOrdonancer(int heading, int currentFloor) : base(heading, currentFloor)
        {
        }

        public override List<int> Sort(List<int> floors)
        {
            List<int> upperFloors = new List<int>();
            List<int> lowerFloors = new List<int>();

            if(floors.Any(f => f > CurrentFloor))
            {
                upperFloors = floors.Where(f => f > CurrentFloor).ToList();
            }
            if (floors.Any(f => f <= CurrentFloor))
            {
                lowerFloors = floors.Where(f => f <= CurrentFloor).ToList();
            }

            upperFloors.Sort();
            lowerFloors.Sort(
                new Comparison<int>
                (
                    (i1, i2) => i2.CompareTo(i1)
                )
            );

            if (CurrentHeading == (int)Heading.UPWARDS)
            {
                
                return upperFloors.Union(lowerFloors).ToList();
            }
            else
            {
                return lowerFloors.Union(upperFloors).ToList();
            }
        }
    }
}
