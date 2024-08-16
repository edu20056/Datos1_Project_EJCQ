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

        public MotorcycleBot(int speed, int stels, string[] list_images, PictureBox box, int x, int y)
        {
            this.speed = speed;
            this.stels = stels;
            this.list_images = list_images;
            this.box = box;
            this.x = y;
            this.y = x;
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
                this.box.Image = System.Drawing.Image.FromFile(list_images[0]);
                this.box.BackColor = Color.Transparent; // Ensure no background color
            }
        }

        public void Move_Image(int index_image)
        {
            string new_image = list_images[index_image];
            this.box.Image = System.Drawing.Image.FromFile(new_image);
        }
    }
}