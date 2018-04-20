using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace liftSimulation
{
    class ShorterSeekTimeFirstOrdonancer : FloorOrdonancer
    {

        


        public ShorterSeekTimeFirstOrdonancer(int heading, int currentFloor) : base(heading, currentFloor)
        {
            //this.CurrentHeading = (int)heading;
            //this.CurrentFloor = currentFloor;
        }

        private int Compare(int x, int y, int pivot)
        {
            if (x == y)
            {
                CurrentFloor = x;
                return 0;
            }
            if (Math.Abs(x - pivot) > Math.Abs(y - pivot))
            {
                if (y > pivot)
                {
                    CurrentHeading = (int)Heading.UPWARDS;
                }
                else
                {
                    CurrentHeading = (int)Heading.DOWNWARDS;
                }
                CurrentFloor = y;

                return -1;
            }
            else if (Math.Abs(x - pivot) < Math.Abs(y - pivot))
            {
                if (x > pivot)
                {
                    CurrentHeading = (int)Heading.UPWARDS;
                }
                else
                {
                    CurrentHeading = (int)Heading.DOWNWARDS;
                }

                CurrentFloor = x;
                return 1;
            }

            if (Math.Abs(x - pivot) == Math.Abs(y - pivot))
            {
                if (x > pivot)
                {
                    if (CurrentHeading == (int)Heading.UPWARDS)
                    {
                        CurrentFloor = x;
                        return 1;
                    }
                    else
                    {
                        CurrentHeading = (int)Heading.DOWNWARDS;
                        CurrentFloor = y;
                        return -1;
                    }

                }
                else if(y > pivot)
                {
                    if (CurrentHeading == (int)Heading.UPWARDS)
                    {
                        CurrentFloor = y;
                        return -1;
                    }
                    else
                    {
                        CurrentHeading = (int)Heading.DOWNWARDS;
                        CurrentFloor = x;
                        return 1;
                    }
                }
            }
            return 0;
        }

        private int getClosestFloor(List<int> floors, int pivot)
        {
            int minFloor = floors[0];

            foreach(int f in floors)
            {
                if(Compare(minFloor, f, pivot) == -1)
                {
                    minFloor = f;
                }
            }

            return minFloor;
        }

        public override List<int> Sort(List<int> floors)
        {
            List<int> sortedFloors = new List<int>();
            int nbFloors = floors.Count, closestFloor, pivot = CurrentFloor;
            while(sortedFloors.Count < nbFloors)
            {
                
                closestFloor = getClosestFloor(floors, pivot);
                sortedFloors.Add(closestFloor);
                floors.Remove(closestFloor);
                pivot = closestFloor;
            }

            return sortedFloors;


        }
    }
}
