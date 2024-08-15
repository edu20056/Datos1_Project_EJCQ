using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Windows_Forms_Attempt
{
    public partial class Form1 : Form
    {
        private PictureBox pictureBox1;
        private Motorcycle motorcycle;
        private ArrayGrid grid;
        private List<ArrayGrid.Node> nodes;
        private System.Windows.Forms.Timer timer;
        private int executionsPerSecond;

        // UI Controls for Speed Adjustment
        private TextBox speedTextBox;
        private Button updateSpeedButton;

        public Form1()
        {
            InitializeComponent_1();
            CreateGridDisplay();

            executionsPerSecond = 3; // Default executions per second

            timer = new System.Windows.Forms.Timer();
            UpdateTimerInterval(); // Initialize the timer interval
            timer.Tick += new EventHandler(OnTimedEvent);
            timer.Start();

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(Movement);

            // Initialize and add UI controls for speed adjustment
            InitializeSpeedControls();
        }

        private void InitializeSpeedControls()
        {
            // Initialize the TextBox
            speedTextBox = new TextBox();
            speedTextBox.Location = new Point(1200, 200);
            speedTextBox.Size = new Size(100, 20);
            this.Controls.Add(speedTextBox);

            // Initialize the Button
            updateSpeedButton = new Button();
            updateSpeedButton.Text = "Update Speed";
            updateSpeedButton.Location = new Point(1200, 500);
            updateSpeedButton.Click += new EventHandler(UpdateSpeedButton_Click);
            this.Controls.Add(updateSpeedButton);
        }

        private void UpdateSpeedButton_Click(object sender, EventArgs e)
        {
            // Update motorcycle speed and timer interval based on the value in the TextBox
            int newSpeed;
            if (int.TryParse(speedTextBox.Text, out newSpeed) && newSpeed > 0) //Transforms current string in speedTextBox into int called newSpeed
            {
                this.motorcycle.SetSpeed(newSpeed);
                executionsPerSecond = newSpeed; // Use the new speed as the execution rate

                UpdateTimerInterval(); // Update the timer interval based on the new speed
            }
            else
            {
                MessageBox.Show("Please enter a valid positive integer.");
            }
        }

        private void UpdateTimerInterval()
        {
            if (executionsPerSecond > 0)
            {
                timer.Interval = 1000 / executionsPerSecond; // Update interval to match new speed
            }
        }

        private void OnTimedEvent(object sender, EventArgs e)
        {
            // Move the motorcycle automatically
            this.motorcycle.Change_Position(nodes);
        }

        private void Movement(object sender, KeyEventArgs e)
        {
            // Change direction based on the arrow key pressed
            switch (e.KeyCode)
            {
                case Keys.Up:
                    this.motorcycle.Move_Image(2);
                    this.motorcycle.Change_Direction(2);
                    break;
                case Keys.Down:
                    this.motorcycle.Move_Image(3);
                    this.motorcycle.Change_Direction(4);
                    break;
                case Keys.Left:
                    this.motorcycle.Move_Image(1);
                    this.motorcycle.Change_Direction(3);
                    break;
                case Keys.Right:
                    this.motorcycle.Move_Image(0);
                    this.motorcycle.Change_Direction(1);
                    break;
            }
        }

        private void InitializeComponent_1()
        {
            // Configuración del formulario
            this.ClientSize = new System.Drawing.Size(1400, 800);
            this.Name = "Form1";
            this.Text = "Trone Game";

            // Inicializar PictureBox
            this.pictureBox1 = new PictureBox();
            this.pictureBox1.Size = new Size(50, 50);
            this.pictureBox1.BackColor = Color.Transparent; // No background color
            this.Controls.Add(this.pictureBox1);

            // Crear la instancia de Motorcycle
            string[] images_player = new string[]
            {
                "Imagenes/jugador/moto_jugador_derecha.png",
                "Imagenes/jugador/moto_jugador_izquierda.png",
                "Imagenes/jugador/moto_jugador_arriba.png",
                "Imagenes/jugador/moto_jugador_abajo.png"
            };

            this.motorcycle = new Motorcycle(executionsPerSecond, 5, 4, images_player, this.pictureBox1, 1);
        }

        private void CreateGridDisplay()
        {
            grid = new ArrayGrid(24, 16);
            nodes = grid.GetNodes();
            int nodeSize = 50; // Tamaño de cada PictureBox

            foreach (ArrayGrid.Node node in nodes)
            {
                PictureBox box = new PictureBox();
                box.Size = new Size(nodeSize, nodeSize);
                box.Location = new Point(node.GetX() * nodeSize, node.GetY() * nodeSize);
                box.BackColor = Color.Transparent;
                box.BorderStyle = BorderStyle.FixedSingle;
                this.Controls.Add(box);

                node.Box = box; // Asociar el PictureBox con el nodo
            }
        }
    }
}
