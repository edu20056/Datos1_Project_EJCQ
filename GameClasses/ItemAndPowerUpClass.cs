using System.Net;

namespace Windows_Forms_Attempt
{
    public class item_PU
    {
        private int value;
        private string image;
        private int x;
        private int y;
        private PictureBox box;


        public item_PU(int value, PictureBox box)
        {
            this.value = value; //In terms of items, value can be a 1,2,3. 1 means fuel tank, 2 means add stels and 3 means a bomb.
                                //In terms of powerups, value can be 4 or 5, 4 means speedbust and 5 means shield.
            this.image = image;
            this.x = 0;
            this.y = 0;
            this.box = box;
            Set_item();
            Create_Box();
        }

        public void Set_item()
        {
            if (this.value == 1) //Fuel tank
            {
                this.image = "Imagenes/Items/fuel_tank.png";
            }
            else if (this.value == 2) //Add stels
            {
                this.image = "Imagenes/Items/add_stels.png";
            }
            else if (this.value == 3) //Bomb
            {
                this.image = "Imagenes/Items/bomba.png";
            }
            else if (this.value == 4) // Speedbust
            {
                this.image = "Imagenes/PowerUps/speed_up.png";
            }
            else if (this.value == 5) //Shield
            {
                this.image = "Imagenes/PowerUps/shield.png";
            }
        }

        public int Get_x()
        {
            return this.x;
        }
        public int Get_y()
        {
            return this.y;
        }
        public void Create_Box()
        {
            if (this.box != null)
            {
                this.box.Location = new System.Drawing.Point(x * 50, y * 50);
                this.box.Name = "Bot";
                this.box.Size = new System.Drawing.Size(50, 50);
                this.box.SizeMode = PictureBoxSizeMode.StretchImage;
                this.box.Image = System.Drawing.Image.FromFile(this.image);
                this.box.BackColor = Color.Transparent; // Ensure no background color
            }
        }
        public void Change_Position(List<ArrayGrid.Node> list, int x_1, int y_1)
        {
            ArrayGrid.Node current = list.Find(node => node.GetX() == this.x && node.GetY() == this.y);

            if (current == null)
            {
                Console.WriteLine("No se encontró el nodo actual.");
                return;
            }

            else if (current != null) // Place the player on the grid
            {
                this.x = x_1;
                this.y = y_1;
                this.box.Location = new System.Drawing.Point(x_1 * 50, y_1 * 50);
            }
        }
    }
}