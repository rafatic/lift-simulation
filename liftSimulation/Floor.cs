using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace liftSimulation
{
    public class Floor
    {

        public int Id
        {
            get;
            private set;
        }

        public ConcurrentQueue<Person> PersonsWaiting
        {
            get;
            private set;
        }

        public Dictionary<long, int> QueueSizeHistory
        {
            get;
            private set;
        }

        public double AverageQueueSize;
        

        public int MaximumQueueSize
        {
            get;
            set;
        }

        public double AverageWaitingTime;

        public double GetAverageWaitingTime()
        {
            return AverageQueueSize / QueueSizeHistory.Count;
        }



        public long MaximumWaitingTime
        {
            get;
            set;
        }

        public Floor(int id)
        {
            this.Id = id;
            PersonsWaiting = new ConcurrentQueue<Person>();
            QueueSizeHistory = new Dictionary<long, int>();
            AverageQueueSize = 0.0d;
            AverageWaitingTime = 0.0d;

            MaximumQueueSize = 0;
            MaximumWaitingTime = 0;
        }

        public string QueueHistoryToCsv()
        {
            string csvString = "";

            foreach(KeyValuePair<long, int> elem in QueueSizeHistory)
            {
                csvString +=  Id + ", " + elem.Key + ", " + elem.Value + "\n";
            }
            return csvString;
        }

        private double GetAverageQueueSize()
        {
            AverageQueueSize = 0.0d;

            foreach(KeyValuePair<long, int> elem in QueueSizeHistory)
            {
                AverageQueueSize  += elem.Value;
            }

            AverageQueueSize  /= QueueSizeHistory.Count;
            return AverageQueueSize;
        }


        public string GetResults()
        {
            string str = "";
            str += "[Floor " + Id + "]\n";
            str += "\t Average queue size : " + Math.Round(GetAverageQueueSize(), 2) + "\n";
            str += "\t Maximum queue size : " + MaximumQueueSize + "\n";
            str += "\t Average waiting time : " + Math.Round(GetAverageWaitingTime(), 2) + "\n";
            str += "\t Maximum waiting time : " + MaximumWaitingTime + "\n\n";

            return str;
        }





    }
}
