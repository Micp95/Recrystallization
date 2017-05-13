using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recrystallization
{
    public class Seed
    {
        public Color myColor;
        public int x, y;

        public Seed(Color myColor, int x, int y)
        {
            this.myColor = myColor;
            this.x = x;
            this.y = y;
        }


    }
}
