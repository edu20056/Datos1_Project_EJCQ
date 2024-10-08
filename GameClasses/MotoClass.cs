using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Windows_Forms_Attempt
{
    public class Motorcycle
    {
        private int speed;
        private int stels;
        private int fuel;
        private PictureBox box;
        private string[] list_images;
        private PriorityQueue players_itmes;
        private ArrayStack players_powerups;

        private int x;
        private int y;
        private int move_indicator; // Indicates numbers form 1 to 4, which are related to movement.

        public Motorcycle(int speed, int stels, int fuel, string[] list_images, PictureBox box, int move_indicator, PriorityQueue players_items, ArrayStack players_powerups)
        {
            this.speed = speed;
            this.players_itmes = players_items;
            this.stels = stels;
            this.fuel = fuel;
            this.list_images = list_images;
            this.box = box;
            this.move_indicator = move_indicator;
            this.players_powerups = players_powerups;
            this.x = 8;
            this.y = 0;
            Create_Box(); // Configure the PictureBox inside the Motorcycle class
        }
        public PriorityQueue Retunr_List_Items()
        {
            return this.players_itmes;
        }

        public ArrayStack Retunr_List_PU()
        {
            return players_powerups;
        }

        public void SetSpeed(int newSpeed)
        {
            if (newSpeed > 0)
            {
                this.speed = newSpeed;
            }
        }
        public int GetSpeed()
        {
            return this.speed;
        }

        public void Set_New_Image_List(string[] list)
        {
            this.list_images = list;
        }

        public int Get_fuel()
        {
            return this.fuel;
        }
        public void Add_Stels()
        {
            this.stels++;
        }

        public void Set_fuel(int new_fuel)
        {
            this.fuel = new_fuel;
        }
        public int Get_x_player()
        {
            return this.x;
        }
        public int Get_Move_Indicator()
        {
            return this.move_indicator;
        }
        public int Get_y_player()
        {
            return this.y;
        }
        public void Create_Box()
        {
            if (this.box != null)
            {
                this.box.Location = new System.Drawing.Point(x * 50, y * 50);
                this.box.Name = "Player1";
                this.box.Size = new System.Drawing.Size(50, 50);
                this.box.SizeMode = PictureBoxSizeMode.StretchImage;
                this.box.Image = System.Drawing.Image.FromFile(list_images[0]);
                this.box.BackColor = Color.Transparent; 
            }
        }

        public void Move_Image(int index_image)
        {
            string new_image = list_images[index_image];
            this.box.Image = System.Drawing.Image.FromFile(new_image);
        }

        public void Change_Direction(int new_indicator)
        {
            if (new_indicator <= 4)
            {
                this.move_indicator = new_indicator;
            }
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
                case 1: // Right
                    current = current.right;
                    break;
                case 2: // Up
                    current = current.up;
                    break;
                case 3: // Left
                    current = current.left;
                    break;
                case 4: // Down
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
