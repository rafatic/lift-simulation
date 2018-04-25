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
            set;
        }

        public long TimeBeforeGoingActive
        {
            get;
            set;
        }

        public int Destination
        {
            get;
            set;
        }
        
        public long BeginQueueTimeGoingUp
        {
            get;
            set;
        }

        public long BeginQueueTimeGoingDown
        {
            get;
            set;
        }

        public long EnteringLiftTimeGoingUp
        {
            get;
            set;
        }

        public long EnteringLiftTimeGoingDown
        {
            get;
            set;
        }

        public long ExitingLiftTimeGoingUp
        {
            get;
            set;
        }

        public long ExitingLiftTimeGoingDown
        {
            get;
            set;
        }


        

        public long TotalQueueTime
        {
            get;
            set;
        }

        public long TotalTimeInLift
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

        public Person(int id, int departure, int destination, long beginQueueTime, long timeBeforeGoingActive)
        {
            this.Id = id;
            this.Departure = departure;
            this.Destination = destination;
            this.BeginQueueTimeGoingUp = beginQueueTime;
            this.TimeBeforeGoingActive = timeBeforeGoingActive;
            
        }

        public override string ToString()
        {
            string str = "";
            str += Id + "\t" + BeginQueueTimeGoingUp + "\t\t" + (EnteringLiftTimeGoingUp - BeginQueueTimeGoingUp) + "\t\t " + (ExitingLiftTimeGoingUp - EnteringLiftTimeGoingUp) + " \t\t|  ";
            str += BeginQueueTimeGoingDown + "\t\t " + (EnteringLiftTimeGoingDown - BeginQueueTimeGoingDown) + "\t\t" + (ExitingLiftTimeGoingDown - EnteringLiftTimeGoingDown) + "\n";

            return str;
        }

        

        
    }
}
