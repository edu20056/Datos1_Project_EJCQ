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
        private List<MotorcycleBot> list_bots; //lists of bot instances
        private SingleLinkedForGame bot1 = new SingleLinkedForGame();//bot1 LinkedList
        private SingleLinkedForGame bot2 = new SingleLinkedForGame();//bot2 LinkedList
        private SingleLinkedForGame bot3 = new SingleLinkedForGame();//bot3 LinkedList
        private SingleLinkedForGame bot4 = new SingleLinkedForGame();//bot4 LinkedList
        private List<Object> All_Objects_For_Colisions = new List<object>(); //All objects that can kill player or bots.
        private List<PriorityQueue> List_Items_All_Characters = new List<PriorityQueue>(); //List with all item Queue list for each bot and the player.
        private List<ArrayStack> List_Power_Ups_All_Characters = new List<ArrayStack>(); //List with all power ups for each bot and the player.
        private List<PictureBox> Boxes_for_items_and_powerups = new List<PictureBox>(); //List that contains all Pictures boxes of all items and powerups
        private List<item_PU> All_items_and_powerups = new List<item_PU>(); //List with all items and powerups presented.
        private int ref_it_pu = 0; //reference for using PictureBox when spawning item of power up.
        private List<ArrayGrid.Node> nodes; //Array with nodes for creating map.
        private Estela estela;
        private System.Windows.Forms.Timer timer_player;
        private System.Windows.Forms.Timer Spaw_items_powerups;
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
        private List<int> current_bots_nedded_rest_fuel = new List<int>(); //Gives a number that when reaches another certain number will change 
        //by decreasing the fuel value in each bot.
        public Form1() // Start
        {
            InitializeComponent_1();
            Create_item_and_powerups_list();
            CreateGridDisplay();
            for (int i = 0; i < 4; i++)
            {
                lista_actual_move_bots.Add(0);
                bots_random_distance.Add(0);
                current_bots_nedded_rest_fuel.Add(0);
            }

            rest_fuel = 0;
            executionsPerSecond = 3; // Default executions per second, it represents the speed.

            timer_player = new System.Windows.Forms.Timer();
            UpdateTimerInterval(); // Actualiza el intervalo del temporizador del jugador
            timer_player.Tick += new EventHandler(Fuel_Check_And_Player_Movement);
            timer_player.Start();

            Spaw_items_powerups = new System.Windows.Forms.Timer();
            Spaw_items_powerups.Interval = 15000;
            Spaw_items_powerups.Tick += new EventHandler(Spaw_Consumables);
            Spaw_items_powerups.Start();


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
                botTimers[i].Tick += (sender, e) => Set_bots_movement_Fuel_check(sender, e, botIndex);
                botTimers[i].Start();
            }
            
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(Movement_Key_detector);

            InitializeControls();
        }
        public void Set_bots_movement_Fuel_check(object sender, EventArgs e, int k)
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
            
            int numberfuel = current_bots_nedded_rest_fuel[k];
            int fuel = bot.Get_Fuel();
            if (fuel > 0)
            {
                if (numberfuel == 3) //The bot moves 3 times before losing 1 fuel.
                {
                    fuel--;
                    bot.Set_Fuel(fuel);
                    current_bots_nedded_rest_fuel[k] = 0;
                }
                else
                {
                    numberfuel++;
                    current_bots_nedded_rest_fuel[k] = numberfuel;
                }
            }
            else
            {
                int ind = bot.Get_position_list_indicator();
                botTimers[ind].Stop();
                bot.Move_Image(4);
                All_Objects_For_Colisions.Remove(bot);
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
        private void Colisions_With_Consumable(Object npc, int indicator)
        {
            
            try
            {
                Motorcycle motorcycle = (Motorcycle)npc;
                int x = motorcycle.Get_x_player();
                int y = motorcycle.Get_y_player();
            }
            catch
            {
                MotorcycleBot bot = (MotorcycleBot)npc;
                int x = bot.Get_x_bot();
                int y = bot.Get_y_bot();

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
                                Spaw_items_powerups.Stop();
                                foreach (System.Windows.Forms.Timer timer in botTimers)
                                {
                                    timer.Stop();
                                }
                                MessageBox.Show("Game Over! You collided with a game Bot.");
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
                                Spaw_items_powerups.Stop();
                                foreach (System.Windows.Forms.Timer timer in botTimers)
                                {
                                    timer.Stop();
                                }
                                MessageBox.Show("Game Over! You collided with a curious object.");
                            }
                        }
                    }
                }
            }
            catch
            {
                MotorcycleBot bot = (MotorcycleBot)ob;
                for (int i = All_Objects_For_Colisions.Count - 1; i >= 0; i--) // foreach cant be used here because the destroyed bots must be
                //removed form "All_Objects_For_Colisions".
                {
                    object objecto = All_Objects_For_Colisions[i];
                    
                    if (objecto is Motorcycle)
                    {
                        continue;
                    }

                    if (objecto == bot)
                    {
                        continue;
                    }

                    try
                    {
                        Estela estela = (Estela)objecto;
                        if (estela.Get_X_est() == bot.Get_x_bot() && estela.Get_Y_est() == bot.Get_y_bot())
                        {
                            botTimers[bot.Get_position_list_indicator()].Stop();
                            bot.Move_Image(4);
                            All_Objects_For_Colisions.Remove(bot);

                            Check_Killed_Bots();
                        }
                    }
                    catch
                    {
                        MotorcycleBot bot_colision = (MotorcycleBot)objecto;
                        if (bot_colision.Get_x_bot() == bot.Get_x_bot() && bot_colision.Get_y_bot() == bot.Get_y_bot())
                        {
                            bot.Move_Image(4);
                            botTimers[bot.Get_position_list_indicator()].Stop();
                            All_Objects_For_Colisions.Remove(bot);

                            botTimers[bot_colision.Get_position_list_indicator()].Stop();
                            bot_colision.Move_Image(4);
                            All_Objects_For_Colisions.Remove(bot_colision);

                            Check_Killed_Bots();
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
            bool are_bots_not_alive = false;
            foreach (Object objecto in All_Objects_For_Colisions)
            {
                try 
                {
                    MotorcycleBot motorcycleBot = (MotorcycleBot)objecto;
                    are_bots_not_alive = true;
                }
                catch
                {
                    continue;
                }
            }
            if (are_bots_not_alive == false)
            {
                timer_player.Stop();
                Spaw_items_powerups.Stop();
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
        private void Fuel_Check_And_Player_Movement(object sender, EventArgs e)
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
                Spaw_items_powerups.Stop();
                foreach (System.Windows.Forms.Timer timer in botTimers)
                {
                    timer.Stop();
                }
                MessageBox.Show("Game Over! You got out of fuel.");
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

            //Implementation of boxes por Items and Powerups
            for (int j = 0; j < 50; j++)
            {
                PictureBox pictureBox = new PictureBox();
                Boxes_for_items_and_powerups.Add(pictureBox);
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
                MotorcycleBot bot = new MotorcycleBot(0, 0, 75, images_bots, box_grid, 5 * k, 7 * k, 1, k); 
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
        private void Spaw_Consumables(object sender, EventArgs e)
        {
            Spawn_items(2*ref_it_pu + 1);
            Spawn_PowerUps(2*ref_it_pu);
            ref_it_pu++;
        }
        public void Spawn_items(int num)
        {
            bool isValidSpawnLocation;
            int item = random.Next(1, 4);
            int x_it;
            int y_it;

            do
            {
                isValidSpawnLocation = true; // Suponemos que la posición es válida

                x_it = random.Next(0, 24);
                y_it = random.Next(0, 16);

                // Verificar colisiones con otros objetos
                foreach (Object objecto in All_Objects_For_Colisions)
                {
                    try
                    {
                        Motorcycle motorcycle = objecto as Motorcycle;
                        if (motorcycle != null && motorcycle.Get_x_player() == x_it && motorcycle.Get_y_player() == y_it)
                        {
                            isValidSpawnLocation = false;
                            break;
                        }

                        MotorcycleBot bot = objecto as MotorcycleBot;
                        if (bot != null && bot.Get_x_bot() == x_it && bot.Get_y_bot() == y_it)
                        {
                            isValidSpawnLocation = false;
                            break;
                        }

                        Estela estela = objecto as Estela;
                        if (estela != null && estela.Get_X_est() == x_it && estela.Get_Y_est() == y_it)
                        {
                            isValidSpawnLocation = false;
                            break;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

                // Verificar colisiones con ítems y power-ups existentes
                foreach (Object objectos in All_items_and_powerups)
                {
                    try
                    {
                        item_PU obj = objectos as item_PU;
                        if (obj != null && obj.Get_x() == x_it && obj.Get_y() == y_it)
                        {
                            isValidSpawnLocation = false;
                            break;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

            } while (!isValidSpawnLocation); // Continuar buscando una posición válida

            // Si se encuentra una posición válida, crear y agregar el ítem
            item_PU items = new item_PU(item, Boxes_for_items_and_powerups[num]);
            items.Change_Position(nodes, x_it, y_it);
            All_items_and_powerups.Add(items);
        }
        public void Spawn_PowerUps(int num)
        {
            bool isValidSpawnLocation;
            int pu = random.Next(4, 6);
            int x_pu;
            int y_pu;

            do
            {
                isValidSpawnLocation = true; // Suponemos que la posición es válida

                x_pu = random.Next(0, 24);
                y_pu = random.Next(0, 16);

                // Verificar colisiones con otros objetos
                foreach (Object objecto in All_Objects_For_Colisions)
                {
                    try
                    {
                        Motorcycle motorcycle = objecto as Motorcycle;
                        if (motorcycle != null && motorcycle.Get_x_player() == x_pu && motorcycle.Get_y_player() == y_pu)
                        {
                            isValidSpawnLocation = false;
                            break;
                        }

                        MotorcycleBot bot = objecto as MotorcycleBot;
                        if (bot != null && bot.Get_x_bot() == x_pu && bot.Get_y_bot() == y_pu)
                        {
                            isValidSpawnLocation = false;
                            break;
                        }

                        Estela estela = objecto as Estela;
                        if (estela != null && estela.Get_X_est() == x_pu && estela.Get_Y_est() == y_pu)
                        {
                            isValidSpawnLocation = false;
                            break;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

                // Verificar colisiones con ítems y power-ups existentes
                foreach (Object objectos in All_items_and_powerups)
                {
                    try
                    {
                        item_PU obj = objectos as item_PU;
                        if (obj != null && obj.Get_x() == x_pu && obj.Get_y() == y_pu)
                        {
                            isValidSpawnLocation = false;
                            break;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

            } while (!isValidSpawnLocation); // Continuar buscando una posición válida

            // Si se encuentra una posición válida, crear y agregar el power-up
            item_PU powerup = new item_PU(pu, Boxes_for_items_and_powerups[num]);
            powerup.Change_Position(nodes, x_pu, y_pu);
            All_items_and_powerups.Add(powerup);
        }
        private void Create_Stels_For_bots(SingleLinkedForGame list, int num_bot, List<PictureBox> estelas_boxes)
        {
            if (num_bot == 1)
            {
                for (int i = 4; i < 7; i++)
                {
                    estela = new Estela(estelas_boxes[i], 0, 0);
                    list_bots[0].Add_Stels();
                    bot1.Add(estela);
                    All_Objects_For_Colisions.Add(estela);
                }
            }

            else if (num_bot == 2)
            {
                for (int i = 8; i < 11; i++)
                {
                    estela = new Estela(estelas_boxes[i], 0, -0);
                    list_bots[1].Add_Stels();
                    bot2.Add(estela);
                    All_Objects_For_Colisions.Add(estela);
                }
            }

            else if (num_bot == 3)
            {
                for (int i = 12; i < 15; i++)
                {
                    estela = new Estela(estelas_boxes[i], 0, 0);
                    list_bots[2].Add_Stels();
                    bot3.Add(estela);
                    All_Objects_For_Colisions.Add(estela);
                }
            }
            else
            {
                for (int i = 16; i < 19; i++)
                {
                    estela = new Estela(estelas_boxes[i], 0, 0);
                    list_bots[3].Add_Stels();
                    bot4.Add(estela);
                    All_Objects_For_Colisions.Add(estela);
                }
            }
        }
        public void Create_item_and_powerups_list()
        {   
            for (int i = 0; i < 5; i++)
            {
                PriorityQueue queue = new PriorityQueue();
                List_Items_All_Characters.Add(queue);

                ArrayStack stack = new ArrayStack(15);
                List_Power_Ups_All_Characters.Add(stack);

                //For both index 4 represents the lists for player
                //the other ones are for bots because of their "atribute" "int position_list_indicator" which is from 0 to 3,
                // it depends on the but number.
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
