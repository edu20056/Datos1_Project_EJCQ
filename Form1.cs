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
        private SingleLinkedForGame player;
        private MotorcycleBot bot; //generic objecto for creating list of the bots
        private ArrayGrid grid; //Object grid for representing the map and leading to the movement of bots and player
        private List<MotorcycleBot> list_bots;
        private List<ArrayGrid.Node> nodes;
        private System.Windows.Forms.Timer timer_player;
        private System.Windows.Forms.Timer[] botTimers;
        private int executionsPerSecond;
        private TextBox speedTextBox;
        private Button updateSpeedButton;
        private Button quit_Button;
        private TextBox current_fuel;
        private int rest_fuel;
        private Random random = new Random();
        private List<int> lista_actual_move_bots = new List<int>(); //this is a list of integers that relate how many times has a bot moved, 
        //if it reaches a certain number, which is the index number in bots_random_distance, it will change direction and reset the integer index
        //related to that bot. Logic in Set_bots_movement().
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
            UpdateTimerInterval(); // Actualiza el intervalo del temporizador del jugador
            timer_player.Tick += new EventHandler(Fuel_Check);
            timer_player.Start();

            // Crear e inicializar los temporizadores de los bots
            int botCount = 4;
            botTimers = new System.Windows.Forms.Timer[botCount];

            for (int i = 0; i < botCount; i++)
            {
                int bot_speed = random.Next(299, 401);
                botTimers[i] = new System.Windows.Forms.Timer();
                botTimers[i].Interval = bot_speed;
                list_bots[i].SetSpeed(bot_speed);
                int botIndex = i; // Capturar la variable en el contexto local
                botTimers[i].Tick += (sender, e) => Set_bots_movement(sender, e, botIndex);
                botTimers[i].Start();
            }
            
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
        private void Fuel_Check(object sender, EventArgs e)
        {
            // Move the motorcycle automatically
            this.motorcycle.Change_Position(nodes);
            int fuel = this.motorcycle.Get_fuel();
            if (fuel > 0)
            {
                if (rest_fuel == 3)
                {
                    fuel--;
                    this.motorcycle.Set_fuel(fuel);
                    current_fuel.Text = "The current fuel is " + fuel + ".";
                    rest_fuel = 0;
                }
                else
                {
                    rest_fuel++;
                }

                // Update positions of Estela objects
                UpdateEstelas_Player();
            }
            else
            {
                timer_player.Stop();
                foreach (System.Windows.Forms.Timer timer in botTimers)
                {
                    timer.Stop();
                }
                MessageBox.Show("Game Over");
            }
        }
        private void UpdateEstelas_Player() //Moves stelas for player
        {
            // Move the last estela first
            for (int i = player.Get_Size() - 1; i > 0; i--)
            {
                if (player.Get(i) is Estela x && player.Get(i - 1) is Estela z)
                {
                    x.Set_Dirr(z.Get_Dirr()); // Set direction based on the previous estela

                    // Move this estela to the position of the previous estela
                    x.Change_Position(nodes, z.Get_X_est(), z.Get_Y_est());
                }
            }

            // Move the first estela (index 1) to follow the motorcycle
            if (player.Get(1) is Estela firstEstela)
            {
                firstEstela.Change_Position(nodes, this.motorcycle.Get_x_player(), this.motorcycle.Get_y_player());
                firstEstela.Set_Dirr(this.motorcycle.Get_Move_Indicator());
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

            List<PictureBox> box_list_bots = new List<PictureBox>(); //List for boxes to be placed over the bots

            List<PictureBox> estelas_boxes = new List<PictureBox>(); //List for boxes to be placed over the bots

            //Implementación de boxes de bots
            for (int j = 0; j < 4; j++) 
            {
                PictureBox pictureBox = new PictureBox();
                box_list_bots.Add(pictureBox);
                pictureBox.Size = new Size(50, 50);
                pictureBox.BackColor = Color.Transparent; 
                this.Controls.Add(pictureBox); 
            }

            // Implementation of boxes for stels
            for (int j = 0; j < 12; j++) 
            {
                PictureBox pictureBox = new PictureBox();
                estelas_boxes.Add(pictureBox);
                pictureBox.Size = new Size(50, 50);
                pictureBox.BackColor = Color.Transparent; 
                this.Controls.Add(pictureBox); 
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

            player = new SingleLinkedForGame();
            this.motorcycle = new Motorcycle(executionsPerSecond, 0, 20, images_player, this.pictureBox1, 1);
            Estela es1_player = new Estela(estelas_boxes[0],2,0,1);
            this.motorcycle.Add_Stels();
            Estela es2_player = new Estela(estelas_boxes[1],1,0,1);
            this.motorcycle.Add_Stels();
            Estela es3_player = new Estela(estelas_boxes[2],0,0,1);
            this.motorcycle.Add_Stels();


            //Adding instances of player
            player.Add(this.motorcycle);
            player.Add(es1_player);
            player.Add(es2_player);
            player.Add(es3_player);


            for (int k = 0; k < 4; k++)
            {
                PictureBox box_grid = box_list_bots[k];
                MotorcycleBot bot = new MotorcycleBot(0, 3, images_bots, box_grid, 5 * k, 7 * k, 1);
                list_bots.Add(bot);
                //Para realizar estelas de bots hace falta lógica para añadirlas, despsués para el movimiento se puede crear una funcion
                //que sea similiar a UpdateEstelas_Player() solo que al inicio habrá un for para cada elemento de la lista de bots
                //Creo que la lista de bots podría cambiarse por una lista de listas tipo DoubleLinkedListForPlayersAndBots.
                //Esto ahorraría lógico, creoooooo.
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
