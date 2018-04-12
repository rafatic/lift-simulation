using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace liftSimulation
{
    public class Person
    {

        public int Id
        {
            get;
            private set;
        }

        public int Departure
        {
            get;
            private set;
        }

        public int Destination
        {
            get;
            private set;
        }
        
        public long BeginQueueTime
        {
            get;
            set;
        }

        public long EnteringLiftTime
        {
            get;
            set;
        }

        public long ExitingLiftTime
        {
            get;
            set;
        }


        public Person(int id, int departure, int destination)
        {
            this.Id = id;
            this.Departure = departure;
            this.Destination = destination;
        }

        public Person(int id, int departure, int destination, long beginQueueTime)
        {
            this.Id = id;
            this.Departure = departure;
            this.Destination = destination;
            this.BeginQueueTime = beginQueueTime;
        }

        
    }
}
