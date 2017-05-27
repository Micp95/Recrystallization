using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recrystallization
{
    public class RoConroller
    {
        private List<double> roSteps;

        int counter;

        public bool isEndList { get; set; }
        
        public RoConroller()
        {
            roSteps = new List<double>();
            var file = File.OpenRead("data.txt");

            var fileLines = File.ReadLines("data.txt");
            foreach(var line in fileLines)
            {
                double tmp;
                if(double.TryParse(line, out tmp))
                    roSteps.Add(tmp);
            }    
        }

        public double GetNext()
        {
            if( counter < roSteps.Count)
                return roSteps[counter++];
            return 0;
        }
        public double GetFirst()
        {
            return roSteps[0];
        }

    }
}
