using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        Bitmap bmp;//определение объекта для будущего создания области для рисования
        Point point1;//точка при зажатой клавише мыши (начало прямоугольника)
        Point point2;//точка при отжатии клавиши мыши (конец прямоугольника)
        bool flag;//флаг существования рисунка
        Graphics g; //объект графики (нужен потом для очищения рисунка)
        public Form1()
        {
            InitializeComponent();
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);//установка размеров поля для рисования 
            flag = false;//рисунка нет

        }

        private void PutPixel(Color color, int x, int y) //функция, которая будет рисовать пиксель
        {
            if (x > 0 && x < bmp.Width && y > 0 && y < bmp.Height) bmp.SetPixel(x, y, color);//рисует пиксель, если он не выходит
                                                                                            //за границы области рисования
        }

        public void Bresenham4Line(Color clr, int x0, int y0,int x1, int y1)//функция рисования линии
        {
            //Изменения координат
            int dx = (x1 > x0) ? (x1 - x0) : (x0 - x1); //длина линии по x
            int dy = (y1 > y0) ? (y1 - y0) : (y0 - y1);//длина линии по y
            //Направление приращения
            int sx = (x1 >= x0) ? (1) : (-1);//будем двигаться вправо или влево в зависимости от направления линии
            int sy = (y1 >= y0) ? (1) : (-1);//будем двигаться вниз или вверх в зависимости от направления линии

            if (dy < dx) //если угол между линией и горизонталью меньше 45 градусов
            {
                int d = (dy << 1) - dx;
                int d1 = dy << 1;
                int d2 = (dy - dx) << 1;
                PutPixel(clr, x0, y0);//рисование пикселя
                int x = x0 + sx;
                int y = y0;
                for (int i = 1; i <= dx; i++)//двигаемся по горизонтали
                {
                    if (d > 0)//если точка между пикселями ближе к следующему по вертикали, чем к текущему, изменяем точку на 1 по y
                    {
                        d += d2;
                        y += sy;
                    }
                    else //если точка между пикселями ближе к текущему по вертикали, оставляем Y таким же
                        d += d1;
                    PutPixel(clr, x, y);//рисование пикселя
                    x += sx;//изменяем x на 1
                }
            }
            else//если угол между линией и горизонталью больше 45 градусов
            {
                int d = (dx << 1) - dy;
                int d1 = dx << 1;
                int d2 = (dx - dy) << 1;
                PutPixel(clr, x0, y0);//рисование пикселя
                int x = x0;
                int y = y0 + sy;
                for (int i = 1; i <= dy; i++)//двигаемся по вертикали
                {
                    if (d > 0)//если точка между пикселями ближе к следующему по горизонтали, чем к текущему, изменяем точку на 1 по х
                    {
                        d += d2;
                        x += sx;
                    }
                    else //если точка между пикселями ближе к текущему по горизонтали, оставляем х таким же
                        d += d1;
                    PutPixel(clr, x, y);//рисование пикселя
                    y += sy;//изменяем y на 1
                }
            }
        }

        private void DrawRectangle(Color clr, Point point1, Point point2) //рисовать прямоугольник
        {
            Bresenham4Line(clr, point1.X, point1.Y, point2.X, point1.Y);//верхняя линия
            Bresenham4Line(clr, point1.X, point2.Y, point2.X, point2.Y);//нижняя линия
            Bresenham4Line(clr, point1.X, point1.Y, point1.X, point2.Y);//левая линия
            Bresenham4Line(clr, point2.X, point1.Y, point2.X, point2.Y);//правая линия
        }

        void Pixel4(int x, int y, int _x, int _y, Color color) // Рисование пикселя одновременно с 4 сторон элипса
                                                               //(сверху, снизу, слева, справа)
        {
            PutPixel(color, x + _x, y + _y);
            PutPixel(color, x + _x, y - _y);
            PutPixel(color, x - _x, y - _y);
            PutPixel(color, x - _x, y + _y);
        }

        void DrawElipse(int x, int y, int a, int b, Color color) //функция рисования элипса: x,y - центр элипса
                                                                //а,b - длины осей x y центр
        {
            int _x = 0; // Компонента x откуда начинаем рисовать 
            int _y = b; // Компонента y
            int a_sqr = a * a; // a^2, a - большая полуось
            int b_sqr = b * b; // b^2, b - малая полуось
            int delta = 4 * b_sqr * ((_x + 1) * (_x + 1)) + a_sqr * ((2 * _y - 1) * (2 * _y - 1)) - 4 * a_sqr * b_sqr; // Функция координат точки (x+1, y-1/2)
            while (a_sqr * (2 * _y - 1) > 2 * b_sqr * (_x + 1)) // Первая часть дуги (дуга это четверть элипса)
                                                               //часть - половинка дуги (в данном случае двигаемся по горизонтали)
            {
                Pixel4(x, y, _x, _y, color);//рисование пикселя
                _x++;//изменяем х на 1
                if (delta < 0) //если точка между пикселями ближе к текущему по вертикали, оставляем Y таким же
                {
                    delta += 4 * b_sqr * (2 * _x + 3);
                }
                else //если точка между пикселями ближе к следующему по вертикали, чем к текущему, изменяем точку на 1 по y
                {
                    delta = delta - 8 * a_sqr * (_y - 1) + 4 * b_sqr * (2 * _x + 3);
                    _y--;
                }
            }
            delta = b_sqr * ((2 * _x + 1) * (2 * _x + 1)) + 4 * a_sqr * ((_y + 1) * (_y + 1)) - 4 * a_sqr * b_sqr; // Функция координат точки (x+1/2, y-1)
            while (_y + 1 != 0) // Вторая часть дуги, если не выполняется условие первого цикла, значит выполняется a^2(2y - 1) <= 2b^2(x + 1)
                               //в данном случае двигаемся по вертикали
            {
                Pixel4(x, y, _x, _y, color);//рисование пикселя
                _y--;
                if (delta < 0) //если точка между пикселями ближе к текущему по горизонтали, оставляем x таким же
                {
                    delta += 4 * a_sqr * (2 * _y + 3);
                }
                else //если точка между пикселями ближе к следующему по горизонтали, чем к текущему, изменяем точку на 1 по х
                {
                    delta = delta - 8 * b_sqr * (_x + 1) + 4 * a_sqr * (2 * _y + 3);
                    _x++;
                }
            }
        }

        private void pictureBox1_DownClick(object sender, MouseEventArgs e) // событие зажатие ЛКМ
        {
            if (!flag)//если нет рисунка
            {
                point1 = new Point(e.X, e.Y); //Сохраняем точку
                
            }
            

        }

        private void pictureBox1_UpClick(object sender, MouseEventArgs e)//событие Отжатие ЛКМ
        {
            if (!flag)//если нет рисунка
            {
                pictureBox1.Image = bmp; //отображаем
                g = Graphics.FromImage(pictureBox1.Image);
                flag = true;//рисунок есть
                point2 = new Point(e.X, e.Y);//сохраняем вторую точку
                
                DrawRectangle(Color.Black, point1, point2);//рисуем прямоугольник
                var cx = Convert.ToInt32(Math.Abs((point2.X + point1.X) / 2));//абсцисса центра 
                var cy = Convert.ToInt32(Math.Abs((point2.Y + point1.Y) / 2));//ордината центра
                var circleCenter = new Point(cx, cy);//сохраняем центр
                var circleLenghtY = Convert.ToInt32(Math.Abs((point2.Y - point1.Y) / 2));//длина элипса по оси ординат
                var circleLenghtX = Convert.ToInt32(Math.Abs((point2.X - point1.X) / 2));//длина элипса по оси абсцисс
                DrawElipse(circleCenter.X, circleCenter.Y, circleLenghtX, circleLenghtY, Color.Black);//рисуем элипс
            }
            else//если рисунок уже есть
            {
                g.Clear(Color.White);//очищаем область
                pictureBox1.Image = bmp; //отображаем
                flag = false;//рисунка нет
            }

        }

    }
}
