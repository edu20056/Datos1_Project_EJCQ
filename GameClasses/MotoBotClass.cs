namespace Windows_Forms_Attempt
{
    public class MotorcycleBot
    {
        private int speed;
        private int stels;
        private PictureBox box;
        private string[] list_images;

        private int x;
        private int y;
        private int move_indicator;

        public MotorcycleBot(int speed, int stels, string[] list_images, PictureBox box, int x, int y, int move_indicator)
        {
            this.speed = speed;
            this.stels = stels;
            this.list_images = list_images;
            this.box = box;
            this.x = y;
            this.y = x;
            this.move_indicator = move_indicator;
            Create_Box();
        }

         public void SetSpeed(int newSpeed)
        {
            if (newSpeed > 0)
            {
                this.speed = newSpeed;
            }
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