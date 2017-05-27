using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recrystallization
{
    public class NodesController
    {
        private Node[,] nodesMap;
        private List<Node> noarNodes;
        private int k;
        private double mn;

        private double time;
        private double deltaTime = 0.001;

        static private double A = 86710969050178.5;
        static private double B = 9.41268203527779;

        static private double LimitedRo;
        static private double FirstRo;

        private int w,h;

        private Random random;

        public NodesController(MapController map, int k)
        {
            this.k = k;
            this.mn = (map.w -2)* (map.h-2);
            this.w = map.w;
            this.h = map.h;

            random = new Random();
            noarNodes = new List<Node>();
            nodesMap = new Node[map.w, map.h];
            FirstRo = GetFirtsRo()/mn;


            for (int x=1;x< map.w-1;x++)
            {
                for ( int y = 1; y < map.h-1; y++)
                {
                    Color myColor = map.GetPixelColor(x, y);
                    var neighbors = map.GetPixelNeighbors(x, y);

                    bool central = true;
                    foreach(var point in neighbors)
                    {
                        if(point.color.ToArgb() != myColor.ToArgb())
                        {
                            central = false;
                            break;
                        }
                    }
                    var tmpNode = new Node() { Weight = FirstRo, IsCentralNode = central };
                    nodesMap[x, y] = tmpNode;

                    if (!central)
                        noarNodes.Add(tmpNode);
                }
            }

            time = 0;
            LimitedRo = GetFunValue(0.065)/mn;



        }

        public List<Point> NextStep()
        {
            time += deltaTime;


            double roPerNode = GetDeltaRo(time)/mn;

            double acum = 0;

            for(int x= 1; x < w-1;x++)
            {
                for(int y = 1; y < h-1; y++)
                {
                    if(nodesMap[x, y].IsCentralNode)
                    {
                        nodesMap[x, y].Weight += roPerNode *0.2;
                        acum += roPerNode * 0.8;
                    }
                    else
                    {
                        nodesMap[x, y].Weight += roPerNode * 0.8;
                        acum += roPerNode * 0.2;
                    }

                }
            }
            int kCount = k;
            while(kCount > 0)
            {
                noarNodes[random.Next(noarNodes.Count)].Weight += acum / k;
                kCount--;
            }



            List<Point> result = new List<Point>();
            for (int x = 1; x < w-1; x++)
            {
                for (int y = 1; y < h-1; y++)
                {
                    if(nodesMap[x, y].Weight > LimitedRo)
                    {
                        result.Add(new Point(x, y));
                        nodesMap[x, y].Weight = FirstRo;
                    }
                }
            }

            return result;
        }
        


        private double GetFunValue(double time)
        {
            return (A / B) + (1 - (A / B)) * Math.Exp(-B * time);
        }

        private double GetDeltaRo(double time)
        {
            return GetFunValue(time ) - GetFunValue(time - deltaTime);
        }
        private double GetFirtsRo()
        {
            double first = 1;
            return GetDeltaRo(deltaTime) - first;

        }


    }
}
