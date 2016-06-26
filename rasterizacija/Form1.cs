using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

//author: NEJC GALOF; galof.nejc@gmail.com
namespace rasterization
{
    public partial class Form1 : Form
    {
        Point start_point;
        Point end_point;
        int zoom = 4;
        //1 square: 10x10 pixel
        public Form1()
        {
            InitializeComponent();
        }
        class ColorPoint
        {
            int x;
            int y;
            int color;

            public ColorPoint(int x, int y,int color)
            {
                this.x = x;
                this.y = y;
                this.color = color;
            }
            public int get_x() {
                return x;
            }
            public int get_y()
            {
                return y;
            }
            public int get_color()
            {
                return color;
            }
        };
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            zoom = Convert.ToInt16(textBox1.Text);
            if (start_point != null & end_point != null)
            {
                if (radioButton1.Checked)//Bresenha algorithm for lines
                {
                    //License:  http://wiki.fmf.uni-lj.si/wiki/Bresenhamov_algoritem
                    //     http://www2.nauk.si/materials/538/out-463719/index.html#state=14
                    //http://en.wikipedia.org/wiki/Bresenham's_line_algorithm  - interger implementiran
                    //Rise from start to end for every move
                    //Mistke with int not with double
                    Point point1 = new Point(start_point.X, start_point.Y);
                    Point point2 = new Point(end_point.X, end_point.Y);
                    //for 360 works:
                    bool rotation = Math.Abs(point2.Y - point1.Y) > Math.Abs(point2.X - point1.X);
                    if (rotation == true)
                    {
                        //turn rotate of line - change x and y
                        Point tmp = new Point(point1.X, point1.Y);
                        point1 = new Point(tmp.Y, tmp.X);

                        tmp = point2;
                        point2 = new Point(tmp.Y, tmp.X);
                    }
                    int deltaX = Math.Abs(point2.X - point1.X);
                    int deltaY = Math.Abs(point2.Y - point1.Y);
                    int mistake = 0;
                    int deltaMistake = deltaY;
                    int stepY = 0;
                    int stepX = 0;
                    int y = point1.Y;
                    int x = point1.X;

                    //Determines the direction of movement Y
                    if (point1.Y < point2.Y)
                    {
                        stepY = 1;
                    }
                    else
                    {
                        stepY = -1;
                    }

                    //Determines the direction of movement X
                    if (point1.X < point2.X)
                    {
                        stepX = 1;
                    }
                    else
                    {
                        stepX = -1;
                    }

                    int tmpX = 0;
                    int tmpY = 0;
                    while (x != point2.X) //moving on x coordinate
                    {
                        x += stepX;
                        mistake += deltaMistake;
                        if ((2 * mistake) > deltaX) //mistake - moving y coordinate
                        {
                            y += stepY;
                            mistake -= deltaX;
                        }
                        //rotate coordinate if I change earlier
                        if (rotation==true)
                        {
                            tmpX = y;
                            tmpY = x;
                        }
                        else
                        {
                            tmpX = x;
                            tmpY = y;
                        }
                        e.Graphics.FillRectangle(new SolidBrush(Color.Black), new Rectangle(tmpX * zoom, tmpY * zoom, 4, 4));
                    }
                }//end of Bresenham alghoritem for  lines
                else if (radioButton2.Checked)//Xiaolin line
                {
                    //Source: http://rosettacode.org/wiki/Xiaolin_Wu's_line_algorithm
                    Point point1 = new Point(start_point.X, start_point.Y);
                    Point point2 = new Point(end_point.X, end_point.Y);

                    List<ColorPoint> points = new List<ColorPoint>();//lista tock z alpha vrednostjo
                    bool direction = Math.Abs(point2.Y - point1.Y) > Math.Abs(point2.X - point1.X);
                    if (direction)//replace x and y
                    {
                        Point tmp = new Point(point1.X, point1.Y);
                        Point tmp2 = new Point(point2.X, point2.Y);
                        point1 = new Point(tmp.Y, tmp.X);
                        point2 = new Point(tmp2.Y, tmp2.X);
                    }
                    if (point1.X > point2.X)//replace points
                    {
                        Point tmp = new Point(point1.X, point1.Y);
                        Point tmp2 = new Point(point2.X, point2.Y);
                        point1 = new Point(tmp2.X, tmp2.Y);
                        point2 = new Point(tmp.X, tmp.Y);
                    }
                    double dx = point2.X - point1.X;
                    double dy = point2.Y - point1.Y;
                    double gradient = (double)((double)(dy) / (double)(dx));
                    //first point
                    int endX = round(point1.X);
                    double endY = point1.Y + gradient * (endX - point1.X);
                    double gapX = rfPart(point1.X + 0.5);
                    int pxlX_1 = endX; 
                    int pxlY_1 = iPart(endY);
                    if (direction)
                    {
                        Point a = new Point(pxlY_1, pxlX_1);
                        ColorPoint b = new ColorPoint(pxlY_1, pxlX_1,(int)(rfPart(endY) * gapX * 255));
                        points.Add(b);
                        b = new ColorPoint(pxlY_1 + 1, pxlX_1, (int)(fPart(endY) * gapX * 255));
                        points.Add(b);
                    }
                    else
                    {
                        Point a = new Point(pxlX_1, pxlY_1);
                        ColorPoint b = new ColorPoint(pxlX_1, pxlY_1, (int)(rfPart(endY) * gapX * 255));
                        points.Add(b);
                        b = new ColorPoint(pxlX_1, pxlY_1 + 1, (int)(fPart(endY) * gapX * 255));
                        points.Add(b);
                    }
                    double intery = endY + gradient;

                    //second point
                    endX = round(point2.X);
                    endY = point2.Y + gradient * (endX - point2.X);
                    gapX = fPart(point2.X + 0.5);
                    int pxlX_2 = endX;  
                    int pxlY_2 = iPart(endY);
                    if (direction)
                    {
                        Point a = new Point(pxlY_2, pxlX_2);
                        ColorPoint b = new ColorPoint(pxlY_2, pxlX_2, (int)(rfPart(endY) * gapX * 255));
                        points.Add(b);
                        b = new ColorPoint(pxlY_2 + 1, pxlX_2, (int)(fPart(endY) * gapX * 255));
                        points.Add(b);
                    }
                    else
                    {
                        Point a = new Point(pxlX_2, pxlY_2);
                        ColorPoint b = new ColorPoint(pxlX_2, pxlY_2, (int)(rfPart(endY) * gapX * 255));
                        points.Add(b);
                        b = new ColorPoint(pxlX_2, pxlY_2 + 1, (int)(fPart(endY) * gapX * 255));
                        points.Add(b);
                    }

                    //loop from all points
                    for (double x = (pxlX_1 + 1); x <= (pxlX_2 - 1); x++)
                    {
                        if (direction)
                        {
                            Point a = new Point(iPart(intery), (int)x);
                            ColorPoint b = new ColorPoint(iPart(intery), (int)x, (int)(rfPart(intery) * 255));
                            points.Add(b);
                            b = new ColorPoint((iPart(intery) + 1), (int)x, (int)(fPart(intery) * 255));
                            points.Add(b);
                        }
                        else
                        {
                            Point a = new Point((int)x, iPart(intery));
                            ColorPoint b = new ColorPoint((int)x, iPart(intery), (int)(rfPart(intery) * 255));
                            points.Add(b);
                            b = new ColorPoint((int)x, iPart(intery) + 1, (int)(fPart(intery) * 255));
                            points.Add(b);
                        }
                        intery += gradient;
                    }
                    Color c1 = Color.Black;
                    foreach (ColorPoint a in points) //draw each pixel
                    {
                        //take care for extremes - if mouse outside from picture-box
                        int shade = 0;
                        if (a.get_color() < 0) {
                            shade = 0;
                        }
                        else if (a.get_color() > 255)
                        {
                            shade = 255;
                        }
                        else {
                            shade = a.get_color();
                        }
                        Color c2 = Color.FromArgb(shade,c1.R, c1.G, c1.B);
                        e.Graphics.FillRectangle(new SolidBrush(c2), new Rectangle(a.get_x()*zoom , a.get_y() *zoom, 4, 4));
                    }
                }//end of Xiaolin line
                else if (radioButton3.Checked)//Xiaolinov alghoritem for rasterization circle
                {
                    int offset_x = start_point.X;
                    int offset_y = start_point.Y;
                    Color color = Color.Black;
                    int r = (int)Math.Sqrt(Math.Pow(start_point.X - end_point.X, 2) + Math.Pow(start_point.Y - end_point.Y, 2));//polmer
                    int x = r;
                    int y = -1;
                    double t = 0;
                    while (x-1 > y)
                    {
                        y++;
                        double current_distance = distance(r, y);
                        if (current_distance < t)
                        {
                            x--;
                        }
                        //shades
                        int transparency = new_color(current_distance);
                        int alpha = transparency;
                        int alpha2 = 127 - transparency;
                        Color c2 = Color.FromArgb(255, color.R, color.G, color.B);
                        Color c3 = Color.FromArgb(alpha2, color.R, color.G, color.B);
                        Color c4 = Color.FromArgb(alpha, color.R, color.G, color.B);

                        e.Graphics.FillRectangle(new SolidBrush(c2), new Rectangle((x + offset_x)*zoom,( y + offset_y)*zoom, 4, 4));
                        e.Graphics.FillRectangle(new SolidBrush(c4), new Rectangle((x + offset_x - 1)*zoom, (y + offset_y)*zoom, 4, 4));
                        e.Graphics.FillRectangle(new SolidBrush(c3), new Rectangle((x + offset_x + 1)*zoom, (y + offset_y)*zoom, 4, 4));

                        e.Graphics.FillRectangle(new SolidBrush(c2), new Rectangle((y + offset_x)*zoom, (x + offset_y)*zoom, 4, 4));
                        e.Graphics.FillRectangle(new SolidBrush(c4), new Rectangle((y + offset_x)*zoom, (x + offset_y - 1)*zoom, 4, 4));
                        e.Graphics.FillRectangle(new SolidBrush(c3), new Rectangle((y + offset_x)*zoom, (x + offset_y + 1)*zoom, 4, 4));

                        e.Graphics.FillRectangle(new SolidBrush(c2), new Rectangle((offset_x - x)*zoom, (y + offset_y)*zoom, 4, 4));
                        e.Graphics.FillRectangle(new SolidBrush(c4), new Rectangle((offset_x - x + 1)*zoom, (y + offset_y)*zoom, 4, 4));
                        e.Graphics.FillRectangle(new SolidBrush(c3), new Rectangle((offset_x - x - 1)*zoom, (y + offset_y)*zoom, 4, 4));

                        e.Graphics.FillRectangle(new SolidBrush(c2), new Rectangle((offset_x - y)*zoom, (x + offset_y)*zoom, 4, 4));
                        e.Graphics.FillRectangle(new SolidBrush(c4), new Rectangle((offset_x - y)*zoom, (x + offset_y - 1)*zoom, 4, 4));
                        e.Graphics.FillRectangle(new SolidBrush(c3), new Rectangle((offset_x - y)*zoom, (x + offset_y + 1)*zoom, 4, 4));

                        e.Graphics.FillRectangle(new SolidBrush(c2), new Rectangle((x + offset_x)*zoom, (offset_y - y)*zoom, 4, 4));
                        e.Graphics.FillRectangle(new SolidBrush(c4), new Rectangle((x + offset_x - 1)*zoom, (offset_y - y)*zoom, 4, 4));
                        e.Graphics.FillRectangle(new SolidBrush(c3), new Rectangle((x + offset_x + 1)*zoom, (offset_y - y)*zoom, 4, 4));

                        //UP
                        e.Graphics.FillRectangle(new SolidBrush(c2), new Rectangle((y + offset_x)*zoom, (offset_y - x)*zoom, 4, 4));
                        e.Graphics.FillRectangle(new SolidBrush(c3), new Rectangle((y + offset_x)*zoom, (offset_y - x - 1)*zoom, 4, 4));
                        e.Graphics.FillRectangle(new SolidBrush(c4), new Rectangle((y + offset_x)*zoom, (offset_y - x + 1)*zoom, 4, 4));

                        e.Graphics.FillRectangle(new SolidBrush(c2), new Rectangle((offset_x - y)*zoom, (offset_y - x)*zoom, 4, 4));
                        e.Graphics.FillRectangle(new SolidBrush(c3), new Rectangle((offset_x - y)*zoom, (offset_y - x - 1)*zoom, 4, 4));
                        e.Graphics.FillRectangle(new SolidBrush(c4), new Rectangle((offset_x - y)*zoom, (offset_y - x + 1)*zoom, 4, 4));

                        e.Graphics.FillRectangle(new SolidBrush(c2), new Rectangle((offset_x - x)*zoom, (offset_y - y)*zoom, 4, 4));
                        e.Graphics.FillRectangle(new SolidBrush(c3), new Rectangle((offset_x - x - 1)*zoom, (offset_y - y)*zoom, 4, 4));
                        e.Graphics.FillRectangle(new SolidBrush(c4), new Rectangle((offset_x - x + 1)*zoom, (offset_y - y)*zoom, 4, 4));
                        t = current_distance;
                    }
                }
            }
        }
        //functions for circle
        double distance(int x, int y){
            double real_point=Math.Sqrt(Math.Pow(x,2)-Math.Pow(y,2));
            return (Math.Ceiling(real_point)-real_point);
        }
        int new_color(double i){
            return (int)Math.Round((i * 127));
        }

        //functions for calculating
        int iPart(double d)
        {
            return (int)d;
        }

        int round(double d)
        {
            return (int)(d + 0.50000);
        }

        double fPart(double d)
        {
            return (double)(d - (int)(d));
        }

        double rfPart(double d)
        {
            return (double)(1.00000 - (double)(d - (int)(d)));
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            zoom = Convert.ToInt16(textBox1.Text);
            //get new start_point
            if (e.Button == MouseButtons.Left)
            {
                start_point = new Point(e.X / zoom, e.Y / zoom);
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                end_point = new Point(e.X / zoom, e.Y / zoom);
                pictureBox1.Invalidate();
            }
           
        }

    }
}
