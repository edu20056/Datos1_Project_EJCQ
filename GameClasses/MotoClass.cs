namespace Windows_Forms_Attempt
{
    public class Motorcycle
    {
        private int speed;
        private int stels;
        private int fuel;
        private PictureBox box;
        private string[] list_images;

        public Motorcycle(int speed, int stels, int fuel, string[] list_images, PictureBox box)
        {
            this.speed = speed;
            this.stels = stels;
            this.fuel = fuel;
            this.list_images = list_images;
            this.box = box;
            Create_Box(); // Configure the PictureBox inside the Motorcycle class
        }

        public void Create_Box()
        {
            if (this.box != null)
            {
                this.box.Location = new System.Drawing.Point(100, 100);
                this.box.Name = "pictureBox1";
                this.box.Size = new System.Drawing.Size(50, 50);
                this.box.SizeMode = PictureBoxSizeMode.StretchImage;
                this.box.Image = System.Drawing.Image.FromFile(list_images[0]);
                
            }
        }

        public void Move(int index_image)
        {
            string new_image = list_images[index_image];
            this.box.Image = System.Drawing.Image.FromFile(new_image);
        }
    }
}
