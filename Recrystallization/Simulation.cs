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
        private MapController mapController;
        private SeedsController seedsController;
        private NeighborhoodMethod method;

        private int width, height;
        private Color background;
        private Random random;
        public Simulation()
        {
            background = Color.White;
            mapController = new MapController(background, Color.Black);
            seedsController = new SeedsController();
            random = new Random();
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
        public void StartSimulation(NeighborhoodMethod method)
        {
            this.method = method;
        }
        public void GenerateSeeds(int width, int height, GenerateSeedsMethod method, BoundaryValue boundaryValue, int amount, int[] parameters)
        {
            this.width = (width += 2);
            this.height= (height  += 2);

            mapController.CreateNewMap(width, height, boundaryValue);
            seedsController.GenerateSeeds(width, height, method, amount, parameters);

            foreach(var seed in seedsController.GetSeeds())
            {
                mapController.SetPixesl(seed.x, seed.y, seed.myColor);
            }
            mapController.Commit();
        }

        public void NextStep()
        {
            for ( int x = 1; x <width-1;x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    DrowColors(x, y);
                 //   mapController.SetPixesl(x, y, DrowColors(x, y));

                }
            }
            mapController.Commit();
        }

        public Bitmap GetMap()
        {
            return mapController.GetMap();
        }






        private void DrowColors ( int x, int y)
        {
            Color myColor = mapController.GetPixelColor(x, y);

            if (myColor.ToArgb() == background.ToArgb())
            {
                return;
            }
            mapController.SetPixesl(x, y, myColor);

            switch (method)
            {
                case NeighborhoodMethod.HexagonalLeft:
                    GetColorHexagonalLeft(myColor,x, y);
                    break;
                case NeighborhoodMethod.HexagonalRandom:
                    GetColorHexagonalRandom(myColor,x, y);
                    break;
                case NeighborhoodMethod.HexagonalRight:
                    GetColorHexagonalRight(myColor,x, y);
                    break;
                case NeighborhoodMethod.Moore:
                    GetColorMoore(myColor,x, y);
                    break;
                case NeighborhoodMethod.PentagonalRandom:
                    GetColorPentagonalRandom(myColor,x, y);
                    break;
                case NeighborhoodMethod.VonNeumann:
                    GetColorVonNeumann(myColor,x, y);
                    break;
                default:
                    break;
                    
            }


        }
        private void GetColorHexagonalLeft(Color myColor, int x, int y)
        {
            List<Pixel> neighbors = mapController.GetPixelNeighbors(x, y);


            SetPixel(neighbors[0], myColor);
            SetPixel(neighbors[1], myColor);
            SetPixel(neighbors[3], myColor);
            SetPixel(neighbors[4], myColor);
            SetPixel(neighbors[6], myColor);
            SetPixel(neighbors[7], myColor);
        }
        private void GetColorHexagonalRight(Color myColor,int x, int y)
        {
            List<Pixel> neighbors = mapController.GetPixelNeighbors(x, y);

            SetPixel(neighbors[1], myColor);
            SetPixel(neighbors[2], myColor);
            SetPixel(neighbors[3], myColor);
            SetPixel(neighbors[4], myColor);
            SetPixel(neighbors[5], myColor);
            SetPixel(neighbors[6], myColor);

        }
        private void GetColorHexagonalRandom(Color myColor,int x, int y)
        {
            List<Pixel> neighbors = mapController.GetPixelNeighbors(x, y);

            int r = random.Next(100);
            if( r < 50)
            {
                GetColorHexagonalLeft(myColor, x, y);

            }
            else
            {
                GetColorHexagonalRight(myColor, x, y);
            }


        }
        private void GetColorMoore(Color myColor,int x, int y)
        {
            List<Pixel> neighbors = mapController.GetPixelNeighbors(x, y);
            foreach ( Pixel pixel in neighbors)
            {
                SetPixel(pixel, myColor);
            }


        }
        private void GetColorPentagonalRandom(Color myColor,int x, int y)
        {
            List<Pixel> neighbors = mapController.GetPixelNeighbors(x, y);

            int r = random.Next(100);
            if (r < 25)
            {
                SetPixel(neighbors[0], myColor);
                SetPixel(neighbors[1], myColor);
                SetPixel(neighbors[3], myColor);
                SetPixel(neighbors[5], myColor);
                SetPixel(neighbors[6], myColor);

            }
            else if (r <50)
            {
                SetPixel(neighbors[1], myColor);
                SetPixel(neighbors[2], myColor);
                SetPixel(neighbors[4], myColor);
                SetPixel(neighbors[6], myColor);
                SetPixel(neighbors[7], myColor);
            }
            else if (r < 75)
            {
                SetPixel(neighbors[3], myColor);
                SetPixel(neighbors[4], myColor);
                SetPixel(neighbors[5], myColor);
                SetPixel(neighbors[6], myColor);
                SetPixel(neighbors[7], myColor);
            }
            else
            {
                SetPixel(neighbors[0], myColor);
                SetPixel(neighbors[1], myColor);
                SetPixel(neighbors[2], myColor);
                SetPixel(neighbors[3], myColor);
                SetPixel(neighbors[4], myColor);
            }
        }
        private void GetColorVonNeumann(Color myColor,int x, int y)
        {
            List<Pixel> neighbors = mapController.GetPixelNeighbors(x, y);

            SetPixel(neighbors[1], myColor);
            SetPixel(neighbors[3], myColor);
            SetPixel(neighbors[4], myColor);
            SetPixel(neighbors[6], myColor);

        }
        private void SetPixel ( Pixel pixel, Color myColor)
        {
            if (pixel.color.ToArgb() == background.ToArgb())
                mapController.SetPixesl(pixel.x, pixel.y, myColor);
        }



    }
}
