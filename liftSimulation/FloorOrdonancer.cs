using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace liftSimulation
{
    public abstract class FloorOrdonancer
    {
        public enum Heading
        {
            UPWARDS, DOWNWARDS
        }
        /// <summary>
        /// Determines the vertical direction in which the lift is going
        /// 0 = Downwards
        /// 1 = Upwards
        /// </summary>
        public int CurrentHeading
        {
            get;
            set;
        }

        public int CurrentFloor
        {
            get;
            protected set;
        }

        public FloorOrdonancer(int heading, int currentFloor)
        {
            this.CurrentFloor = currentFloor;
            this.CurrentHeading = heading;
        }

        public abstract List<int> Sort(List<int> floors);
    }
}
