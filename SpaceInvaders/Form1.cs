using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO.Ports;
using System.Net.NetworkInformation;
using System.Drawing.Text;

namespace Space_Invaders_Style_Game_MOO_ICT
{
    public partial class Form1 : Form
    {
        private CancellationTokenSource cancellationTokenSource;
        SerialPort serialPort;
        bool goLeft, goRight;
        int playerSpeed = 0;
        int enemySpeed = 5;
        int score = 0;
        int lives = 5;
        int enemyBulletTimer = 300;
        PictureBox[] sadInvadersArray;

        bool shooting;
        bool isGameOver;

        private void CancelMakeInvaders()
        {
            cancellationTokenSource?.Cancel();
        }
        static double Map(double value, double fromTarget, double toTarget)
        {
            return (value / 3.30) * (toTarget - fromTarget) + fromTarget;
        }
        
        public Form1()
        {
            InitializeComponent();
            serialPort = new SerialPort("COM9", 9600);

            gameSetup();
        }
        private void mainGameTimerEvent(object sender, EventArgs e)
        {
            try
            {
                String position = "lol";
                String fire = "fire";
                serialPort.Open();
                Console.WriteLine("Serial port opened.");
                string data = serialPort.ReadExisting();
                Console.WriteLine("Received data:" + data);
                string[] rawoutput = data.Split(' ');
                bool flag1 = false;
                bool flag2  = false;
                for (int i = 0; i < rawoutput.Length; i++)
                {
                    if (rawoutput[i].Equals("X"))
                    {
                        Console.WriteLine(rawoutput[i + 1]);
                        position = rawoutput[i + 1];

                        flag1 = true;
                    }
                    if (rawoutput[i].Equals("F"))
                    {
                        fire = rawoutput[i + 1];
                        Console.WriteLine(rawoutput[i]);
                        Console.WriteLine(rawoutput[i + 1]);
                        flag2 = true;
                    }
                    if (flag1 && flag2)
                    {
                        break;
                    }
                }
                double xpos1 = double.Parse(position);
                int fire1 = int.Parse(fire);
                Console.WriteLine("ParsedDouble: " + xpos1);
                //Console.WriteLine("ParsedInt: " + fire1);
                if (xpos1 < 1)
                {
                    player.Left -= 12;
                }
                if (xpos1 > 2)
                {
                    player.Left += 12;
                }
                if (fire1 == 1)
                {
                    
                    shooting = true;
                    makeBullet("bullet");
                }
                
                // int playerspeed = (int)Map(xpos1, -20, 20);
                //Console.WriteLine("Parsed data2:" + playerSpeed);
                Console.ReadLine();
            }
            catch (Exception ex)
            {   
                Console.WriteLine("Error :" , ex.Message);
            }
            finally
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                }
            }
            LivesTracker.Text = "Lives: " + lives;
            txtScore.Text = "Score: " + score;

            //player.Left += playerSpeed;
            
