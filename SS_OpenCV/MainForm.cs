using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;

namespace SS_OpenCV
{
    public partial class MainForm : Form
    {
        //bool haveOpenCL = CvInvoke.HaveOpenCL;

        //bool haveOpenClGpu = CvInvoke.HaveOpenCLCompatibleGpuDevice;

        //CvInvoke.UseOpenCL = true;
        
        Image<Bgr, Byte> img = null; // working image
        Image<Bgr, Byte> imgUndo = null; // undo backup image - UNDO
        Image<Hsv, Byte> imgHsv = null; // hsv image
        Image<Bgr, Byte> digit0 = new Image<Bgr, Byte>("..\\..\\Imagens-20190916\\digits\\0.png");
        Image<Bgr, Byte> digit1 = new Image<Bgr, Byte>("..\\..\\Imagens-20190916\\digits\\1.png");
        Image<Bgr, Byte> digit2 = new Image<Bgr, Byte>("..\\..\\Imagens-20190916\\digits\\2.png");
        Image<Bgr, Byte> digit3 = new Image<Bgr, Byte>("..\\..\\Imagens-20190916\\digits\\3.png");
        Image<Bgr, Byte> digit4 = new Image<Bgr, Byte>("..\\..\\Imagens-20190916\\digits\\4.png");
        Image<Bgr, Byte> digit5 = new Image<Bgr, Byte>("..\\..\\Imagens-20190916\\digits\\5.png");
        Image<Bgr, Byte> digit6 = new Image<Bgr, Byte>("..\\..\\Imagens-20190916\\digits\\6.png");
        Image<Bgr, Byte> digit7 = new Image<Bgr, Byte>("..\\..\\Imagens-20190916\\digits\\7.png");
        Image<Bgr, Byte> digit8 = new Image<Bgr, Byte>("..\\..\\Imagens-20190916\\digits\\8.png");
        Image<Bgr, Byte> digit9 = new Image<Bgr, Byte>("..\\..\\Imagens-20190916\\digits\\9.png");
        List<Image<Bgr, Byte>> digits = new List<Image<Bgr, Byte>>();

        string title_bak = "";

        public MainForm()
        {
            InitializeComponent();
            title_bak = Text;
        }

        /// <summary>
        /// Opens a new image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                img = new Image<Bgr, byte>(openFileDialog1.FileName);
                imgHsv = new Image<Hsv, byte>(openFileDialog1.FileName);
                digits.Add(digit0);
                digits.Add(digit1);
                digits.Add(digit2);
                digits.Add(digit3);
                digits.Add(digit4);
                digits.Add(digit5);
                digits.Add(digit6);
                digits.Add(digit7);
                digits.Add(digit8);
                digits.Add(digit9);
                Text = title_bak + " [" +
                        openFileDialog1.FileName.Substring(openFileDialog1.FileName.LastIndexOf("\\") + 1) +
                        "]";
                imgUndo = img.Copy();

