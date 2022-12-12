using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snake
{

    /* Модель змейки:
     * 1) координата головы (X, Y)
     * 2) Длина змеи
     * 3) Массив направлений от головы до хвоста
     */
    public partial class Form1 : Form
    {
        Label[,] labelArray;
        int direction;
        int[] directions;
        // 0 - up
        // 1 - right
        // 2 - down
        // 3 - left
        int previousX, previousY, X, Y, fruitX, fruitY, tailX, tailY, size, snakeSize;
        Random r;
        Boolean autoControl;
        public Form1()
        {
            InitializeComponent();
            timer1.Interval = 100;
            autoControl = false;
            size = 16;
            directions = new int[size * size];
            labelArray = new Label[size, size];
            r = new Random();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Label label = new Label();
                    label.BackColor = Color.White;
                    label.Width = 20;
                    label.Height = 20;
                    label.Location = new Point(2 + i * 22, 2 + j * 22);
                    this.Controls.Add(label);
                    labelArray[i, j] = label;
                }
            }
            autoControlSet();
            firstStep();
            generateFruitPosition();
        }

        private void autoControlSet()
        {
            if (autoControl)
            {
                this.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            }
        }

        private void firstStep()
        {
            X = 6; Y = 6;
            previousX = X; previousY = Y;
            snakeSize = 5;
            direction = 1;
            for (int i = 0; i < snakeSize - 1; i++)
                directions[i] = 1;
            for (int i = 0; i < snakeSize - 1; i++)
                labelArray[X - i, Y].BackColor = Color.Black;
        }
        private void clearTail()
        {
            tailX = X;
            tailY = Y;
            for (int i = 0; i < snakeSize - 1; i++)
            {
                switch (directions[i])
                {
                    case 0:
                        tailY++;
                        break;
                    case 1:
                        tailX--;
                        break;
                    case 2:
                        tailY--;
                        break;
                    case 3:
                        tailX++;
                        break;
                }
            }
            changeColor(tailX, tailY, 2);
        }

        private void renewDirections()
        {
            for (int i = 0; i < snakeSize - 1; i++)
                directions[snakeSize - i - 1] = directions[snakeSize - i - 2];
            directions[0] = direction;
        }

        private void generateFruitPosition()
        {
            do
            {
                fruitX = r.Next(0, size - 1);
                fruitY = r.Next(0, size - 1);
            }
            while (labelArray[fruitX, fruitY].BackColor == Color.Black);
            labelArray[fruitX, fruitY].BackColor = Color.Lime;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (direction != 2)
                        direction = 0;
                    break;
                case Keys.Right:
                    if (direction != 3)
                        direction = 1;
                    break;
                case Keys.Down:
                    if (direction != 0)
                        direction = 2;
                    break;
                case Keys.Left:
                    if (direction != 1)
                        direction = 3;
                    break;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            changePosition();
            renewDirections();
            if (autoControl)
                autoDirectionChange();
            if (X == fruitX && Y == fruitY)
            {
                increaseScore();
                generateFruitPosition();
                snakeSize++;
            }
            else
                clearTail();
        }

        private void checkFruit()
        {
            if (X == fruitX && Y == fruitY)
            {
                increaseScore();
                generateFruitPosition();
            }
        }

        private int getTailX()
        {
            tailX = X;
            for (int i = 0; i < snakeSize - 1; i++)
            {
                switch (directions[i])
                {
                    case 1:
                        tailX--;
                        break;
                    case 3:
                        tailX++;
                        break;
                    default:
                        break;
                }
            }
            return tailX;
        }

        private void autoDirectionChange()
        {
            if (X == 0 && Y == 0)
            {
                direction = 1;
                return;
            }
            if (X == 0 && Y == size - 1)
            {
                direction = 0;
                return;
            }
            if (X == size - 1 && Y%2 == 0)
            {
                direction = 2;
                return;
            }
            if (X == size - 1 && Y % 2 == 1)
            {
                direction = 3;
                return;
            }
            if (X == 1 && Y%2 == 0 && Y != 0)
            {
                direction = 1;
                return;
            }
            if (X == 1 && Y % 2 == 1 && Y != size - 1)
            {
                direction = 2;
                return;
            }

        }

        private void changePosition()
        {
            previousX = X;
            previousY = Y;
            switch (direction)
            {
                case 0:
                    Y--;
                    break;
                case 1:
                    X++;
                    break;
                case 2:
                    Y++;
                    break;
                case 3:
                    X--;
                    break;
            }
            if (X == -1 || X == size || Y == -1 || Y == size || labelArray[X, Y].BackColor == Color.Black)
                gameOver();
            else
                changeColor(X, Y, 1);
        }

        private void changeColor(int X, int Y, int color) // 1 - black, 2 - white, 3 - lime
        {
            try
            {
                switch (color)
                {
                    case 1:
                        labelArray[X, Y].BackColor = Color.Black;
                        break;
                    case 2:
                        labelArray[X, Y].BackColor = Color.White;
                        break;
                    case 3:
                        labelArray[X, Y].BackColor = Color.Lime;
                        break;
                }
            }
            catch (IndexOutOfRangeException e)
            {
                gameOver();
            }
        }

        private void gameOver()
        {
            timer1.Enabled = false;
            DialogResult dialogResult = MessageBox.Show("Your score is " + labelScore.Text + "\nStart new game?", "You lost", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                newGame();
            }
        }

        private void gameWin()
        {
            timer1.Enabled = false;
            DialogResult dialogResult = MessageBox.Show("Your score is " + labelScore.Text + "\nStart new game?", "You WIN!!!", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                newGame();
            }
        }

        private void newGame()
        {
            foreach (Label l in labelArray)
                l.BackColor = Color.White;
            firstStep();
            generateFruitPosition();
            labelScore.Text = "0";
            timer1.Enabled = true;
        }

        private void increaseScore()
        {
            labelScore.Text = Convert.ToString(Convert.ToInt32(labelScore.Text) + 1);
            if (labelScore.Text == "250")
                gameWin();
        }
    }
}
