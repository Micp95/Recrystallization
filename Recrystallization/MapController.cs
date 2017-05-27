using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recrystallization
{
    public class MapController
    {
        private Bitmap map;
        private Bitmap mapTemp;
        private Color background;
        private Color boardColor;

        public int w, h;

        public BoundaryValue boundaryValue { get; set; }
        public MapController(Color background,Color boardColor)
        {
            this.background = background;
            this.boardColor = boardColor;
            map = null;
        }


        public Bitmap GetMap()
        {
            return map;
        }
        public void CreateNewMap(int w, int h, BoundaryValue boundaryValue)
        {
            this.w = w;
            this.h = h;
            this.boundaryValue = boundaryValue;

            mapTemp = CreateMap(w, h);
            Commit();
        }
        public void Commit()
        {
            if(map != null)
                map.Dispose();
            DrawBoard();
            map = mapTemp;
            mapTemp = CreateMap(w,h);
        }
        private Bitmap CreateMap(int w, int h)
        {
            Bitmap result = new Bitmap(w, h);

            for (int xx = 0; xx < w; xx++)
                for (int yy = 0; yy < h; yy++)
                    result.SetPixel(xx, yy, background);

            return result;
        }

        private void DrawBoard()
        {
            for (int k = 0; k < w; k++)
            {
                mapTemp.SetPixel(k, 0, boardColor);
                mapTemp.SetPixel(k, h - 1, boardColor);
            }

            for (int k = 0; k < h; k++)
            {
                mapTemp.SetPixel(0, k, boardColor);
                mapTemp.SetPixel(w - 1, k, boardColor);
            }

        }
        public void SetPixesl(int x, int y, Color color)
        {
            mapTemp.SetPixel(x, y, color);
        }
        public void SetGeneralPixesl(int x, int y, Color color)
        {
            map.SetPixel(x, y, color);
        }

        public Color GetPixelColor(int x, int y)
        {
            return map.GetPixel(x, y);
        }

        public List<Pixel> GetPixelNeighbors(int x, int y)
        {
            List<Pixel> result = new List<Pixel>();

            for (int xx = x-1; xx<= x + 1; xx++)
            {
                for (int yy = y - 1; yy <= y + 1; yy++)
                {
                    if( xx != x || yy != y)
                    {
                        result.Add(new Pixel() {
                            color = map.GetPixel(xx, yy),
                            x = xx,
                            y = yy
                        });
                    }
                }
            }
            if (boundaryValue == BoundaryValue.Periodic)
                return SpecialFilter(result);
            else
                return result;
        }

        private List<Pixel> SpecialFilter(List<Pixel> pixels)
        {
            foreach(var pixel in pixels)
            {
                if(pixel.x == 1)
                {
                    pixel.x = w - 2;
                    pixel.color = GetPixelColor(pixel.x, pixel.y);
                }
                else if (pixel.x == w-2)
                {
                    pixel.x = 1;
                    pixel.color = GetPixelColor(pixel.x, pixel.y);
                }
                else if (pixel.y == 1)
                {
                    pixel.y = h - 2;
                    pixel.color = GetPixelColor(pixel.x, pixel.y);
                }
                else if (pixel.y == h-2)
                {
                    pixel.y = 1;
                    pixel.color = GetPixelColor(pixel.x, pixel.y);
                }
            }

            return pixels;
        }

    }
}
