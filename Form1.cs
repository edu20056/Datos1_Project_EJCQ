using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Windows_Forms_Attempt
{
    public partial class Form1 : Form
    {
        private PictureBox pictureBox1; //Box for player
        private Motorcycle motorcycle; //Players object
        private MotorcycleBot bot; //generic objecto for creating list of the bots
        private ArrayGrid grid; //Object grid for representing the map and leading to the movement of bots and player
        private List<MotorcycleBot> list_bots;
        private List<ArrayGrid.Node> nodes;
        private System.Windows.Forms.Timer timer_player;
        private System.Windows.Forms.Timer timer_bot1;
        private System.Windows.Forms.Timer timer_bot2;
        private System.Windows.Forms.Timer timer_bot3;
        private System.Windows.Forms.Timer timer_bot4;
        private int executionsPerSecond;
        private TextBox speedTextBox;
        private Button updateSpeedButton;
        private Button quit_Button;
        private TextBox current_fuel;
        private int rest_fuel;
        private Random random = new Random();
        private List<int> lista_actual_move_bots = new List<int>();
        private List<int> bots_random_distance = new List<int>();
        public Form1()
        {
            InitializeComponent_1();
            CreateGridDisplay();
            for (int i = 0; i < 4; i++)
            {
                lista_actual_move_bots.Add(0);
            }

            for (int i = 0; i < 4; i++)
            {
                bots_random_distance.Add(0);
            }
            rest_fuel = 0;
            executionsPerSecond = 3; // Default executions per second, it represents the speed.

            timer_player = new System.Windows.Forms.Timer();
            UpdateTimerInterval(); //timer.Interval = 333; 
            timer_player.Tick += new EventHandler(Fuel_Check);
            timer_player.Start();

            timer_bot1 = new System.Windows.Forms.Timer();
            timer_bot1.Interval = 400;
            timer_bot1.Tick += (sender, e) => Set_bots_movement(sender, e, 0); 
            timer_bot1.Start();
            
            timer_bot2 = new System.Windows.Forms.Timer();
            timer_bot2.Interval = 400;
            timer_bot2.Tick += (sender, e) => Set_bots_movement(sender, e, 1); 
            timer_bot2.Start();

            timer_bot3 = new System.Windows.Forms.Timer();
            timer_bot3.Interval = 400;
            timer_bot3.Tick += (sender, e) => Set_bots_movement(sender, e, 2); 
            timer_bot3.Start();
            
            timer_bot4 = new System.Windows.Forms.Timer();
            timer_bot4.Interval = 400;
            timer_bot4.Tick += (sender, e) => Set_bots_movement(sender, e, 3); 
            timer_bot4.Start();
            
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(Movement);

            InitializeControls();
        }
        public void Set_bots_movement(object sender, EventArgs e, int k)
         {
            bot = list_bots[k];
            if (lista_actual_move_bots[k] == 0)
            {
                int move = random.Next(3, 7);
                bots_random_distance[k] = move;
                int actual_dir = bot.Current_Image_Direction();
                int direction;

                do
                {
                    direction = random.Next(0, 4);
                } while (direction == (actual_dir + 2) % 4); // Avoid moving directly backwards

                bot.Move_Image(direction);
            }
            else if (lista_actual_move_bots[k] == bots_random_distance[k] )
            {
                lista_actual_move_bots[k] = 0;
            }
            else
            {
                lista_actual_move_bots[k]++;
            }
            
            bot.Change_Position(nodes);
        }
        private void InitializeControls() // Initialize all textboxes and buttons
        {
            
            int fuel = this.motorcycle.Get_fuel();

            // Initialize the TextBox for changing speed
            speedTextBox = new TextBox();
            speedTextBox.Location = new Point(1200, 200);
            speedTextBox.Size = new Size(100, 20);
            this.Controls.Add(speedTextBox);

            // Initialize the Button for making the update in the speed
            updateSpeedButton = new Button();
            updateSpeedButton.Text = "Update Speed";
            updateSpeedButton.Location = new Point(1200, 400);
            updateSpeedButton.Click += new EventHandler(UpdateSpeedButton_Click);
            this.Controls.Add(updateSpeedButton);

            // Initialize Button for exiting the game.
            quit_Button = new Button();
            quit_Button.Text =  "Exit the game";
            quit_Button.Size = new Size(100,100);
            quit_Button.Location = new Point(1200, 650);
            quit_Button.Click += new EventHandler(Quit_Game);
            this.Controls.Add(quit_Button);
            
            // Configure the message box
            this.current_fuel = new TextBox();
            current_fuel.ReadOnly = true;
            current_fuel.Text = "The current fue is " + fuel + ".";
            current_fuel.Size = new Size(150,50);
            current_fuel.Location = new System.Drawing.Point(1200,550);
            this.Controls.Add(current_fuel);
        }
        public void Quit_Game(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void UpdateSpeedButton_Click(object sender, EventArgs e) //Updates speed value
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

        private void UpdateTimerInterval() //This function changes directly the execution per second in terms of movement. 
        {
            if (executionsPerSecond > 0)
            {
                timer_player.Interval = 1000 / executionsPerSecond; // Update interval to match new speed
            }
        }

        private void Fuel_Check(object sender, EventArgs e) //This function changes player positions and checks if players still has fuel.
        {
            // Move the motorcycle automatically
            this.motorcycle.Change_Position(nodes);
            int fuel = this.motorcycle.Get_fuel();
            if (fuel > 0)
            {
                if (rest_fuel == 3) //This means that, this line will be called 3 times, the motorcycle will move 3 blocks and only then loss 1 in fuel
                {
                    fuel--;
                    this.motorcycle.Set_fuel(fuel);
                    current_fuel.Text = "The current fue is " + fuel + ".";
                    rest_fuel = 0;
                }

                else
                {
                    rest_fuel++;
                }
            }
            else
            {
                timer_player.Stop();
                MessageBox.Show("Game Over");
            }
        }

        private void Movement(object sender, KeyEventArgs e) //if any key is pressed, changes players direction
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

        private void InitializeComponent_1() //Main loop
        {
            // Configuración del formulario
            this.ClientSize = new System.Drawing.Size(1400, 800);
            this.Name = "Form1";
            this.Text = "Trone Game";

            // Inicializar PictureBox por player
            this.pictureBox1 = new PictureBox();
            this.pictureBox1.Size = new Size(50, 50);
            this.pictureBox1.BackColor = Color.Transparent; // No background color
            this.Controls.Add(this.pictureBox1);

            list_bots = new List<MotorcycleBot>(); // List of bots for future iterating them

            List<PictureBox> box_list_bots = new List<PictureBox>();

            for (int j = 0; j < 4; j++) 
            {
                PictureBox pictureBox = new PictureBox();
                box_list_bots.Add(pictureBox);
            }
            
            foreach (PictureBox pictureBox in box_list_bots)
            {
                pictureBox.Size = new Size(50, 50);
                pictureBox.BackColor = Color.Transparent; // Fondo transparente
                this.Controls.Add(pictureBox); // Agregar al formulario
            }

            // Crear la instancia de Motorcycle
            string[] images_player = new string[]
            {
                "Imagenes/jugador/moto_jugador_derecha.png",
                "Imagenes/jugador/moto_jugador_izquierda.png",
                "Imagenes/jugador/moto_jugador_arriba.png",
                "Imagenes/jugador/moto_jugador_abajo.png"
            };


            string[] images_bots = new string[]
            {
                "Imagenes/bots/moto_bot_derecha.png",
                "Imagenes/bots/moto_bot_arriba.png",
                "Imagenes/bots/moto_bot_izquierda.png",
                "Imagenes/bots/moto_bot_abajo.png"
            };

            this.motorcycle = new Motorcycle(executionsPerSecond, 3, 100, images_player, this.pictureBox1, 1);

            for (int k = 0; k < 4; k++)
            {
                PictureBox box_grid = box_list_bots[k];
                MotorcycleBot bot = new MotorcycleBot(3, 3, images_bots, box_grid, 5 * k, 7 * k, 1);
                list_bots.Add(bot);
            }
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
