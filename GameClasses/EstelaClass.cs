namespace Windows_Forms_Attempt
{
    public class Estela
    {
        private PictureBox box;
        private int x;
        private int y;
        int prev_val;

        public Estela(PictureBox box, int x, int y)
        {
            this.box = box;
            this.x = x;
            this.y = y;
            Create_Box();
        }

        public int Get_Dirr()
        {
            return this.prev_val;
        }

        public int Set_Dirr(int new_val)
        {
            this.prev_val = new_val;
            return this.prev_val;
        }
        public int Get_X_est()
        {
            return this.x;
        }

        public int Get_Y_est()
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
                this.box.Image = System.Drawing.Image.FromFile("Imagenes/estela.png");
                this.box.BackColor = Color.Transparent; // Ensure no background color
            }
        }
        
        public void Change_Position(List<ArrayGrid.Node> list, int x, int y)
        {
            ArrayGrid.Node current = list.Find(node => node.GetX() == this.x && node.GetY() == this.y);
            ArrayGrid.Node next =  list.Find(node => node.GetX() == x && node.GetY() == y);

            if (current != null) // Place the player on the grid
            {
                this.x = next.GetX();
                this.y = next.GetY();
                this.box.Location = new System.Drawing.Point(current.GetX() * 50, current.GetY() * 50);
            }
        }

    }
}