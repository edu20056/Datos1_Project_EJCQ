using System.Windows.Forms.VisualStyles;

namespace Windows_Forms_Attempt
{
    public class MotorcycleBot
    {
        private int speed;
        private int stels;
        private PictureBox box;
        private string[] list_images;
        
        private PriorityQueue bot_itmes;
        private ArrayStack bot_powerups;

        private int x;
        private int y;
        private int move_indicator;

        private int fuel;
        private int position_list_indicator; //Indicates the number of position on SingledLinkedListForPlayers.

        public MotorcycleBot(int speed, int stels, int fuel, string[] list_images, PictureBox box, int x, int y, int move_indicator, int position_list_indicator, PriorityQueue bot_itmes, ArrayStack bot_powerups )
        {
            this.speed = speed;
            this.stels = stels;
            this.fuel = fuel;
            this.list_images = list_images;
            this.box = box;
            this.x = y;
            this.y = x;
            this.bot_itmes = bot_itmes;
            this.bot_powerups = bot_powerups;
            this.move_indicator = move_indicator;
            this.position_list_indicator = position_list_indicator;
            Create_Box();
        }
        public PriorityQueue Retunr_List_Items()
        {
            return this.bot_itmes;
        }

        public ArrayStack Retunr_List_PU()
        {
            return bot_powerups;
        }

        public void Add_Stels()
        {
            this.stels++;
        }

        public void Set_Fuel(int newfuel)
        {
            this.fuel = newfuel;
        }

        public int Get_Fuel()
        {
            return this.fuel;
        }

        public int Get_position_list_indicator()
        {
            return this.position_list_indicator;
        }
         public void SetSpeed(int newSpeed)
        {
            if (newSpeed > 0)
            {
                this.speed = newSpeed;
            }
        }
        public PictureBox Get_Box()
        {
            return this.box;
        }
        public int Get_Move_Indicator()
        {
            return this.move_indicator;
        }
        public void Create_Box()
        {
            if (this.box != null)
            {
                this.box.Location = new System.Drawing.Point(x * 50, y * 50);
                this.box.Name = "Bot";
                this.box.Size = new System.Drawing.Size(50, 50);
                this.box.SizeMode = PictureBoxSizeMode.StretchImage;
                this.box.Image = System.Drawing.Image.FromFile(list_images[move_indicator]);
                this.box.BackColor = Color.Transparent; // Ensure no background color
            }
        }

        public void Move_Image(int index_image)
        {
            string new_image = list_images[index_image];
            this.move_indicator = index_image;
            this.box.Image = System.Drawing.Image.FromFile(new_image);
        }

        public int Get_x_bot()
        {
            return this.x;
        }

        public int Get_y_bot()
        {
            return this.y;
        }
        public int Current_Image_Direction()
        {
            return this.move_indicator;
        }

        public void Change_Position(List<ArrayGrid.Node> list)
        {
            ArrayGrid.Node current = list.Find(node => node.GetX() == this.x && node.GetY() == this.y);

            if (current == null)
            {
                Console.WriteLine("No se encontró el nodo actual.");
                return;
            }

            switch (this.move_indicator)
            {
                case 0: // Right
                    current = current.right;
                    break;
                case 1: // Up
                    current = current.up;
                    break;
                case 2: // Left
                    current = current.left;
                    break;
                case 3: // Down
                    current = current.down;
                    break;
            }

            if (current != null) // Place the player on the grid
            {
                this.x = current.GetX();
                this.y = current.GetY();
                this.box.Location = new System.Drawing.Point(current.GetX() * 50, current.GetY() * 50);
            }
        }
    }
}