using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace WindowsFormsApplication3
{

    public partial class Form1 : Form
    {
        public int i = 0; //count # of rotate calls
        public int flag = 0;//for checking if any effect have been used 
        public Form1()
        {
            InitializeComponent();
        }

        private void label6_DoubleClick(object sender, EventArgs e)
        {
            label6.Visible = false;
        }

        private void load(object sender, EventArgs e)
        {
            OpenFileDialog opfile = new OpenFileDialog();
            opfile.Filter = "Image file(*.bmp, *.jpg)|*.bmp; *.jpg";

            if (DialogResult.OK == opfile.ShowDialog())
                pictureBox2.Image = new Bitmap(opfile.FileName);

            label9.Visible = true;
            label10.Visible = true;
            label10.Text = opfile.FileName;
            // pictureBox2.Load(textBox1.Text);
            // pictureBox2.ImageLocation = textBox1.Text;
            // pictureBox2.Load();
        }

        private void internals(object sender, EventArgs e)
        {

            FileStream fs = new FileStream(label10.Text, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);

            label6.Visible = true;
            Byte Signature1 = br.ReadByte();
            label6.Text += ("Signature = ") + (Convert.ToChar(Signature1)).ToString();
            byte Signature2 = br.ReadByte();
            label6.Text += (Convert.ToChar(Signature2)).ToString() + Environment.NewLine;
            int size = br.ReadInt32();
            label6.Text += ("File size = ") + size.ToString() + Environment.NewLine;
            fs.Seek(18, SeekOrigin.Begin);
            int width = br.ReadInt32();
            label6.Text += ("Img width = ") + width.ToString() + Environment.NewLine;
            int height = br.ReadInt32();
            label6.Text += ("Img height = ") + height.ToString();

        }

        private void filtersandcopy(object sender, EventArgs e)
        {
            if (pictureBox2.Image != null || pictureBox1.Image != null) // for stopping null exception
            {
                Bitmap bmp1;
                if (flag == 0 && pictureBox1.Image == null)
                    bmp1 = new Bitmap(pictureBox2.Image);
                else
                    bmp1 = new Bitmap(pictureBox1.Image);
                progressBar1.Maximum = bmp1.Width - 100; //better view -100

                Button button = sender as Button;
                string id = button.Text;

                if (id == "Sepia" || id == "Invert" || id == "Gray scale" || id == "Threshold" || id == "Opacity" || id == "Contrast" || id == "luminosity")
                {
                    flag++;
                    for (int x = 0; x < bmp1.Width; x++)
                    {
                        for (int y = 0; y < bmp1.Height; y++)
                        {

                            Color c1 = bmp1.GetPixel(x, y);

                            switch (id)

                            {
                                case "Gray scale":

                                    byte gray = (byte)(c1.R * 0.299 + c1.G * 0.587 + c1.G * 0.114);
                                    //int gp = (c1.R + c1.G + c1.B) / 3;

                                    bmp1.SetPixel(x, y, Color.FromArgb(gray, gray, gray));

                                    break;

                                case "Threshold":

                                    int ret;
                                    // Transform RGB to Y (gray scale)
                                    ret = (c1.A + c1.R + c1.G + c1.B) / 4;
                                    //Threshold 
                                    if (ret > 130)
                                    {
                                        ret = 255;
                                    }
                                    else
                                    {
                                        ret = 0;
                                    }
                                    //Set the pixel into the bitmap object.
                                    bmp1.SetPixel(x, y, Color.FromArgb(ret, ret, ret));
                                    //bmp2.SetPixel(x, y, Color.FromArgb((byte)((c1.A + c1.R + c1.G + c1.B) / 3), ret, ret, ret));
                                    // bmp2.SetPixel(x, y, Color.FromArgb((byte)((c1.A + c1.R + c1.G + c1.B) / 3), 0, 0, 0));

                                    break;
                                case "Opacity":
                                    /*since now approach is on current image at picturebox1
                                    // adjusting labels for and trackbars
                                    label2.Text = Convert.ToString(0); label3.Text = Convert.ToString(0);
                                    trackBar2.Value = 0; trackBar3.Value = 0;
                                    */
                                    float Opacity = float.Parse(label1.Text);
                                    label1.ForeColor = Color.Red;
                                    Opacity = c1.A * Opacity;

                                    bmp1.SetPixel(x, y, Color.FromArgb((int)(Opacity), c1.R, c1.G, c1.B));

                                    break;

                                case "luminosity":
                                    /*since now approach is on current image at picturebox1
                                    // adjusting labels for and trackbars
                                    label3.Text = Convert.ToString(0); label1.Text = Convert.ToString(0);
                                    trackBar3.Value = 0; trackBar1.Value = 0;
                                    */
                                    int cR, cG, cB;
                                    int luminosity = trackBar2.Value;
                                    label2.ForeColor = Color.Red;

                                    cR = c1.R + luminosity;
                                    cG = c1.G + luminosity;
                                    cB = c1.B + luminosity;

                                    if (cR < 0) cR = 1;
                                    if (cR > 255) cR = 255;

                                    if (cG < 0) cG = 1;
                                    if (cG > 255) cG = 255;

                                    if (cB < 0) cB = 1;
                                    if (cB > 255) cB = 255;
                                    bmp1.SetPixel(x, y, Color.FromArgb(cR, cG, cB));

                                    break;

                                case "Contrast":
                                    /*since now approach is on current image at picturebox1
                                    // adjusting labels for and trackbars
                                    label2.Text = Convert.ToString(0); label1.Text = Convert.ToString(0);
                                    trackBar2.Value = 0; trackBar1.Value = 0;
                                    */
                                    double contrast = trackBar3.Value;
                                    label3.ForeColor = Color.Red;

                                    contrast = (100.0 + contrast) / 100.0;
                                    contrast *= contrast;

                                    double pR = c1.R / 255.0;
                                    pR -= 0.5;
                                    pR *= contrast;
                                    pR += 0.5;
                                    pR *= 255;
                                    if (pR < 0) pR = 0;
                                    if (pR > 255) pR = 255;

                                    double pG = c1.G / 255.0;
                                    pG -= 0.5;
                                    pG *= contrast;
                                    pG += 0.5;
                                    pG *= 255;
                                    if (pG < 0) pG = 0;
                                    if (pG > 255) pG = 255;

                                    double pB = c1.B / 255.0;
                                    pB -= 0.5;
                                    pB *= contrast;
                                    pB += 0.5;
                                    pB *= 255;
                                    if (pB < 0) pB = 0;
                                    if (pB > 255) pB = 255;

                                    bmp1.SetPixel(x, y, Color.FromArgb((byte)pR, (byte)pG, (byte)pB));

                                    break;

                                case "Invert":

                                    bmp1.SetPixel(x, y, Color.FromArgb(255 - c1.R, 255 - c1.G, 255 - c1.B));

                                    break;

                                case "Sepia":

                                    int r = c1.R;
                                    int g = c1.G;
                                    int b = c1.B;

                                    //calculate temp value
                                    int tr = (int)(0.393 * r + 0.769 * g + 0.189 * b);
                                    int tg = (int)(0.349 * r + 0.686 * g + 0.168 * b);
                                    int tb = (int)(0.272 * r + 0.534 * g + 0.131 * b);

                                    //set new RGB value
                                    if (tr > 255) r = 255;

                                    else r = tr;

                                    if (tg > 255) g = 255;

                                    else g = tg;

                                    if (tb > 255) b = 255;

                                    else b = tb;

                                    bmp1.SetPixel(x, y, Color.FromArgb(r, g, b));
                                    break;

                            }//end of innter loop

                            pictureBox1.Image = bmp1;
                        }//end of switch

                        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                        progressBar1.Visible = true;
                        progressBar1.PerformStep();

                    }//end loops
                    progressBar1.Value = 0;

                }//main if

                progressBar1.Visible = false;

                if (id == "Blur")
                {
                    flag++;
                    textBox4.ForeColor = Color.Red;
                    int blurAmount = Convert.ToInt32(textBox4.Text);
                    progressBar1.Maximum = bmp1.Width * blurAmount + (blurAmount + 20);// *bluramount + (blurAmount+20) average time complexity of loops
                    progressBar1.Visible = true;

                    for (int blr = 0; blr <= blurAmount; blr++)
                    {
                        for (int x = blr; x < bmp1.Width - blr; x++)
                        {
                            for (int y = blr; y < bmp1.Height - blr; y++)
                            {
                                try
                                {
                                    Color prevX = bmp1.GetPixel(x - blr, y);
                                    Color nextX = bmp1.GetPixel(x + blr, y);
                                    Color prevY = bmp1.GetPixel(x, y - blr);
                                    Color nextY = bmp1.GetPixel(x, y + blr);

                                    int avgR = (prevX.R + nextX.R + prevY.R + nextY.R) / 4;
                                    int avgG = (prevX.G + nextX.G + prevY.G + nextY.G) / 4;
                                    int avgB = (prevX.B + nextX.B + prevY.B + nextY.B) / 4;

                                    bmp1.SetPixel(x, y, Color.FromArgb(avgR, avgG, avgB));
                                }
                                catch (Exception) { }
                            }
                            progressBar1.PerformStep();
                        }
                        pictureBox1.Image = bmp1;
                    }//end of loops
                    progressBar1.Value = 0;
                    progressBar1.Visible = false;
                }

                if (id == "Rotate")
                {
                    flag++;
                    i++;
                    float angle = (90 * i);
                    int w = bmp1.Width;
                    int h = bmp1.Height;
                    PixelFormat pf = default(PixelFormat);

                    //pf = PixelFormat.Format32bppArgb;

                    pf = bmp1.PixelFormat;
                    Bitmap tempImg = new Bitmap(w, h, pf);
                    Graphics g = Graphics.FromImage(tempImg);
                    g.DrawImageUnscaled(bmp1, 1, 1);
                    g.Dispose();

                    GraphicsPath path = new GraphicsPath();
                    path.AddRectangle(new RectangleF(0f, 0f, w, h));
                    Matrix mtrx = new Matrix();
                    //Using System.Drawing.Drawing2D.Matrix class 
                    mtrx.Rotate(angle);
                    RectangleF rct = path.GetBounds(mtrx);
                    Bitmap newImg = new Bitmap(Convert.ToInt32(rct.Width), Convert.ToInt32(rct.Height), pf);
                    g = Graphics.FromImage(newImg);

                    g.TranslateTransform(-rct.X, -rct.Y);
                    g.RotateTransform(angle);
                    g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                    g.DrawImageUnscaled(tempImg, 0, 0);

                    g.Dispose();
                    tempImg.Dispose();

                    pictureBox1.Image = newImg;
                }

                else if (id == "Resize")
                {
                    flag++;

                    textBox2.ForeColor = Color.Red;
                    textBox3.ForeColor = Color.Red;
                    int w = int.Parse(textBox3.Text);  //size input
                    int h = int.Parse(textBox2.Text);

                    int newW = pictureBox1.Width, newH = pictureBox1.Height;

                    Size original = pictureBox1.Size; //store current picbox size

                    //change according to new size
                    if (original.Width < w)
                        newW = original.Width + (h - original.Width);
                    else if (original.Width > w)
                        newW = original.Width - (original.Width - h);
                    if (original.Height < h)
                        newH = original.Height + (h - original.Height);
                    else if (original.Width > h)
                        newH = original.Height - (original.Height - h);

                    //set picbox size
                    original.Width = newW; original.Height = newH;
                    pictureBox1.Size = original;

                    //set image
                    pictureBox1.Image = bmp1;

                }

                else if (id == "Color filter")
                {
                    flag++;
                    progressBar1.Visible = true;
                    progressBar1.Maximum = bmp1.Width - 100; //better view

                    int xmid = bmp1.Width / 2;
                    int ymid = bmp1.Height / 2;


                    for (int y = 0; y < ymid; y++)
                    {

                        for (int x = 0; x < xmid; x++)
                        {

                            Color c1 = bmp1.GetPixel(x, y);

                            //    bmp2.SetPixel(x, y, Color.FromArgb((byte)((c1.A + c1.R + c1.G + c1.B) / 3), 0, 0, 0));                        
                            bmp1.SetPixel(x, y, Color.FromArgb(c1.A, (byte)((c1.A + c1.R + c1.G + c1.B) / 3), (byte)((c1.A + c1.R + c1.G + c1.B) / 3), (byte)((c1.A + c1.R + c1.G + c1.B) / 3)));
                        }
                        pictureBox1.Image = bmp1;
                        progressBar1.PerformStep();
                    }

                    for (int y = ymid; y < bmp1.Height; y++)
                    {

                        for (int x = 0; x < xmid; x++)
                        {
                            Color c1 = bmp1.GetPixel(x, y);

                            bmp1.SetPixel(x, y, Color.FromArgb(c1.R, 0, 0));
                        }
                        pictureBox1.Image = bmp1;
                        progressBar1.PerformStep();
                    }


                    for (int y = 0; y < ymid; y++)
                    {
                        for (int x = xmid; x < bmp1.Width; x++)
                        {
                            Color c1 = bmp1.GetPixel(x, y);

                            bmp1.SetPixel(x, y, Color.FromArgb(0, c1.G, 0));
                        }
                        pictureBox1.Image = bmp1;
                        progressBar1.PerformStep();
                    }

                    for (int y = ymid; y < bmp1.Height; y++)
                    {

                        for (int x = xmid; x < bmp1.Width; x++)
                        {
                            Color c1 = bmp1.GetPixel(x, y);

                            bmp1.SetPixel(x, y, Color.FromArgb(0, 0, c1.B));
                        }
                        pictureBox1.Image = bmp1;
                        progressBar1.PerformStep();
                    }

                    progressBar1.Visible = false;
                    progressBar1.Value = 0;
                }//else if end
            }

            else//Null exception catcher
            
                MessageBox.Show("Please load an image.", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //for managing label when messagebox pop-up
            if (pictureBox2.Image == null)
            {
                label1.ForeColor = Color.Black;
                label1.Text = Convert.ToString(0);
                trackBar1.Value = 0;
                label2.ForeColor = Color.Black;
                label2.Text = Convert.ToString(0);
                trackBar2.Value = 0;
                label3.ForeColor = Color.Black;
                label3.Text = Convert.ToString(0);
                trackBar3.Value = 0;
            }
            
            }//fucntion end


        private void conventionalkernel(object sender, EventArgs e)
        {
            if (pictureBox2.Image != null || pictureBox1.Image != null) // for stopping null exception
            {

                Bitmap bmp1;
                if (flag == 0 || pictureBox1.Image == null)
                    bmp1 = new Bitmap(pictureBox2.Image);
                else
                    bmp1 = new Bitmap(pictureBox1.Image);

                Bitmap bmp2 = new Bitmap(bmp1.Width, bmp1.Height); //have to use two bmp becoz shapern causing issue
                progressBar1.Maximum = bmp1.Width - 100; //better view -100

                Button button = sender as Button;
                string id = button.Text;

                if (id == "Laplacian detection" || id == "Emboss" || id == "GaussianBlur" || id == "Sobel detection" || id == "Kirsch detection" || id == "Prewitt detection" || id == "Sharpen")
                {
                    flag++;
                    int red, blu, grn, avg;
                    Color color, color1, color2, color3, color4, color5, color6, color7, color8;

                    for (int x = 1; x < bmp1.Width - 1; x++)
                    {
                        for (int y = 1; y < bmp1.Height - 1; y++)
                        {

                            switch (id)
                            {
                                case "Laplacian detection":

                                    color = bmp1.GetPixel(x - 1, y - 1);//top left
                                    color1 = bmp1.GetPixel(x, y - 1);//above middle    /* |-1  -1  -1|
                                    color2 = bmp1.GetPixel(x + 1, y - 1);//top right      |-1   8  -1|
                                    color3 = bmp1.GetPixel(x - 1, y);//left middle        |-1  -1  -1|
                                    color4 = bmp1.GetPixel(x, y);//middle                              */
                                    color5 = bmp1.GetPixel(x + 1, y);//right middle
                                    color6 = bmp1.GetPixel(x - 1, y + 1);//bottom left
                                    color7 = bmp1.GetPixel(x, y + 1);//bottom middle
                                    color8 = bmp1.GetPixel(x + 1, y + 1);//bottom right

                                    //All direction Laplacian
                                    red = color.R * (-1) + color1.R * (-1) + color2.R * (-1) + color3.R * (-1) + color4.R * (8) + color5.R * (-1) + color6.R * (-1) + color7.R * (-1) + color8.R * (-1);
                                    grn = color.G * (-1) + color1.G * (-1) + color2.G * (-1) + color3.G * (-1) + color4.G * (8) + color5.G * (-1) + color6.G * (-1) + color7.G * (-1) + color8.G * (-1);
                                    blu = color.B * (-1) + color1.B * (-1) + color2.B * (-1) + color3.B * (-1) + color4.B * (8) + color5.B * (-1) + color6.B * (-1) + color7.B * (-1) + color8.B * (-1);

                                    if (red > 255) red = 255; else if (red < 0) red = 0;
                                    if (grn > 255) grn = 255; else if (grn < 0) grn = 0;
                                    if (blu > 255) blu = 255; else if (blu < 0) blu = 0;

                                    avg = (red + blu + grn) / 3;

                                    bmp2.SetPixel(x, y, Color.FromArgb(avg,avg,avg));
                                    break;

                                case "Emboss":

                                    color = bmp1.GetPixel(x - 1, y - 1); //top left         /*|-2 - 1   0 |
                                    color1 = bmp1.GetPixel(x, y - 1);//above middle           |-1   1   1 |
                                    color2 = bmp1.GetPixel(x, y); // middle                   | 0   1   2 |/*
                                    color3 = bmp1.GetPixel(x - 1, y);//left of middle
                                    color4 = bmp1.GetPixel(x + 1, y); //right of middle
                                    color5 = bmp1.GetPixel(x, y + 1);//bottom of middle
                                    color6 = bmp1.GetPixel(x + 1, y + 1); //bottom right

                                    red = color.R * (-2) + color1.R * (-1) + color2.R * (1) + color3.R * (-1) + color4.R * (1) + color5.R * (1) + color6.R * (2);
                                    grn = color.G * (-2) + color1.G * (-1) + color2.G * (1) + color3.G * (-1) + color4.G * (1) + color5.G * (1) + color6.G * (2);
                                    blu = color.B * (-2) + color1.B * (-1) + color2.B * (1) + color3.B * (-1) + color4.B * (1) + color5.B * (1) + color6.B * (2);

                                    if (red > 255) red = 255; else if (red < 0) red = 0;
                                    if (grn > 255) grn = 255; else if (grn < 0) grn = 0;
                                    if (blu > 255) blu = 255; else if (blu < 0) blu = 0;

                                    bmp2.SetPixel(x, y, Color.FromArgb(red, grn, blu));
                                    break;

                                case "GaussianBlur":

                                    color = bmp1.GetPixel(x - 1, y - 1);//top left
                                    color1 = bmp1.GetPixel(x, y - 1);//above middle      /*|1  2  1|
                                    color2 = bmp1.GetPixel(x + 1, y - 1);//top right       |2  4  2|
                                    color3 = bmp1.GetPixel(x - 1, y);//left middle         |1  2  1|*/
                                    color4 = bmp1.GetPixel(x, y);//middle
                                    color5 = bmp1.GetPixel(x + 1, y);//right middle
                                    color6 = bmp1.GetPixel(x - 1, y + 1);//bottom left
                                    color7 = bmp1.GetPixel(x, y + 1);//bottom middle
                                    color8 = bmp1.GetPixel(x + 1, y + 1);//bottom right

                                    red = (color.R * (1) + color1.R * (2) + color2.R * (1) + color3.R * (2) + color4.R * (4) + color5.R * (2) + color6.R * (1) + color7.R * (2) + color8.R * (1));
                                    grn = (color.G * (1) + color1.G * (2) + color2.G * (1) + color3.G * (2) + color4.G * (4) + color5.G * (2) + color6.G * (1) + color7.G * (2) + color8.G * (1));
                                    blu = (color.B * (1) + color1.B * (2) + color2.B * (1) + color3.B * (2) + color4.B * (4) + color5.B * (2) + color6.B * (1) + color7.B * (2) + color8.B * (1));

                                    red = (int)(red * (0.0625)); grn = (int)(grn * (0.0625)); blu = (int)(blu * (0.0625)); // (1/6)=0.0625(Normalization factor)
                                    if (red > 255) red = 255; else if (red < 0) red = 0;
                                    if (grn > 255) grn = 255; else if (grn < 0) grn = 0;
                                    if (blu > 255) blu = 255; else if (blu < 0) blu = 0;


                                    bmp2.SetPixel(x, y, Color.FromArgb(red, grn, blu));
                                    break;

                                case "Sobel detection":

                                    color = bmp1.GetPixel(x - 1, y - 1);//top left
                                    color1 = bmp1.GetPixel(x + 1, y - 1);//top right   /*Vertical sobel   Horizontal sobel
                                    color2 = bmp1.GetPixel(x - 1, y);//left middle        |-1  0  1|      |-1  -2  -1|
                                    color3 = bmp1.GetPixel(x + 1, y); //right middle      |-2  0  2|      |0    0   0|
                                    color4 = bmp1.GetPixel(x - 1, y + 1);//bottom left    |-1  0  1|      |1    2   1| */
                                    color5 = bmp1.GetPixel(x + 1, y + 1);//bottom right
                                    color6 = bmp1.GetPixel(x, y - 1);//above middle
                                    color7 = bmp1.GetPixel(x, y + 1);//below middle
                                    color8 = bmp1.GetPixel(x, y);//middle

                                    //Vertical sobel
                                    red = color.R * (-1) + color1.R * (1) + color2.R * (-2) + color3.R * (2) + color4.R * (-1) + color5.R * (1);
                                    grn = color.G * (-1) + color1.G * (1) + color2.G * (-2) + color3.G * (2) + color4.G * (-1) + color5.G * (1);
                                    blu = color.B * (-1) + color1.B * (1) + color2.B * (-2) + color3.B * (2) + color4.B * (-1) + color5.B * (1);

                                    //Horizontal sobel
                                    red += color.R * (-1) + color6.R * (-2) + color1.R * (-1) + color4.R * (1) + color7.R * (2) + color5.R * (1);
                                    grn += color.G * (-1) + color6.G * (-2) + color1.G * (-1) + color4.G * (1) + color7.G * (2) + color5.G * (1);
                                    blu += color.B * (-1) + color6.B * (-2) + color1.B * (-1) + color4.B * (1) + color7.B * (2) + color5.B * (1);

                                    red = (int)(red * (0.125)); grn = (int)(grn * (0.125)); blu = (int)(blu * (0.125)); //normalization factor (1/8)

                                    if (red > 255) red = 255; else if (red < 0) red = 0;
                                    if (grn > 255) grn = 255; else if (grn < 0) grn = 0;
                                    if (blu > 255) blu = 255; else if (blu < 0) blu = 0;

                                    avg = (red + grn + blu) / 3;

                                    bmp2.SetPixel(x, y, Color.FromArgb(avg, avg, avg));
                                    break;

                                case "Kirsch detection":

                                    color = bmp1.GetPixel(x - 1, y - 1);//top left  /* |5    5   5|
                                    color1 = bmp1.GetPixel(x, y - 1);//above middle    |-3  -3  -3|
                                    color2 = bmp1.GetPixel(x + 1, y - 1);//top right   |-3  -3  -3|*/
                                    color3 = bmp1.GetPixel(x - 1, y);//left middle
                                    color4 = bmp1.GetPixel(x, y);//middle
                                    color5 = bmp1.GetPixel(x + 1, y);//right middle
                                    color6 = bmp1.GetPixel(x - 1, y + 1);//bottom left
                                    color7 = bmp1.GetPixel(x, y + 1);//bottom middle
                                    color8 = bmp1.GetPixel(x + 1, y + 1);//bottom right

                                    red = color.R * (-5) + color1.R * (3) + color2.R * (3) + color3.R * (-5) + color4.R * (0) + color5.R * (3) + color6.R * (-5) + color7.R * (3) + color8.R * (3);
                                    grn = color.G * (-5) + color1.G * (3) + color2.G * (3) + color3.G * (-5) + color4.G * (0) + color5.G * (3) + color6.G * (-5) + color7.G * (3) + color8.G * (3);
                                    blu = color.B * (-5) + color1.B * (3) + color2.B * (3) + color3.B * (-5) + color4.B * (0) + color5.B * (3) + color6.B * (-5) + color7.B * (3) + color8.B * (3);

                                    if (red > 255) red = 255; else if (red < 0) red = 0;
                                    if (grn > 255) grn = 255; else if (grn < 0) grn = 0;
                                    if (blu > 255) blu = 255; else if (blu < 0) blu = 0;

                                    avg = (red + grn + blu) / 3;

                                    bmp2.SetPixel(x, y, Color.FromArgb(avg, avg, avg));
                                    break;

                                case "Prewitt detection":

                                    color = bmp1.GetPixel(x - 1, y - 1);//top left   /*|-1 -1 -1|
                                    color1 = bmp1.GetPixel(x, y - 1);//above middle    |0   0  0|
                                    color2 = bmp1.GetPixel(x + 1, y - 1);//top right   |1   1  1|*/                                
                                    color3 = bmp1.GetPixel(x - 1, y + 1);//bottom left
                                    color4 = bmp1.GetPixel(x, y + 1);//bottom middle
                                    color5 = bmp1.GetPixel(x + 1, y + 1);//bottom right

                                    red = (color.R * (-1) + color1.R * (-1) + color2.R * (-1) + color3.R * (1) + color4.R * (1) + color5.R * (1));
                                    grn = (color.G * (-1) + color1.G * (-1) + color2.G * (-1) + color3.G * (1) + color4.G * (1) + color5.G * (1));
                                    blu = (color.B * (-1) + color1.B * (-1) + color2.B * (-1) + color3.B * (1) + color4.B * (1) + color5.B * (1));

                                    red = (int)(red * (0.333)); grn = (int)(grn * (0.333)); blu = (int)(blu * (0.333)); //normalization factor (1/3)

                                    if (red > 255) red = 255; else if (red < 0) red = 0;
                                    if (grn > 255) grn = 255; else if (grn < 0) grn = 0;
                                    if (blu > 255) blu = 255; else if (blu < 0) blu = 0;

                                    avg = (red + grn + blu) / 3;

                                    bmp2.SetPixel(x, y, Color.FromArgb(avg, avg, avg));
                                    break;

                                case "Sharpen":

                                    color = bmp1.GetPixel(x, y - 1);//above middle    /*|0  -1   0|
                                    color1 = bmp1.GetPixel(x - 1, y);//left middle      |-1  5  -1|
                                    color2 = bmp1.GetPixel(x, y);//middle               |0  -1  0 |*/
                                    color3 = bmp1.GetPixel(x + 1, y);//right middle                                
                                    color4 = bmp1.GetPixel(x, y + 1);//bottom middle                                

                                    red = color.R * (-1) + color1.R * (-1) + color2.R * (5) + color3.R * (-1) + color4.R * (-1);
                                    grn = color.G * (-1) + color1.G * (-1) + color2.G * (5) + color3.G * (-1) + color4.G * (-1);
                                    blu = color.B * (-1) + color1.B * (-1) + color2.B * (5) + color3.B * (-1) + color4.B * (-1);

                                    if (red > 255) red = 255; else if (red < 0) red = 0;
                                    if (grn > 255) grn = 255; else if (grn < 0) grn = 0;
                                    if (blu > 255) blu = 255; else if (blu < 0) blu = 0;

                                    bmp2.SetPixel(x, y, Color.FromArgb(red, grn, blu));
                                    break;

                            }//end of innter loop

                            pictureBox1.Image = bmp2;

                        }//end of switch

                        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                        progressBar1.Visible = true;
                        progressBar1.PerformStep();

                    }//end loops

                    progressBar1.Value = 0;
                }//main if
            }

            else//null exception catcher
                MessageBox.Show("Please load an image.", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        }
        private void Valuetrack(object sender, EventArgs e)
        {
            label3.ForeColor = Color.Black;
            label3.Text = trackBar3.Value.ToString();
        }

        private void ValueTrack1(object sender, EventArgs e)
        {
            label2.ForeColor = Color.Black;
            label2.Text = trackBar2.Value.ToString();
        }

        private void ValueTrack2(object sender, EventArgs e)
        {
            label1.ForeColor = Color.Black;
            label1.Text = ((float)trackBar1.Value / 10).ToString();
        }

        private void ClearPicBox(object sender, EventArgs e)
        {
            var pic = (sender as PictureBox).Name;
            if (pic == "pictureBox2")
            {
                pictureBox2.Image = null;
                label9.Visible = false;
                label10.Text = null;
                label10.Visible = false;
            }
            else if (pic == "pictureBox1")
            {    
                pictureBox1.Image = null;

                label1.ForeColor = Color.Black;             
                label1.Text = Convert.ToString(0);
                trackBar1.Value = 0;

                label2.ForeColor = Color.Black;
                label2.Text = Convert.ToString(0);
                trackBar2.Value = 0;

                label3.ForeColor = Color.Black;
                label3.Text = Convert.ToString(0);
                trackBar3.Value = 0;

                //size related
                pictureBox1.Size = pictureBox2.Size;
                textBox2.Text = null;
                textBox2.ForeColor = Color.Black;
                textBox3.Text = null;
                textBox3.ForeColor = Color.Black;
                //blur related
                textBox4.Text = null;
                textBox4.ForeColor = Color.Black;

                flag = 0;
            }
        }

        private void NumberOnly(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && (!char.IsDigit(e.KeyChar)))

                e.Handled = true;
        }

        private void resizeHW(object sender, EventArgs e)
        {
            if (pictureBox2.Image != null || pictureBox1.Image!=null)
            {
                textBox2.Enabled = true;
                textBox3.Enabled = true;
                textBox4.Enabled = true;
            }
            else
            {  
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                textBox4.Enabled = false;
                textBox2.Text = null;
                textBox3.Text = null;
                textBox4.Text = null;
            }
        }

        private void Default_Forecolor(object sender, EventArgs e)
        {
            var text = (sender as TextBox).Name;
            if (text == "textBox2")
                textBox2.ForeColor = Color.Black;
            else if (text == "textBox3")
                textBox3.ForeColor = Color.Black;
            else if (text == "textBox4")
                textBox4.ForeColor = Color.Black;

        }

        private void Helpdetails(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBox.Show("\nINSTRUCTIONS \n\n\nFor clearing pictureboxes: Simply double click them.\nFor turning off BMP details: Double click it.\nREMEMBER!\nIf you over-extend the resize option,kindly drag adjust the application.\nInternal info of BMP image will only be displayed otherwise some garbage value.\n\n\t\t\tEnjoy!!! :) ", "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void Helpdetails(object sender, HelpEventArgs hlpevent)
        {
            MessageBox.Show("\nINSTRUCTIONS \n\n\nFor clearing pictureboxes: Simply double click them.\nFor turning off BMP details: Double click it.\nREMEMBER!\nIf you over-extend the resize option,kindly drag adjust the application.\nInternal info of BMP image will only be displayed otherwise some garbage value.\n\n\t\t\tEnjoy!!! :) ", "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
    }
}