using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace GameOfLife
{
    public partial class Form1 : Form
    {
        private const int GridWidth = 40;
        private const int GridHeight = 30;
        private const int CellSize = 20;

        private bool[,] grid;
        private bool[,] nextGrid;
        private Timer timer;
        private bool isPaused;

        public Form1()
        {
            InitializeComponent();
            InitializeGrid();
            InitializeTimer();
            isPaused = false;
        }

        private void InitializeGrid()
        {
            grid = new bool[GridWidth, GridHeight];
            nextGrid = new bool[GridWidth, GridHeight];
            Random rand = new Random();

            // Zwiększona szansa na żywe komórki
            for (int x = 0; x < GridWidth; x++)
            {
                for (int y = 0; y < GridHeight; y++)
                {
                    grid[x, y] = rand.NextDouble() > 0.5; // 50% szansa na czarną kratkę
                }
            }
        }

        private void InitializeTimer()
        {
            timer = new Timer();
            timer.Interval = 300; // Aktualizacja co 300ms dla płynności
            timer.Tick += (sender, args) =>
            {
                if (!isPaused)
                {
                    UpdateGrid();
                    Invalidate(); // Ograniczamy rysowanie do minimum
                }
            };
            timer.Start();
        }

        private void UpdateGrid()
        {
            for (int x = 0; x < GridWidth; x++)
            {
                for (int y = 0; y < GridHeight; y++)
                {
                    int neighbors = GetLiveNeighborCount(x, y);
                    if (grid[x, y])
                    {
                        nextGrid[x, y] = neighbors == 2 || neighbors == 3;
                    }
                    else
                    {
                        nextGrid[x, y] = neighbors == 3;
                    }
                }
            }

            // Przełączamy siatkę
            var temp = grid;
            grid = nextGrid;
            nextGrid = temp;
        }

        private int GetLiveNeighborCount(int x, int y)
        {
            int count = 0;
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    int nx = (x + dx + GridWidth) % GridWidth;
                    int ny = (y + dy + GridHeight) % GridHeight;
                    if (grid[nx, ny]) count++;
                }
            }
            return count;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawCells(e.Graphics);
        }

        private void DrawCells(Graphics g)
        {
            // Rysujemy tylko zmienione komórki
            Brush blackBrush = new SolidBrush(Color.Black);
            Brush whiteBrush = new SolidBrush(Color.White);

            for (int x = 0; x < GridWidth; x++)
            {
                for (int y = 0; y < GridHeight; y++)
                {
                    Rectangle cell = new Rectangle(x * CellSize, y * CellSize, CellSize, CellSize);
                    if (grid[x, y])
                    {
                        g.FillRectangle(blackBrush, cell);
                    }
                    else
                    {
                        g.FillRectangle(whiteBrush, cell);
                    }
                }
            }
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.X / CellSize;
            int y = e.Y / CellSize;
            if (x < GridWidth && y < GridHeight)
            {
                grid[x, y] = !grid[x, y];
                Invalidate();
            }
        }

        private void BtnPauseResume_Click(object sender, EventArgs e)
        {
            isPaused = !isPaused;
            btnPauseResume.Text = isPaused ? "Resume" : "Pause";
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            using (StreamWriter writer = new StreamWriter("gameState.txt"))
            {
                for (int y = 0; y < GridHeight; y++)
                {
                    for (int x = 0; x < GridWidth; x++)
                    {
                        writer.Write(grid[x, y] ? 1 : 0);
                    }
                    writer.WriteLine();
                }
            }
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            if (File.Exists("gameState.txt"))
            {
                using (StreamReader reader = new StreamReader("gameState.txt"))
                {
                    for (int y = 0; y < GridHeight; y++)
                    {
                        string line = reader.ReadLine();
                        for (int x = 0; x < GridWidth; x++)
                        {
                            grid[x, y] = line[x] == '1';
                        }
                    }
                }
                Invalidate();
            }
            else
            {
                MessageBox.Show("No saved game found.");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
