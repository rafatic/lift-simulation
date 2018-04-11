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


        /*public Dictionary<Queue<Person>, int> ProcessingTimeRequiredByJobQueue
        {
            get;
            private set;
        }*/

        public Person(int id, int departure, int destination)
        {
            this.Id = id;
            this.Departure = departure;
            this.Destination = destination;

            //ProcessingTimeRequiredByJobQueue = new Dictionary<Queue<Person>, int>();
        }

        /*public bool RequiresMoreWork
        {
            get
            {
                return ProcessingTimeRequiredByJobQueue.Count > 0;
            }
        }*/

        
    }
}
