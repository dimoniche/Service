using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Snake
{
    public partial class Form1 : Form
    {
        // число клеток по осям
        private int valueX = 40;
        private int valueY = 30;
        // курс движения
        Coord course = new Coord(0, 0);
        // голова змейки
        Head head = new Head(0, 0);
        // тело змейки
        List<Coord> body = new List<Coord>();
        // цель
        Coord target;
        // шаги координатной сетки
        float xx;
        float yy;
        // захват цели
        bool capture = false;
        // уровень
        int level = -1;
        // очки по уровням
        int[] points = { 10, 15, 20 };
        // счетчик собранных целей
        int counter = 0;
        // начальная скорость
        const int initSpeed = 120;
        // коэффициент ускорения
        const float coef = 16;

        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;

            xx = canva.Width / valueX;
            yy = canva.Height / valueY;

            head.MaxX = valueX;
            head.MaxY = valueY;
            LevelComplete(null, false);
            target = GetCleanCoord();

        }


        private void Form1_Shown(object sender, EventArgs e)
        {
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            // отрисовка клеток
            xx = canva.Width / valueX;
            yy = canva.Height / valueY;
            for (int i = 0; i < valueX; i++)
            {
                for (int j = 0; j < valueY; j++)
                {
                    e.Graphics.DrawRectangle(new Pen(Color.DarkSlateBlue), i * xx, j * yy, xx, yy);
                }
            }

            // отрисовка головы
            e.Graphics.FillRectangle(new SolidBrush(Color.Red), head.X * xx, head.Y * yy, xx, yy);
            // отрисовка тела
            PaintBody(e);

            // отрисовка цели
            e.Graphics.FillRectangle(new SolidBrush(Color.Yellow), target.X * xx, target.Y * yy, xx, yy);
        }

        private void PaintBody(PaintEventArgs e)
        {
            foreach (var v in body)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.Black), v.X * xx, v.Y * yy, xx, yy);
            }
        }

        private void SetHeadToCenter()
        {
            head.X = (int)valueX / 2 - 1;
            head.Y = (int)valueY / 2 - 1;
            body[0].X = (int)valueX / 2 - 1;
            body[0].Y = (int)valueY / 2;
        }

        private Coord GetCleanCoord()
        {
            Random rm = new Random();
            Coord c = new Coord(0, 0);
            int i = 0;
            do
            {
                i++;
                if (i > 1)
                {
                    i++;
                    i -= 1;
                }
                c.X = rm.Next(0, valueX);
                c.Y = rm.Next(0, valueY);

            }
            while (Crash(c, true));
            return c;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    timer1.Start();
                    course.X = 0;
                    course.Y = -1;
                    break;
                case Keys.Down:
                    timer1.Start();
                    course.X = 0;
                    course.Y = 1;
                    break;
                case Keys.Left:
                    timer1.Start();
                    course.X = -1;
                    course.Y = 0;
                    break;
                case Keys.Right:
                    timer1.Start();
                    course.X = 1;
                    course.Y = 0;
                    break;
                case Keys.Space:
                    if (timer1.Enabled) timer1.Stop();
                    else timer1.Start();
                    break;
            }
        }

        private void MoveBody(Coord prevHead)
        {
            if (body.Count != 0)
            {
                // простое перемещение:
                // ставим хвост на предыдущее место головы
                body.RemoveAt(body.Count - 1);
                body.Insert(0, prevHead);
            }
        }

        private Coord tail;
        private void MoveHead()
        {
            bool ouT = false;
            Head prevHead = (Head)head.Clone();
            try
            {
                head.X += course.X;
                head.Y += course.Y;
                if (head.Equals(body[0]))
                {
                    head = prevHead;
                    timer1.Stop();
                    return;
                }
            }
            catch (System.ArgumentException ex)
            {
                ouT = true;
            }


            if ((!Crash(head, false)) && (!ouT))
            {
                MoveBody(prevHead);
                if (head.Equals(target))
                {
                    capture = true;
                    counter++;
                    label2.Text = "Осталось " + (points[level] - counter).ToString() + " цыплят";

                    if (body.Count != 0) tail = (Coord)body[body.Count - 1].Clone();
                    else tail = (Coord)head.Clone();

                    if (counter == points[level])
                    {
                        LevelComplete("Уровень пройден", false);
                    }
                    target = GetCleanCoord();
                }
                else
                {
                    if (capture)
                    {
                        body.Add(tail);
                        capture = false;
                    }
                }
            }
            else
            {
                //game over
                timer1.Stop();
                LevelComplete("Авария! Вы проиграли!", true);
            }
        }

        private void LevelComplete(string text, bool gameOver)
        {
            timer1.Stop();
            level++;
            body.Clear();
            body.Add(new Coord(0, 0));
            SetHeadToCenter();
            SetLevelSpeed();
            capture = false;

            if (gameOver)
            {
                GameOver(text);
                MessageBox.Show((level + 1).ToString() + " уровень");
                return;
            }

            if (level != points.Length)
            {
                if (level > 0) MessageBox.Show(text);
                MessageBox.Show((level + 1).ToString() + " уровень");
                counter = 0;
                label1.Text = "Уровень " + (level + 1).ToString();
                label2.Text = "Осталось " + (points[level] - counter).ToString() + " цыплят";
            }
            else
                GameOver("Игра пройдена! Поздравляем!");
        }

        private void GameOver(string text)
        {
            MessageBox.Show(text);
            int result = 0;
            for (int i = 0; i < level - 1; i++)
            {
                result += points[i] * (i + 1);
            }
            result += counter * level;
            MessageBox.Show("Вы заработали " + result.ToString() + " очков");
            level = 0;
            SetLevelSpeed();
            target = GetCleanCoord();
            label1.Text = "Уровень " + (level + 1).ToString();
            label2.Text = "Осталось " + (points[level]).ToString() + " целей";
            counter = 0;
        }

        private void SetLevelSpeed()
        {
            timer1.Interval = initSpeed - (int)(level * coef);
        }

        private bool Crash(Coord obj, bool objIsTarget)
        {
            if (body.Count != 0)
            {
                int i;
                if (objIsTarget)
                {
                    i = 0;
                    if (obj.Equals(head))
                        return true;
                }
                else
                    i = 2;

                for (; i < body.Count; i++)
                {
                    if (body[i].Equals(obj)) return true;
                }
                return false;
            }
            else
                return false;
        }



        private void timer1_Tick(object sender, EventArgs e)
        {
            MoveHead();
            canva.Refresh();
        }

        private void canva_Click(object sender, EventArgs e)
        {

        }

        private void canva_MouseClick(object sender, MouseEventArgs e)
        {
            int mx = e.X / valueX;
            int my = e.Y / valueY;
            label4.Text = "XY " + mx.ToString() + "  " + my.ToString();
            if ((course.X == 0) && (course.Y == 0)) //движения нет
            {
                if (Math.Abs(mx - head.X) > Math.Abs(my - head.Y))
                {
                    if (mx > head.X)
                    {
                        course.X = 1;
                    }else
                    {
                        course.X = -1;
                    }
                }else
                {
                    if (my > head.Y)
                    {
                        course.Y = 1;
                    }
                    else
                    {
                        course.Y = -1;
                    }
                }
                timer1.Start();
            }else

            if (course.Y == 0) //движение горизонтальное
            {
                if (my > head.Y)
                {
                    course.Y = 1;
                    course.X = 0;
                }
                else
                if (my < head.Y)
                {
                    course.Y = -1;
                    course.X = 0;
                }
            }
            else
            if (course.X == 0) //движение вертикальное  
            {
                if (mx > head.X)
                {
                    course.X = 1;
                    course.Y = 0;
                }
                else
                if (mx < head.X)
                {
                    course.X = -1;
                    course.Y = 0;

                }
            }
        }
    }
}