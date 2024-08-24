using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Windows_Forms_Attempt
{
    public partial class Form1 : Form
    {
        // Main Objects
        private PictureBox pictureBox1; // Box for player
        private Motorcycle motorcycle; // Player's object
        private SingleLinkedForGame player; //Players SingleLinkedList
        private MotorcycleBot bot; // Generic object for creating list of bots
        private ArrayGrid grid; // Object grid for representing the map and leading to the movement of bots and player

        // Instance Lists
        private List<MotorcycleBot> list_bots; // List of bot instances
        private List<PictureBox> Boxes_for_items_and_powerups = new List<PictureBox>(); // List that contains all PictureBoxes of all items and powerups
        private List<PictureBox> Boxes_ReDrops = new List<PictureBox>(); // List that contains all PictureBoxes of all items and powerups
        private List<item_PU> All_items_and_powerups = new List<item_PU>(); // List with all items and powerups presented
        private List<Object> All_Objects_For_Colisions = new List<object>(); // All objects that can kill player or bots
        private List<PriorityQueue> List_Items_All_Characters = new List<PriorityQueue>(); // List with all item queue lists for each bot and the player
        private List<ArrayStack> List_Power_Ups_All_Characters = new List<ArrayStack>(); // List with all power ups for each bot and the player
        private List<ArrayGrid.Node> nodes; // Array with nodes for creating map

        // Bot Specific
        private SingleLinkedForGame bot1 = new SingleLinkedForGame(); // bot1 LinkedList
        private SingleLinkedForGame bot2 = new SingleLinkedForGame(); // bot2 LinkedList
        private SingleLinkedForGame bot3 = new SingleLinkedForGame(); // bot3 LinkedList
        private SingleLinkedForGame bot4 = new SingleLinkedForGame(); // bot4 LinkedList

        // Game Variables
        private int ref_it_pu = 0; // Reference for using PictureBox when spawning item or power up
        private int current_shield = 0; // Number of shields consumed that haven't been used
        private int executionsPerSecond; // Player's speed
        private int rest_fuel; // Counter for fuel depletion

        // Timers
        private System.Windows.Forms.Timer timer_player; // Timer for player's speed
        private System.Windows.Forms.Timer Spaw_items_powerups; // Timer for spawning new objects
        private System.Windows.Forms.Timer consume_wait; // Timer to prevent player from consuming within 2 seconds after an item has been consumed
        private System.Windows.Forms.Timer[] botTimers; // List of timers for the bots

        // Game Logic
        private bool can_be_killed = true; // Boolean related to current shield; when true, player can be killed
        private bool can_consume = true; // Related to Timer consume wait; timer sets this bool to true after 2 seconds

        // Bot Movement and Fuel
        private List<int> lista_actual_move_bots = new List<int>(); // List of integers relating how many times a bot has moved
        private List<int> bots_random_distance = new List<int>(); // List of random distances for bots
        private List<int> current_bots_nedded_rest_fuel = new List<int>(); // Number that, when reaching a certain value, will decrease the fuel in each bot

        // General Objects
        private Estela estela; // Generic object for adding to bots and player

        // Extras
        private Random random = new Random(); // Generic random variable
        private Button quit_Button; // Button to exit the game

        // Labels
        private Label current_fuel; // Label with current fuel
        private Label current_speed; // Label with current speed
        private Label current_shield_num; // Label with current number of shields


        //SECTIONS 1//
        // INITILIZERS//
        public Form1() // Start
        {
            Create_item_and_powerups_list();
            InitializeComponent_1();
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
            UpdateTimerInterval(); // Updates Plyers inteval
            timer_player.Tick += new EventHandler(Fuel_Check_And_Player_Movement);
            timer_player.Start();

            Spaw_items_powerups = new System.Windows.Forms.Timer();
            Spaw_items_powerups.Interval = 9000;
            Spaw_items_powerups.Tick += new EventHandler(Spaw_Consumables);
            Spaw_items_powerups.Start();

            consume_wait = new System.Windows.Forms.Timer();
            consume_wait.Interval = 2000;
            consume_wait.Tick += new EventHandler(Can_Consume_Check);

            // Create and initialize bot timers.
            int botCount = 4;
            botTimers = new System.Windows.Forms.Timer[botCount];

            for (int i = 0; i < botCount; i++)
            {
                int bot_speed = random.Next(299, 401);
                botTimers[i] = new System.Windows.Forms.Timer();
                botTimers[i].Interval = bot_speed; 
                list_bots[i].SetSpeed(bot_speed);
                int botIndex = i; // Capture variable in local context
                botTimers[i].Tick += (sender, e) => Set_bots_movement_Fuel_check(sender, e, botIndex);
                botTimers[i].Start();
            }
            
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(Movement_Key_detector_Consume_Items);

            InitializeControls();
        }
        private void InitializeComponent_1() //Main loop
        {
            // Form Settings
            this.ClientSize = new System.Drawing.Size(1700, 800);
            this.Name = "Form1";
            this.Text = "Trone Game";

            // Initialize PictureBox by player
            this.pictureBox1 = new PictureBox();
            this.pictureBox1.Size = new Size(50, 50);
            this.pictureBox1.BackColor = Color.Transparent; // No background color
            this.Controls.Add(this.pictureBox1);

            list_bots = new List<MotorcycleBot>(); // List of bots for future iterating them

            List<PictureBox> box_list_bots = new List<PictureBox>(); //List for boxes to be placed over the bots

            List<PictureBox> estelas_boxes = new List<PictureBox>(); //List for boxes to be placed over the bots

            //Implementation of bot boxes
            for (int j = 0; j < 4; j++) 
            {
                PictureBox pictureBox = new PictureBox();
                box_list_bots.Add(pictureBox);
                pictureBox.Size = new Size(50, 50);
                pictureBox.BackColor = Color.Transparent; 
                this.Controls.Add(pictureBox); 
            }

            //Implementation of boxes by Items and Powerups
            for (int j = 0; j < 50; j++)
            {
                PictureBox pictureBox = new PictureBox();
                Boxes_for_items_and_powerups.Add(pictureBox);
                pictureBox.Size = new Size(50, 50);
                pictureBox.BackColor = Color.Transparent; 
                this.Controls.Add(pictureBox);
            } 

            for (int j = 0; j < 50; j++)
            {
                PictureBox pictureBox = new PictureBox();
                Boxes_ReDrops.Add(pictureBox);
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
            // Create the Motorcycle instance
            player = new SingleLinkedForGame();
            this.motorcycle = new Motorcycle(executionsPerSecond, 0, 90, images_player, this.pictureBox1, 1, List_Items_All_Characters[4], List_Power_Ups_All_Characters[4]);
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
                MotorcycleBot bot = new MotorcycleBot(0, 0, 75, images_bots, box_grid, 5 * k, 7 * k, 1, k, List_Items_All_Characters[k], List_Power_Ups_All_Characters[k]); 
                list_bots.Add(bot);
                All_Objects_For_Colisions.Add(bot);
                Organize_Bots_In_List(bot, k, estelas_boxes);
            }
        }
        private void InitializeControls() // Initialize all textboxes and buttons
        {
            
            int fuel = this.motorcycle.Get_fuel();
            int speed_act = this.motorcycle.GetSpeed();
        
            TextBox reference = new TextBox();
            reference.ReadOnly = true;
            reference.Size = new Size(1,1);
            reference.Location = new System.Drawing.Point(1200,1200);
            this.Controls.Add(reference);

            // Initialize Button for exiting the game.
            quit_Button = new Button();
            quit_Button.Text =  "Exit the game";
            quit_Button.Size = new Size(100,100);
            quit_Button.Location = new Point(1200, 650);
            quit_Button.Click += new EventHandler(Quit_Game);
            this.Controls.Add(quit_Button);
            
            // Configure the message box
            this.current_fuel = new Label();
            current_fuel.Text = "The current fue is " + fuel + ".";
            current_fuel.Size = new Size(150,50);
            current_fuel.Location = new System.Drawing.Point(1200,550);
            this.Controls.Add(current_fuel);

            this.current_speed = new Label();
            current_speed.Text = "Actual speed is " + executionsPerSecond + " movements per second.";
            current_speed.Size = new Size(270,50);
            current_speed.Location = new System.Drawing.Point(1200,450);
            this.Controls.Add(current_speed);

            this.current_shield_num = new Label();
            current_shield_num.Text = "The amount of shields is " + current_shield + ".";
            current_shield_num.Size = new Size(150,50);
            current_shield_num.Location = new System.Drawing.Point(1200,350);
            this.Controls.Add(current_shield_num);
        }        
        private void CreateGridDisplay() //Creates Grid using Nodes
        {
            grid = new ArrayGrid(24, 16);
            nodes = grid.GetNodes();
            int nodeSize = 50; // Size for each PicturBox
            foreach (ArrayGrid.Node node in nodes)
            {
                PictureBox box = new PictureBox();
                box.Size = new Size(nodeSize, nodeSize);
                box.Location = new Point(node.GetX() * nodeSize, node.GetY() * nodeSize);
                box.BackColor = Color.Transparent;
                box.BorderStyle = BorderStyle.FixedSingle;
                this.Controls.Add(box);

                node.Box = box; // AsosiatePpictureBox with node
            }
        }
        //END SECTION 1//


        //SECTION 2//
        //ANALICE COLLISIONS WITH PLAYER, BOTS, CONSUMABLES//
        private void Colisions_With_Consumable(Object npc, int indicator) //Detect collisions with items or powerups
        {
            try
            {
                Motorcycle motorcycle = (Motorcycle)npc;
                int x = motorcycle.Get_x_player();
                int y = motorcycle.Get_y_player();
                foreach (item_PU item_PU in All_items_and_powerups)
                {
                    int num = item_PU.Get_value();
                    if (num < 4 && item_PU.Get_x() == x && item_PU.Get_y() == y)
                    {
                        if (num == 3) //Colision with bomb
                        {
                            if (current_shield > 0 && can_be_killed)
                            {
                                current_shield--;
                                can_be_killed = false;
                                Update_Shield();
                            }
                            else if (can_be_killed && current_shield == 0)
                            {
                                timer_player.Stop();
                                Spaw_items_powerups.Stop();
                                foreach (System.Windows.Forms.Timer timer in botTimers)
                                {
                                    timer.Stop();
                                }
                                MessageBox.Show("Game Over! You exploded.");  
                            }                                
                            else
                            {
                                can_be_killed = true;
                                Colisions_With_Consumable(this.motorcycle, 4);
                            }
                        }
                        else
                        {
                            List_Items_All_Characters[indicator].Enqueue(item_PU);
                            item_PU.Change_Position(nodes,-100,-100);
                            Show_Items_On_grid();
                        }

                    }
                    else if (item_PU.Get_x() == x && item_PU.Get_y() == y)
                    {
                        List_Power_Ups_All_Characters[indicator].Push(item_PU);
                        item_PU.Change_Position(nodes,-100,-100);
                        Show_PowerUps_On_grid();
                    }
                }
            }
            catch
            {
                MotorcycleBot bot = (MotorcycleBot)npc;
                int x = bot.Get_x_bot();
                int y = bot.Get_y_bot();
                int j = All_items_and_powerups.Count();
                for (int i = 0; i< j; i++)
                {
                    item_PU item_PU = All_items_and_powerups[i];
                    int num = item_PU.Get_value();
                    if (num < 4 && item_PU.Get_x() == x && item_PU.Get_y() == y)
                    {
                        if (num == 3) //colission with bomb
                        {
                            int ind = bot.Get_position_list_indicator();
                            botTimers[ind].Stop();
                            bot.Move_Image(4);
                            All_Objects_For_Colisions.Remove(bot);
                            Check_Killed_Bots();
                            Spawn_Obj_Dead_Bot(bot);
                        }
                        else
                        {
                            List_Items_All_Characters[indicator].Enqueue(item_PU);
                            item_PU.Change_Position(nodes,-100,-100);
                        }
                    }
                    else if (item_PU.Get_x() == x && item_PU.Get_y() == y)
                    {
                        List_Power_Ups_All_Characters[indicator].Push(item_PU);
                        item_PU.Change_Position(nodes,-100,-100);
                    }
                }
            }
        }
        private void Analice_Colisions(object ob, SingleLinkedForGame list) //Detection of a bot or player with all the other objects that can kill a character.
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
                                if (current_shield > 0 && can_be_killed)
                                {
                                    current_shield--;
                                    can_be_killed = false;
                                    Update_Shield();
                                }
                                else if (can_be_killed && current_shield == 0)
                                {
                                    timer_player.Stop();
                                    Spaw_items_powerups.Stop();
                                    foreach (System.Windows.Forms.Timer timer in botTimers)
                                    {
                                        timer.Stop();
                                    }
                                    MessageBox.Show("Game Over! You collided with a curious object.");  
                                }                                
                                else
                                {
                                    can_be_killed = true;
                                    Analice_Colisions(moto, list);
                                }

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

                                if (current_shield > 0 && can_be_killed)
                                {
                                    current_shield--;
                                    can_be_killed = false;
                                    Update_Shield();
                                }
                                else if (can_be_killed && current_shield == 0)
                                {
                                    timer_player.Stop();
                                    Spaw_items_powerups.Stop();
                                    foreach (System.Windows.Forms.Timer timer in botTimers)
                                    {
                                        timer.Stop();
                                    }
                                    MessageBox.Show("Game Over! You collided with a curious object.");   
                                }
                                else
                                {
                                    can_be_killed = true;
                                }
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
                            Spawn_Obj_Dead_Bot(bot);
                        }
                    }
                    catch (InvalidCastException)
                    {
                        if (objecto is MotorcycleBot bot_colision)
                        {
                            if (bot_colision.Get_x_bot() == bot.Get_x_bot() && bot_colision.Get_y_bot() == bot.Get_y_bot())
                            {
                                bot.Move_Image(4);
                                botTimers[bot.Get_position_list_indicator()].Stop();
                                All_Objects_For_Colisions.Remove(bot);

                                botTimers[bot_colision.Get_position_list_indicator()].Stop();
                                bot_colision.Move_Image(4);
                                All_Objects_For_Colisions.Remove(bot_colision);
                                Spawn_Obj_Dead_Bot(bot_colision);
                                Spawn_Obj_Dead_Bot(bot);

                                Check_Killed_Bots();
                            }
                        }
                    }

                }

            }
        }
        //END SECTION 2//


        //SECTION 3//
        //PLAYER AND BOTS UPDATE/CREATION STELS AND MOVEMENTS && FUEL CHECK && CREATION OF ITEMS PU LISTS FOR ALL //
        private void Create_Stels_For_bots(SingleLinkedForGame list, int num_bot, List<PictureBox> estelas_boxes) //Creates stels for the presented bot.
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
            Colisions_With_Consumable(bot, bot.Get_position_list_indicator());

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
        private void Fuel_Check_And_Player_Movement(object sender, EventArgs e)
        {
            // Move the motorcycle automatically
            Check_Killed_Bots();
            this.motorcycle.Change_Position(nodes);
            Analice_Colisions(this.motorcycle, player);
            Colisions_With_Consumable(this.motorcycle, 4);
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
        private void Movement_Key_detector_Consume_Items(object sender, KeyEventArgs e) //if any key is pressed, changes players direction
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
                case Keys.F: //Consume item
                    try
                    {
                        PriorityQueue list_items = List_Items_All_Characters[4];
                        item_PU top = list_items.Front();
                        if (top.Get_value() == 1 && can_consume)
                        {
                            if (this.motorcycle.Get_fuel() < 100) //Check if fuel tank is full
                            {
                                Add_fuel();
                                Show_Items_On_grid();
                                can_consume = false;
                                consume_wait.Start();
                                list_items.Dequeue();
                            }

                        }
                        else if (top.Get_value() == 2 && can_consume)
                        {
                            Add_Random_Stels();
                            Show_Items_On_grid();
                            can_consume = false;
                            consume_wait.Start();
                            list_items.Dequeue();
                        }
                        Clear_PictureBoxes_For_Items();
                        Show_Items_On_grid();
                    }
                    catch
                    {
                        
                    }
                    break;
                case Keys.Q: //Use next powerup
                    try
                    {
                        ArrayStack list_items = List_Power_Ups_All_Characters[4];
                        item_PU top = list_items.Top();
                        if (top.Get_value() == 4)
                        {
                            Use_PU_Speed();
                            Show_PowerUps_On_grid();
                        }
                        else if (top.Get_value() == 5)
                        {
                            Use_PU_Shield();
                            Show_PowerUps_On_grid();
                        }
                        list_items.Pop();
                        Clear_PictureBoxes_For_PU();
                        Show_PowerUps_On_grid();
                    }
                    catch
                    {
                        
                    }
                    break;
                case Keys.Y: //Change PowerUps Order
                    ArrayStack list_items_x = List_Power_Ups_All_Characters[4];
                    Clear_PictureBoxes_For_PU();
                    list_items_x.Set_bottom_to_top();
                    Clear_PictureBoxes_For_PU();
                    Show_PowerUps_On_grid();

                    break;
                case Keys.X:
                    timer_player.Stop();
                    break;
                case  Keys.Escape:
                    Application.Exit();
                    break;
            }
        }
        private void Organize_Bots_In_List(MotorcycleBot bot , int indicator, List<PictureBox> estelas_boxes ) //During the code, bots are related to a number which is the SingleLinkedList bot(1 to 4)
        //this function relates the bots with their numbers.
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
        public void Create_item_and_powerups_list() //creates the ArrayStack list and the Priority Queue for each bot and player.
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
        //END SECTION 3//


        //SECTION 4//
        //UPDATE TEXTS FOR PLAYER CHARACTERISTICS AND UPDATE TIMER INTERVAL//
        public void UpdateTimerInterval() //This function changes directly the execution per second in terms of movement. 
        {
            if (executionsPerSecond > 0)
            {
                timer_player.Interval = 1000 / executionsPerSecond; // Update interval to match new speed
            }
        }
        public void Update_Spped() //Updates label of speed
        {
            current_speed.Text = "Actual speed is " + executionsPerSecond + " movements per second.";
        }
        public void Update_Shield() //Updates label of shields.
        {
            current_shield_num.Text = "The amount of shields is " + current_shield + ".";
        }
        
        //Notice that fuel Label is already been updated on function Fuel_Check_And_Player_Movement()

        //END SECTION 4//


        //SECTIONS 5//
        // CREATION OF PLAYERS ITEMS AND POWERUPS ON GRID//
        public void Show_Items_On_grid() //Show the character current items of PriorityQueue on grid.
        {
        
            PriorityQueue list_items = List_Items_All_Characters[4];
            int colocator = 0;

            // Clean items PictureBox
            Clear_PictureBoxes_For_Items();

            string[] items_PU_images = new string[]
            {
                "Imagenes/Items/fuel_tank.png",
                "Imagenes/Items/add_stels.png",
                "Imagenes/Items/bomba.png",
                "Imagenes/PowerUps/speed_up.png",
                "Imagenes/PowerUps/shield.png"
            };

            int count = list_items.Count();
            for (int i = 0; i < count; i++)
            {
                item_PU item_PU = list_items.GetByIndex(i);

                int num = item_PU.Get_value();
                string image_str = items_PU_images[num - 1];
                Create_Image_for_Items_On_grid(image_str, colocator, 1500);
                colocator = Increase_colocator(colocator);
            }

        }
        private void Clear_PictureBoxes_For_Items() //All PictureBox related to players items is removed for recreated later.
        {
            // Delete PictureBoxes that have names starting with "item_"
            foreach (Control control in this.Controls)
            {
                if (control is PictureBox pictureBox && pictureBox.Name.StartsWith("item_"))
                {
                    this.Controls.Remove(control);
                    pictureBox.Dispose(); // Release resources
                }
            }
        }
        public void Create_Image_for_Items_On_grid(string image_str, int colocator, int pos) //Displace the image of the players item on the grid
        {
            PictureBox box = new PictureBox
            {
                Size = new Size(25, 25),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Location = new Point(pos, colocator * 25 + 25),
                BackColor = Color.Transparent,
                Image = System.Drawing.Image.FromFile(image_str),
                Name = $"item_{colocator}" // Asign a specifica name for later deleting
            };

            this.Controls.Add(box);
        }
        public void Show_PowerUps_On_grid() //Show the players current powerups of ArrayStack on grid.
        {
            ArrayStack list_stack = List_Power_Ups_All_Characters[4];
            int colocator = 0;

            Clear_PictureBoxes_For_PU();

            string[] items_PU_images = new string[]
            {
                "Imagenes/Items/fuel_tank.png",
                "Imagenes/Items/add_stels.png",
                "Imagenes/Items/bomba.png",
                "Imagenes/PowerUps/speed_up.png",
                "Imagenes/PowerUps/shield.png"
            };

            int count = list_stack.Count();
            for (int i = 0; i < count; i++)
            {
                item_PU item_PU = list_stack.GetIndex(i);
                int num = item_PU.Get_value();
                string image_str = items_PU_images[num - 1];
                Create_Image_for_PU_On_grid(image_str, colocator, 1400);
                colocator = Increase_colocator(colocator);
            }
        }
        public void Create_Image_for_PU_On_grid(string s, int n, int x) //Displace the images of players powerups on grid.
        {
                PictureBox box = new PictureBox
            {
                Size = new Size(25, 25),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Location = new Point(x, n * 25 + 25),
                BackColor = Color.Transparent,
                Image = System.Drawing.Image.FromFile(s),
                Name = $"PU_{n}" // Asign a specifica name for later deleting
            };

            this.Controls.Add(box);
        }
        public void Clear_PictureBoxes_For_PU() //All PictureBox related to players powerups is removed for recreated later
        {
            // Delete PictureBoxes that have names starting with "PU_"
            foreach (Control control in this.Controls)
            {
                if (control is PictureBox pictureBox && pictureBox.Name.StartsWith("PU_"))
                {
                    this.Controls.Remove(control);
                    pictureBox.Dispose(); // Release resources
                }
            }
        }
        public int Increase_colocator(int num) //A function that adds 1 to the input number.
        {
            return num + 1;
        }
        //END SECTION 5//

        //SECTION 6//
        //CONSUME OBJECTS//
        public void Add_fuel() //Increase the fuel value. If the new value is greater than 100, fuel is set to 100.
        //This is programed because 100 must be maximun fuel.
        {
            int actual_fuel = this.motorcycle.Get_fuel();
            int addition = random.Next(10, 15);
            actual_fuel = actual_fuel + addition;
            if (actual_fuel > 100)
            {
                this.motorcycle.Set_fuel(100);
            }
            else
            {
                this.motorcycle.Set_fuel(actual_fuel);
            }
        }
        public void Add_Random_Stels() //Adds a random from 1 to 4 stels.
        {
            int aumento = random.Next(1,5);
            for (int i = 0; i < aumento; i++)
            {
                PictureBox pictureBox = new PictureBox();
                pictureBox.Size = new Size(50, 50);
                pictureBox.BackColor = Color.Transparent;
                this.Controls.Add(pictureBox);
                pictureBox.BringToFront(); // Ensure the PictureBox is in front
                estela = new Estela(pictureBox, this.motorcycle.Get_x_player(), this.motorcycle.Get_y_player());
                this.motorcycle.Add_Stels();
                player.Add(estela);
                All_Objects_For_Colisions.Add(estela);
            }

        }
        public void Use_PU_Shield() //Increases shield number and changes players colors.
        {
            string[] images_player = new string[]
            {
                "Imagenes/jugador/Purple/moto_jugador_derecha_purple.png",
                "Imagenes/jugador/Purple/moto_jugador_izquierda_purple.png",
                "Imagenes/jugador/Purple/moto_jugador_arriba_purple.png",
                "Imagenes/jugador/Purple/moto_jugador_abajo_purple.png"
            };
            current_shield++;
            Update_Shield();
            this.motorcycle.Set_New_Image_List(images_player);
        }
        public void Use_PU_Speed() //Increases players speed and changes its colors.
        {
            string[] images_player = new string[]
            {
                "Imagenes/jugador/Yellow/moto_jugador_derecha_yellow.png",
                "Imagenes/jugador/Yellow/moto_jugador_izquierda_yellow.png",
                "Imagenes/jugador/Yellow/moto_jugador_arriba_yellow.png",
                "Imagenes/jugador/Yellow/moto_jugador_abajo_yellow.png"
            };
            int add_speed = random.Next(1, 3);
            int actual_speed = executionsPerSecond;
            executionsPerSecond = actual_speed + add_speed;
            Update_Spped();
            this.motorcycle.Set_New_Image_List(images_player);
        }
        public void Can_Consume_Check(object sender, EventArgs e) //Related to timer of consume. This is the functions that is run 2 seconds after consuming an item.
        {
            can_consume = true;
            consume_wait.Stop();
        }        
        //END SECTION 6//

        //SECTION 7//
        //SPAWN OBJECTS AND RANDOM DROP WHEN BOT DIES//
        private void Spaw_Consumables(object sender, EventArgs e) //Starts  with the players display of items and powerups.
        {
            Spawn_items(2*ref_it_pu + 1);
            Spawn_PowerUps(2*ref_it_pu);
            ref_it_pu++;
        }
        public void Spawn_items(int num) //Display items.
        {
            bool isValidSpawnLocation;
            int item = random.Next(1, 4);
            int x_it;
            int y_it;

            do
            {
                isValidSpawnLocation = true; //We assume that the position is valid

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

                // Check for collisions with existing items and power-ups
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

            } while (!isValidSpawnLocation); // Continue searching for a valid position

            // If a valid position is found, create and add the item
            item_PU items = new item_PU(item, Boxes_for_items_and_powerups[num]);
            items.Change_Position(nodes, x_it, y_it);
            All_items_and_powerups.Add(items);
        }
        public void Spawn_PowerUps(int num) //Display powerups
        {
            bool isValidSpawnLocation;
            int pu = random.Next(4, 6);
            int x_pu;
            int y_pu;

            do
            {
                isValidSpawnLocation = true; //We assume that the position is valid

                x_pu = random.Next(0, 24);
                y_pu = random.Next(0, 16);

                // Check collisions with other objects
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

                // Check for collisions with existing items and power-ups
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

            } while (!isValidSpawnLocation); // Continue searching for a valid position

            // If a valid position is found, create and add the power-up
            item_PU powerup = new item_PU(pu, Boxes_for_items_and_powerups[num]);
            powerup.Change_Position(nodes, x_pu, y_pu);
            All_items_and_powerups.Add(powerup);
        }
        public void Spawn_Obj_Dead_Bot(MotorcycleBot bot) //Whenever a bot dies, this function runs so their objects (items and powerups) respawn on grid
        {
            PriorityQueue list_items = bot.Retunr_List_Items();
            ArrayStack list_PU = bot.Retunr_List_PU();

            int count = list_items.Count();
            for (int i = 0; i < count; i++)
            {
                item_PU item_PU = list_items.GetByIndex(i);
                Random_Drop_Items_Or_PU(item_PU);
                list_items.Dequeue();
            }

            int count2 = list_PU.Count();
            for (int i = 0; i < count2; i++)
            {
                item_PU item_PU = list_PU.GetIndex(i);
                Random_Drop_Items_Or_PU(item_PU);
                list_PU.Pop();
            }
        }
        public void Random_Drop_Items_Or_PU(item_PU obj) //Generate the random drop of dead bot items. Logic for not spawning on occupied place
        {
            int x = random.Next(0, 24);
            int y = random.Next(0, 16);
            bool is_ValidSpawnLocation_for_drop = true; // We assume that the position is valid
            do
            {
                // Check for collisions with other objects
                foreach (Object objecto in All_Objects_For_Colisions)
                {
                    try
                    {
                        Motorcycle motorcycle = objecto as Motorcycle;
                        if (motorcycle != null && motorcycle.Get_x_player() == x && motorcycle.Get_y_player() == y)
                        {
                            is_ValidSpawnLocation_for_drop = false;
                            break;
                        }

                        MotorcycleBot bot = objecto as MotorcycleBot;
                        if (bot != null && bot.Get_x_bot() == x && bot.Get_y_bot() == y)
                        {
                            is_ValidSpawnLocation_for_drop = false;
                            break;
                        }

                        Estela estela = objecto as Estela;
                        if (estela != null && estela.Get_X_est() == x && estela.Get_Y_est() == y)
                        {
                            is_ValidSpawnLocation_for_drop = false;
                            break;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
                // Check for collisions with existing items and power-ups
                foreach (Object objectos in All_items_and_powerups)
                {
                    try
                    {
                        item_PU objec = objectos as item_PU;
                        if (objec != null && objec.Get_x() == x && objec.Get_y() == y)
                        {
                            is_ValidSpawnLocation_for_drop = false;
                            break;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            } while (!is_ValidSpawnLocation_for_drop); // Continue searching for a valid position

            if (obj != null)
            {
                item_PU items = new item_PU(obj.Get_value(), Boxes_for_items_and_powerups[GetFirstUnoccupiedIndex()]);
                items.Change_Position(nodes, x, y);
                All_items_and_powerups.Add(items);
            }

        }
        public int GetFirstUnoccupiedIndex() //For spawning the new items from dead bots, it looks for the next PictureBox (on the respawn picturebox list)
        //So a new blanc picturebox is used.
        {
            for (int i = 0; i < Boxes_ReDrops.Count; i++)
            {
                if (Boxes_ReDrops[i].Image == null) // If there is no associated image
                {
                    return i; // Returns the first unoccupied index found
                }
            }

            return -1; // Returns -1 if no unoccupied index is found
        }
        //END SECTION 7//

        //SECTION 8//
        //ENDGAME AND CHECK FOR KILLED BOTS
        public void Quit_Game(object sender, EventArgs e) //Exit game
        {
            Application.Exit();
        }
        private void Check_Killed_Bots() //Check if every bot has been killed so player can win the game.
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
        //END SECTION 8//
    }
}