            enemyBulletTimer -= 10;
            if (enemyBulletTimer < 1)
            {
                enemyBulletTimer = 300;
                makeBullet("sadBullet");
            }
            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && (string)x.Tag == "sadInvaders")
                {
                    x.Top += 10;
                    if (x.Top > 600)
                    {
                        this.Controls.Remove(x);
                        lives -= 1;
                        if (lives <= 0)
                        {
                            gameOver("Game Over");
                        }
                    }
                    foreach (Control y in this.Controls)
                    {
                        if (y is PictureBox && (string)y.Tag == "bullet")
                        {
                            if (y.Bounds.IntersectsWith(x.Bounds))
                            {
                                this.Controls.Remove(x);
                                this.Controls.Remove(y);
                                score += 1;
                                shooting = false;
                            }
                        }
                    }
                }
                if (x is PictureBox && (string)x.Tag == "bullet")
                {
                    x.Top -= 20;
                    if (x.Top < 15)
                    {
                        this.Controls.Remove(x);
                        shooting = false;
                    }
                }
                if (x is PictureBox && (string)x.Tag == "sadBullet")
                {
                    x.Top += 20;
                    if (x.Top > 620)
                    {
                        this.Controls.Remove(x);
                    }
                    if (x.Bounds.IntersectsWith(player.Bounds))
                    {
                        lives = 0;
                        this.Controls.Remove(x);
                        gameOver("Game Over");
                    }
                }
            }
            if (score > 8)
            {
                enemySpeed = 12;
            }
            if (score == sadInvadersArray.Length)
            {
                gameOver("Game Over");
            }
        }
       
        private void keyisdown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goLeft = true;
            }
            if (e.KeyCode == Keys.Right)
            {
                goRight = true;
            }
        }
        
        
        private void keyisup(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goLeft = false;
            }
            if (e.KeyCode == Keys.Right)
            {
                goRight = false;
            }
            if (e.KeyCode == Keys.Space && shooting == false)
            {
                shooting = true;
                makeBullet("bullet");
            }
            if (e.KeyCode == Keys.Enter && isGameOver == true)
            {
                CancelMakeInvaders();
                removeAll();
                gameSetup();
            }
        }
        
        private async void makeInvaders()
        {
            cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            sadInvadersArray = new PictureBox[50];
            int top = 0;

            Random random = new Random();
            for (int i = 0; i < sadInvadersArray.Length; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
                int randNum = random.Next(700);
                Console.WriteLine("Random: " +  randNum);
                sadInvadersArray[i] = new PictureBox();
                sadInvadersArray[i].Size = new Size(60, 50);
                sadInvadersArray[i].Image = Properties.Resources.alien;
                sadInvadersArray[i].Tag = "sadInvaders";
                //random number generator for .left position of the sad invader 
                sadInvadersArray[i].Left = randNum;
                sadInvadersArray[i].Top = -50;
                sadInvadersArray[i].SizeMode = PictureBoxSizeMode.StretchImage;
                this.Controls.Add(sadInvadersArray[i]);
                try
                {
                    await Task.Delay(3000, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    // Task was cancelled, exit loop
                    return;
                }
            }
        }

        private void gameSetup()
        {
            txtScore.Text = "Score: 0";
            score = 0;
            lives = 5;
            isGameOver = false;
            enemyBulletTimer = 300;
            enemySpeed = 5;
            shooting = false;
            //Menu here 
            makeInvaders();
            gameTimer.Start();
        }
        
        private async void gameOver(string message)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
            serialPort.Open();
            serialPort.Write("O");
            serialPort.Close();
            isGameOver = true;
            gamedone.Text = message;
            txtScore.Text = "Score: " + score;
            gameTimer.Stop();
            
            
            CancelMakeInvaders();
            //gamedone.Text = " ";
            
            removeAll();
            await Task.Delay(3000);
            gamedone.Text = "Next Game\nIn: 5";
            await Task.Delay(1000);
            gamedone.Text = "Next Game\nIn: 4";
            await Task.Delay(1000);
            gamedone.Text = "Next Game\nIn: 3";
            await Task.Delay(1000);
            gamedone.Text = "Next Game\nIn: 2";
            await Task.Delay(1000);
            gamedone.Text = "Next Game\nIn: 1";
            await Task.Delay(1000);
            gamedone.Text = " ";
            gameSetup();
            /*
            while (true)
            {
                String fire1 = "0";
                serialPort.Open();
                string resetdata = serialPort.ReadExisting();
                string[] rawoutput1 = resetdata.Split(' ');
                
                for (int i = 0; i < rawoutput1.Length; i++)
                {
                    Console.WriteLine(rawoutput1[i]);
                    if (rawoutput1[i].Equals("F"))
                    {
                        Console.WriteLine(rawoutput1[i + 1]);
                        fire1 = rawoutput1[i + 1];
                    }
                }
                int pushed = int.Parse(fire1);
                serialPort.Close();
                if (pushed == 1)
                {
                    CancelMakeInvaders();
                    gamedone.Text = " ";
                    removeAll();
                    gameSetup();
                }
            }
            */

        }

        private void removeAll()
        {
            foreach (PictureBox i in sadInvadersArray)
            {
                this.Controls.Remove(i);
            }
            foreach (Control x in this.Controls)
            {
                if (x is PictureBox)
                {
                    if ((string)x.Tag == "bullet" || (string)x.Tag == "sadBullet")
                    {
                        this.Controls.Remove(x);
                    }
                    
                }
            }
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void player_Click(object sender, EventArgs e)
        {

        }

        private void makeBullet(string bulletTag)
        {
            PictureBox bullet = new PictureBox();
            bullet.Image = Properties.Resources.finalbullet;
            bullet.Size = new Size(6, 20);
            bullet.Tag = bulletTag;
            bullet.Left = player.Left + player.Width / 2;
            if ((string)bullet.Tag == "bullet")
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Write("B");
                }
                bullet.Top = player.Top - 20;
            }
            else if ((string)bullet.Tag == "sadBullet")
            {
                bullet.Top = -100;
            }
            this.Controls.Add(bullet);
            bullet.BringToFront();
            
        }
    }
}