                ImageViewer.Image = img.Bitmap;
                ImageViewer.Refresh();
                
            }
        }

        /// <summary>
        /// Saves an image with a new name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ImageViewer.Image.Save(saveFileDialog1.FileName);
            }
        }

        /// <summary>
        /// Closes the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// restore last undo copy of the working image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (imgUndo == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor;
            img = imgUndo.Copy();

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        /// <summary>
        /// Change visualization mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // zoom
            if (autoZoomToolStripMenuItem.Checked)
            {
                ImageViewer.SizeMode = PictureBoxSizeMode.Zoom;
                ImageViewer.Dock = DockStyle.Fill;
            }
            else // with scroll bars
            {
                ImageViewer.Dock = DockStyle.None;
                ImageViewer.SizeMode = PictureBoxSizeMode.AutoSize;
            }
        }

        /// <summary>
        /// Show authors form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AuthorsForm form = new AuthorsForm();
            form.ShowDialog();
        }

        /// <summary>
        /// Calculate the image negative
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void negativeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.Negative(img);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        /// <summary>
        /// Call automated image processing check
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void evalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EvalForm eval = new EvalForm();
            eval.ShowDialog();
        }

        /// <summary>
        /// Call image convertion to gray scale
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.ConvertToGray(img);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        private void TranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            InputBox form = new InputBox("dX?");
            form.ShowDialog();
            int dx = Convert.ToInt32(form.ValueTextBox.Text);

            InputBox form2 = new InputBox("dY?");
            form2.ShowDialog();
            int dy = Convert.ToInt32(form2.ValueTextBox.Text);

            ImageClass.Translation(img, imgUndo, dx, dy);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        private void RotationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            InputBox form = new InputBox("Angle?");
            form.ShowDialog();
            int angle = Convert.ToInt32(form.ValueTextBox.Text);


            ImageClass.Rotation(img, imgUndo, angle);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        private void MeanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.Mean(img, imgUndo);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        private void NonUniformToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            Form1 form = new Form1();

            form.ShowDialog();

            ImageClass.NonUniform(img, imgUndo, form.matrix, form.weight);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        private void SobelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.Sobel(img, imgUndo);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        private void RobertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.Diferentiation(img, imgUndo);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        private void MedianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.Median(img, imgUndo);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        private void GreyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 
            int[,] histDataRGB = ImageClass.Histogram_RGB(img);

            Form2 hist = new Form2(ImageClass.Histogram_Gray(img), histDataRGB);
            hist.ShowDialog();

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        private void ScaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //input boxes
            InputBox form = new InputBox("scale?");
            form.ShowDialog();
            float scaleFactor = Convert.ToInt32(form.ValueTextBox.Text);

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.Scale(img, imgUndo, scaleFactor);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor
        }

        private void ScalePointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //input boxes
            InputBox form = new InputBox("scale?");
            form.ShowDialog();
            float scaleFactor = Convert.ToInt32(form.ValueTextBox.Text);

            InputBox formx = new InputBox("x?");
            formx.ShowDialog();
            int centerX = Convert.ToInt32(formx.ValueTextBox.Text);

            InputBox formy = new InputBox("y?");
            formy.ShowDialog();
            int centerY = Convert.ToInt32(formy.ValueTextBox.Text);

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.Scale_point_xy(img, imgUndo, scaleFactor, centerX, centerY);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor
        }

        private void BrightnessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //input boxes
            InputBox form = new InputBox("brightness?");
            form.ShowDialog();
            int brightness = Convert.ToInt32(form.ValueTextBox.Text);

            InputBox formc = new InputBox("contrast?");
            formc.ShowDialog();
            float contrast = Convert.ToInt32(formc.ValueTextBox.Text);


            //copy Undo Image
            imgUndo = img.Copy();



            ImageClass.BrightContrast(img, brightness, contrast);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor

        }

        private void RedChannelToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 


            //copy Undo Image
            imgUndo = img.Copy();



            ImageClass.RedChannel(img);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor


        }

        private void BlackAndWhiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //input boxes
            InputBox form = new InputBox("threshold?");
            form.ShowDialog();
            int threshold = Convert.ToInt32(form.ValueTextBox.Text);

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.ConvertToBW(img, threshold);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor
        }

        private void BlackAndWhiteOtsuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 


            ImageClass.ConvertToBW_Otsu(img);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor

        }

        private void Identify1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (img == null) // verify if the image is already opened
             //   return;
            //Cursor = Cursors.WaitCursor; // clock cursor 

            //Load digits
            

            //HSV image inside imgUndo, you can change imgUndo to imgHsv to get different result (also Bgr to Hsv change needed in BgrToHsv func)
            //Identify.BgrToHsv(img, imgUndo);

            //Drawing rectangle
            //Identify.DrawRectangle(img, new int[]{200, 300, 400, 500});

            //Scale digits image
            digits = Identify.Scale(digits, new int[] { 200, 300, 400, 500 });
            
            ImageViewer.Image = digits[9].Bitmap;
            //ImageViewer.Image = img.Bitmap;

            ImageViewer.Refresh(); // refresh image on the screen
            Cursor = Cursors.Default; // normal cursor
        }

    }
}