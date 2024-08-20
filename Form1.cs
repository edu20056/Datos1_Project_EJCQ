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
        private MotorcycleBot bot_colision; //generic objecto for analizing colision between bots
        private ArrayGrid grid; //Object grid for representing the map and leading to the movement of bots and player
        private List<MotorcycleBot> list_bots; //lists of bot instances
        private SingleLinkedForGame bot1 = new SingleLinkedForGame();
        private SingleLinkedForGame bot2 = new SingleLinkedForGame();
        private SingleLinkedForGame bot3 = new SingleLinkedForGame();
        private SingleLinkedForGame bot4 = new SingleLinkedForGame();

        private List<Object> All_Objects_For_Colisions = new List<object>();
        private List<ArrayGrid.Node> nodes;
        private Estela estela;
        private System.Windows.Forms.Timer timer_player;
        private System.Windows.Forms.Timer[] botTimers;
        private int executionsPerSecond;
        private TextBox speedTextBox;
        private Button updateSpeedButton;
        private Button quit_Button;
        private TextBox current_fuel;
        private int rest_fuel;
        private Random random = new Random();
        private int killed_bot = 0;
        private List<int> lista_actual_move_bots = new List<int>(); //this is a list of integers that relate how many times has a bot moved, 
        //if it reaches a certain number, which is the index number in bots_random_distance, it will change direction and reset the integer index
        //related to that bot. Logic in Set_bots_movement().
        private List<int> bots_random_distance = new List<int>();
        public Form1() // Start
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
            this.KeyDown += new KeyEventHandler(Movement_Key_detector);

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

            if (k == 0)
            {
                Analice_Colisions(bot, bot1);
                UpdateEstelas_Bots(bot1, bot);
            }

            else if (k == 1)
            {
                Analice_Colisions(bot, bot2);
                UpdateEstelas_Bots(bot2, bot);
            }

            else if (k == 2)
            {
                Analice_Colisions(bot, bot3);
                UpdateEstelas_Bots(bot3, bot);
            }
            else
            {
                Analice_Colisions(bot, bot4);
                UpdateEstelas_Bots(bot4, bot);
            }

            
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
        private void Analice_Colisions(object ob, SingleLinkedForGame list)
        {
            try 
            {
                Motorcycle moto = (Motorcycle)ob;
                foreach (object objecto in All_Objects_For_Colisions)
                {
                    if (moto == objecto)
                    {
                        continue;
                    }
                    else
                    {
                        try
                        {
                            MotorcycleBot bot = (MotorcycleBot)objecto;
                            if (bot.Get_x_bot() == moto.Get_x_player() && bot.Get_y_bot() == moto.Get_y_player())
                            {
                                timer_player.Stop();
                                foreach (System.Windows.Forms.Timer timer in botTimers)
                                {
                                    timer.Stop();
                                }
                                MessageBox.Show("Game Over");
                            }
                        }
                        catch
                        {
                            Estela estela = (Estela)objecto;
                            if (estela == list.Get(1))
                            {
                                continue;
                            }
                            else if (estela.Get_X_est() == moto.Get_x_player() && estela.Get_Y_est() == moto.Get_y_player())
                            {
                                timer_player.Stop();
                                foreach (System.Windows.Forms.Timer timer in botTimers)
                                {
                                    timer.Stop();
                                }
                                MessageBox.Show("Game Over");
                            }
                        }
                    }
                }
            }
            catch
            {
                MotorcycleBot bot = (MotorcycleBot)ob;
                foreach (object objecto in All_Objects_For_Colisions)
                {
                    if (objecto is Motorcycle )
                    {
                        continue;
                    }

                    else if (objecto == bot)
                    {
                        continue;
                    }
                    try 
                    {
                        Estela estela = (Estela)objecto;
                        if (estela.Get_X_est() == bot.Get_x_bot() && estela.Get_Y_est() == bot.Get_y_bot())
                        {
                            botTimers[bot.Get_position_list_indicator()].Stop();
                            killed_bot++;
                            bot.Move_Image(4);
                        }
                    }
                    catch
                    {
                        MotorcycleBot bot_colision = (MotorcycleBot)objecto;
                        if (bot_colision.Get_x_bot() == bot.Get_x_bot() && bot_colision.Get_y_bot() == bot.Get_y_bot())
                        {
                            botTimers[bot.Get_position_list_indicator()].Stop();
                            bot.Move_Image(4);
                            killed_bot++;
                            botTimers[bot_colision.Get_position_list_indicator()].Stop();
                            bot_colision.Move_Image(4);
                            killed_bot++;
                        } 
                    }
                }
            }
        }
        private void UpdateEstelas_Bots(SingleLinkedForGame list, MotorcycleBot bot) //Moves stelas for each bot
        {
            // Move the last estela first
            for (int i = list.Get_Size() - 1; i > 0; i--)
            {
                if (list.Get(i) is Estela x && list.Get(i - 1) is Estela z)
                {
                    x.Set_Dirr(z.Get_Dirr()); // Set direction based on the previous estela

                    // Move this estela to the position of the previous estela
                    x.Change_Position(nodes, z.Get_X_est(), z.Get_Y_est());
                }
            }

            // Move the first estela (index 1) to follow the motorcycle
            if (list.Get(1) is Estela firstEstela)
            {
                firstEstela.Set_Dirr(bot.Get_Move_Indicator());
                firstEstela.Change_Position(nodes, bot.Get_x_bot(), bot.Get_y_bot());
            }
        }
        private void Check_Killed_Bots()
        {
            if (killed_bot == 4)
            {
                timer_player.Stop();
                foreach (System.Windows.Forms.Timer timer in botTimers)
                {
                    timer.Stop();
                }
                MessageBox.Show("Win, you managed to kill all enemies!");
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
            Check_Killed_Bots();
            this.motorcycle.Change_Position(nodes);
            Analice_Colisions(this.motorcycle, player);
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
        private void Movement_Key_detector(object sender, KeyEventArgs e) //if any key is pressed, changes players direction
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
            for (int j = 0; j < 30; j++) 
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
                "Imagenes/bots/moto_bot_abajo.png",
                "Imagenes/bots/escombros.png"
            };

            player = new SingleLinkedForGame();
            this.motorcycle = new Motorcycle(executionsPerSecond, 0, 1000, images_player, this.pictureBox1, 1);
            player.Add(this.motorcycle);
            All_Objects_For_Colisions.Add(this.motorcycle);

            for (int i = 0; i < 3; i++) //Creation of players stels.
            {
                estela = new Estela(estelas_boxes[i], Math.Abs(i - 8), 0);
                this.motorcycle.Add_Stels();
                player.Add(estela);
                All_Objects_For_Colisions.Add(estela);
            }

            for (int k = 0; k < 4; k++) //Creation of bots
            {
                PictureBox box_grid = box_list_bots[k];
                MotorcycleBot bot = new MotorcycleBot(0, 0, images_bots, box_grid, 5 * k, 7 * k, 1, k);
                list_bots.Add(bot);
                All_Objects_For_Colisions.Add(bot);
                Organize_Bots_In_List(bot, k, estelas_boxes);
            }
        }
        private void Organize_Bots_In_List(MotorcycleBot bot , int indicator, List<PictureBox> estelas_boxes )
        {
            if (indicator == 0)
            {
                bot1.Add(bot);
                Create_Stels_For_bots(bot1, 1, estelas_boxes);
            }
            else if (indicator == 1)
            {
                bot2.Add(bot);
                Create_Stels_For_bots(bot2, 2, estelas_boxes);
            }
            else if (indicator == 2)
            {
                bot3.Add(bot);
                Create_Stels_For_bots(bot3, 3, estelas_boxes);
            }
            else
            {
                bot4.Add(bot);
                Create_Stels_For_bots(bot4, 4, estelas_boxes);
            }
        }
        private void Create_Stels_For_bots(SingleLinkedForGame list, int num_bot, List<PictureBox> estelas_boxes)
        {
            if (num_bot == 1)
            {
                for (int i = 4; i < 7; i++)
                {
                    estela = new Estela(estelas_boxes[i], Math.Abs(i - 2), 3);
                    list_bots[0].Add_Stels();
                    bot1.Add(estela);
                    All_Objects_For_Colisions.Add(estela);
                }
            }

            else if (num_bot == 2)
            {
                for (int i = 8; i < 11; i++)
                {
                    estela = new Estela(estelas_boxes[i], Math.Abs(i - 2), 5);
                    list_bots[1].Add_Stels();
                    bot2.Add(estela);
                    All_Objects_For_Colisions.Add(estela);
                }
            }

            else if (num_bot == 3)
            {
                for (int i = 12; i < 15; i++)
                {
                    estela = new Estela(estelas_boxes[i], Math.Abs(i - 2), 7);
                    list_bots[2].Add_Stels();
                    bot3.Add(estela);
                    All_Objects_For_Colisions.Add(estela);
                }
            }
            else
            {
                for (int i = 16; i < 19; i++)
                {
                    estela = new Estela(estelas_boxes[i], Math.Abs(i - 2), 9);
                    list_bots[3].Add_Stels();
                    bot4.Add(estela);
                    All_Objects_For_Colisions.Add(estela);
                }
            }
        }
        private void CreateGridDisplay() //Creates Grid using Nodes
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
