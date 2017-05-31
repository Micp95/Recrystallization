using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recrystallization
{
    public class Simulation
    {
        private MapController mapController { get; set; }
        private MapController mapControllerSec { get; set; }
        private SeedsController seedsController;
        private NeighborhoodMethod method;

        private int width, height;
        private Color background;
        private Color board;
        private Random random;
        private int k;

        private NodesController nodesController;
        public Simulation()
        {
            background = Color.White;
            board = Color.Black;
            mapController = new MapController(background, board);
            seedsController = new SeedsController();
            random = new Random();
            mapControllerSec = new MapController(background, board);
        }

        public void AddSeed(int x, int y)
        {
            seedsController.AddSeed(x, y);

            foreach (var seed in seedsController.GetSeeds())
            {
                mapController.SetPixesl(seed.x, seed.y, seed.myColor);
            }
            mapController.Commit();
        }
        public void StartSimulation(NeighborhoodMethod method,int k)
        {
            this.k = k;
            this.method = method;
            status = SimulationStatus.Growthing;
        }
        public void GenerateSeeds(int width, int height, GenerateSeedsMethod method, BoundaryValue boundaryValue, int amount, int[] parameters)
        {
            this.width = (width += 2);
            this.height= (height  += 2);

            mapController.CreateNewMap(width, height, boundaryValue);
            mapControllerSec.CreateNewMap(width, height, boundaryValue);

            seedsController.GenerateSeeds(width, height, method, amount, parameters);

            foreach(var seed in seedsController.GetSeeds())
            {
                mapController.SetPixesl(seed.x, seed.y, seed.myColor);
            }
            mapController.Commit();
        }

        private SimulationStatus status;
        public bool NextStep()
        {
            switch (status)
            {
                case SimulationStatus.Growthing:
                    status =Growth();
                    break;
                case SimulationStatus.EndGrowth:
                    status =StartRecrystallization();
                    return false;
                case SimulationStatus.Recrystallizationing:
                    status =Recrystallization();
                    break;
                case SimulationStatus.MC:
                    MCIteration();
                    break;
                case SimulationStatus.EndMC:
                case SimulationStatus.EndRecrystallization:
                    return false;
            }

            return true;

        }
        private List<Color> seedColorsMC;
        public void StartMC(int seeds, int width, int height)
        {
            this.width = (width += 2);
            this.height = (height += 2);
            mapController.CreateNewMap(width, height,BoundaryValue.Periodic);

            seedColorsMC = new List<Color>();
            while (seeds-- > 0) { seedColorsMC.Add(GenerateColor());  }
            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    mapController.SetPixesl(x, y, seedColorsMC[random.Next(seedColorsMC.Count)]);
                }
            }
            mapController.Commit();
            status = SimulationStatus.MC;
        }

        private void MCIteration()
        {


            int pozX = random.Next(1,width-1);
            int pozY = random.Next(1,height-1);

            Color actColor = mapController.GetPixelColor(pozX, pozY);
            int actEnergy = GetMCEnergy(actColor,pozX, pozY);

            Color newColor = actColor;
            do
            {
                newColor = seedColorsMC[random.Next(seedColorsMC.Count)];
            } while (actColor.ToArgb() == newColor.ToArgb());

            int newEnergy = GetMCEnergy(newColor, pozX, pozY);
            if (newEnergy<= actEnergy)
            {
                mapController.SetGeneralPixesl(pozX, pozY, newColor);
            }
           // mapController.Commit();
        }

       

        private int GetMCEnergy(Color actColor,int pozX, int pozY)
        {
            var neighbors = mapController.GetPixelNeighbors(pozX, pozY);
            int energy = 0;
            foreach(var pixel in neighbors)
            {
                if (actColor.ToArgb() != pixel.color.ToArgb())
                    energy++;
            }
            return energy;
        }

        private Color GenerateColor()
        {
            return Color.FromArgb(random.Next(255), random.Next(255), random.Next(255));
        }

        private SimulationStatus Growth()
        {
            for ( int x = 1; x <width-1;x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    DrowColors(mapController,x, y);
                 //   mapController.SetPixesl(x, y, DrowColors(x, y));

                }
            }
            mapController.Commit();


            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    if (mapController.GetPixelColor(x, y).ToArgb() == background.ToArgb())
                        return SimulationStatus.Growthing;
                }
            }
            return SimulationStatus.EndGrowth;
        }





        private SimulationStatus StartRecrystallization()
        {
            Node.backround = background;
            nodesController = new NodesController(mapController,k);

            return SimulationStatus.Recrystallizationing;

        }
        private SimulationStatus Recrystallization()
        {

						//rozrost ziaren
            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    DrowColors(mapControllerSec, x, y);
                }
            }

						//generowanie nowych ziaren
            var points = nodesController.NextStep();
            if(points.Count != 0)
            {
                foreach (var point in points)
                {
                    var color = GenerateColor();
                    if(mapControllerSec.GetPixelColor(point.X, point.Y).ToArgb() == background.ToArgb())
                        mapControllerSec.SetPixesl(point.X, point.Y, color);
                    var neighbors = mapControllerSec.GetPixelNeighbors(point.X, point.Y);
                    foreach(var p in neighbors)
                    {
                        if (mapControllerSec.GetPixelColor(p.x, p.y).ToArgb() == background.ToArgb())
                            mapControllerSec.SetPixesl(p.x, p.y, color);
                    }
                }
            }
            mapControllerSec.Commit();


            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    if (mapControllerSec.GetPixelColor(x, y).ToArgb() == background.ToArgb())
                        return SimulationStatus.Recrystallizationing;
                }
            }
            return SimulationStatus.EndRecrystallization;
        }



        public Bitmap GetMap()
        {
            if(status == SimulationStatus.EndGrowth || status == SimulationStatus.Growthing ||status == SimulationStatus.MC)
                return mapController.GetMap();
            else
            {
                Bitmap result = new Bitmap(mapControllerSec.GetMap());
                Bitmap back = mapController.GetMap();
                for (int x = 1; x < width - 1; x++)
                {
                    for (int y = 1; y < height - 1; y++)
                    {
                        if (result.GetPixel(x, y).ToArgb() == background.ToArgb())
                        {
                            result.SetPixel(x, y, back.GetPixel(x, y));
                        }
                    }
                }

                return result;
            }
        }






        private void DrowColors (MapController mapController,  int x, int y)
        {
            Color myColor = mapController.GetPixelColor(x, y);

            if (myColor.ToArgb() == background.ToArgb())
            {
                return;
            }
            mapController.SetPixesl(x, y, myColor);

            List<Pixel> nodes = GetNeighborhoodFromMethod(mapController,x, y);
            foreach( var pixel in nodes)
            {
                SetPixel(mapController, pixel, myColor);
            }

        }

        private List<Pixel> GetNeighborhoodFromMethod(MapController mapController,int x, int y)
        {
            List<Pixel> pixels =null;
            switch (method)
            {
                case NeighborhoodMethod.HexagonalLeft:
                    pixels = GetColorHexagonalLeft(mapController, x, y);
                    break;
                case NeighborhoodMethod.HexagonalRandom:
                    pixels = GetColorHexagonalRandom(mapController, x, y);
                    break;
                case NeighborhoodMethod.HexagonalRight:
                    pixels = GetColorHexagonalRight(mapController, x, y);
                    break;
                case NeighborhoodMethod.Moore:
                    pixels = GetColorMoore(mapController, x, y);
                    break;
                case NeighborhoodMethod.PentagonalRandom:
                    pixels = GetColorPentagonalRandom(mapController, x, y);
                    break;
                case NeighborhoodMethod.VonNeumann:
                    pixels = GetColorVonNeumann(mapController, x, y);
                    break;
                default:
                    break;

            }

            return pixels;
        }




        private List<Pixel> GetColorHexagonalLeft(MapController mapController, int x, int y)
        {
            List<Pixel> neighbors = mapController.GetPixelNeighbors(x, y);
            List<Pixel> result = new List<Pixel>();


            result.Add(neighbors[0]);
            result.Add(neighbors[1]);
            result.Add(neighbors[3]);
            result.Add(neighbors[4]);
            result.Add(neighbors[6]);
            result.Add(neighbors[7]);
            return result;

        }
        private List<Pixel> GetColorHexagonalRight(MapController mapController,int x, int y)
        {
            List<Pixel> neighbors = mapController.GetPixelNeighbors(x, y);
            List<Pixel> result = new List<Pixel>();

            result.Add(neighbors[1]);
            result.Add(neighbors[2]);
            result.Add(neighbors[3]);
            result.Add(neighbors[4]);
            result.Add(neighbors[5]);
            result.Add(neighbors[6]);

            return result;

        }
        private List<Pixel> GetColorHexagonalRandom(MapController mapController,int x, int y)
        {
           // List<Pixel> neighbors = mapController.GetPixelNeighbors(x, y);

            int r = random.Next(100);
            if( r < 50)
            {
                return GetColorHexagonalLeft(mapController, x, y);

            }
            else
            {
                return GetColorHexagonalRight(mapController, x, y);
            }


        }
        private List<Pixel> GetColorMoore(MapController mapController,int x, int y)
        {
            return mapController.GetPixelNeighbors(x, y);
        }
        private List<Pixel> GetColorPentagonalRandom(MapController mapController,int x, int y)
        {
            List<Pixel> neighbors = mapController.GetPixelNeighbors(x, y);
            List<Pixel> result = new List<Pixel>();

            int r = random.Next(100);
            if (r < 25)
            {
                result.Add(neighbors[0]);
                result.Add(neighbors[1]);
                result.Add(neighbors[3]);
                result.Add(neighbors[5]);
                result.Add(neighbors[6]);

            }
            else if (r <50)
            {
                result.Add(neighbors[1]);
                result.Add(neighbors[2]);
                result.Add(neighbors[4]);
                result.Add(neighbors[6]);
                result.Add(neighbors[7]);
            }
            else if (r < 75)
            {
                result.Add(neighbors[3]);
                result.Add(neighbors[4]);
                result.Add(neighbors[5]);
                result.Add(neighbors[6]);
                result.Add(neighbors[7]);
            }
            else
            {
                result.Add(neighbors[0]);
                result.Add(neighbors[1]);
                result.Add(neighbors[2]);
                result.Add(neighbors[3]);
                result.Add(neighbors[4]);
            }
            return result;
        }
        private List<Pixel> GetColorVonNeumann(MapController mapController,int x, int y)
        {
            List<Pixel> neighbors = mapController.GetPixelNeighbors(x, y);
            List<Pixel> result = new List<Pixel>();

            result.Add(neighbors[1]);
            result.Add(neighbors[3]);
            result.Add(neighbors[4]);
            result.Add(neighbors[6]);
            return result;

        }
        private void SetPixel (MapController mapController, Pixel pixel, Color myColor)
        {
            if (pixel.color.ToArgb() == background.ToArgb())
                mapController.SetPixesl(pixel.x, pixel.y, myColor);
        }



    }
}
