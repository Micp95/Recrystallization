using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recrystallization
{
    public class SeedsController
    {
        private List<Seed> seeds;
        private int width, height;

        private Random random;

        public SeedsController()
        {
            random = new Random();
        }

        public List<Seed> GetSeeds()
        {
            return seeds;
        }
        public void GenerateSeeds(int width, int height,GenerateSeedsMethod method,int amount, int[] parameters)
        {
            this.width = width;
            this.height = height;
            this.seedsToAdd = 0;
            seeds = new List<Seed>();

            switch (method)
            {
                case GenerateSeedsMethod.ClickMethod:
                    {
                        GenerateClickMethod(amount);
                    }
                    break;
                case GenerateSeedsMethod.Evenly:
                    {
                        GenerateEvenly(amount, parameters[0], parameters[1]);
                    }
                    break;
                case GenerateSeedsMethod.Random:
                    {
                        GenerateRandom(amount);
                    }
                    break;
                case GenerateSeedsMethod.RandomR:
                    {
                        GenerateRandomR(amount, parameters[0]);
                    }
                    break;
                default:
                    break;
            }
        }

        private Color GenerateColor()
        {
            Color randomColor = Color.FromArgb(random.Next(255), random.Next(255), random.Next(255));
            return randomColor;
           // return Color.r
        }

        private int seedsToAdd;
        private void GenerateClickMethod(int amount)
        {
            seedsToAdd = amount;
        }

        public void AddSeed ( int x, int y)
        {
            if(seedsToAdd > 0)
            {
                seeds.Add(new Seed(GenerateColor(), x, y));
                seedsToAdd--;
            }else if (seedsToAdd == 0)
            {

                seedsToAdd = -1;
            }
        }


        private void GenerateEvenly(int amount, int x, int y)
        {
            int row = 1;
            int column = 1;
            int actX, actY;
            int errorTest = 0;
            while (amount > 0)
            {
                if (errorTest > 1000)
                    throw new Exception("Cos sie zepsulo");
                errorTest++;

                actX = column * x;
                if(actX > width)
                {
                    column = 1;
                    row++;
                    continue;
                }
                actY = row * y;
                if(actY > height)
                {
                    throw new Exception("Cos sie zepsulo");
                }
                column++;
                seeds.Add(new Seed(GenerateColor(), actX, actY));
                errorTest = 0;
                amount--;
            }

        }
        private void GenerateRandom(int amount)
        {
            while(amount > 0)
            {
                int x = random.Next(0, width);
                int y = random.Next(0, height);

                seeds.Add(new Seed(GenerateColor(), x, y));

                amount--;
            }


        }
        private void GenerateRandomR(int amount, int r)
        {
            int x, y;
            while (amount > 0)
            {
                int errorTest = 0;
                do
                {
                    if (errorTest > 10000)
                        throw new Exception("Cos sie zepsulo");
                    errorTest++;

                    x = random.Next(0, width);
                    y = random.Next(0, height);

                } while (!CheckDistances(x, y,r));

                seeds.Add(new Seed(GenerateColor(), x, y));
                amount--;
            }
        }


        private bool CheckDistances(int x, int y, int r)
        {
            foreach(Seed seed in seeds)
            {
                double distance = Math.Sqrt((x - seed.x) * (x - seed.x) + (y - seed.y) * (y - seed.y));
                if (distance < r)
                    return false;
            }
            return true;
        }

    }
}
