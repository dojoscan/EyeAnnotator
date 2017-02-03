using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

// Main form

// To do:
// Delete current boxes
// Show current boxes
// Next un-annotated image
// Save object class

namespace EyeAnnotator
{
    public partial class Form1 : Form
    {
        int imageIndex = 1;
        int futureIndex;
        int clickCounter = 0;
        float X1;
        float Y1;
        float X2;
        float Y2;
        string objectClass;
        float width;
        float height;
        string folderPath = "C:\\Users\\Donal\\Documents\\PewpL\\Eyes";
        string savePath = "C:\\Users\\Donal\\Documents\\PewpL\\DELETE";
        int numImages = Directory.GetFiles("C:\\Users\\Donal\\Documents\\PewpL\\Eyes").Length - 1;
        int numImAnno = Directory.GetFiles("C:\\Users\\Donal\\Documents\\PewpL\\DELETE").Length;
        bool canDraw;
        int startX, startY;
        Rectangle rekt;

        // Initialise
        public Form1()
        {
            DoubleBuffered = true;
            InitializeComponent();
            displayImage(imageIndex);
            writeTextBox3(numImAnno);
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            imageIndex++;
            if (imageIndex == numImages + 1)
            { imageIndex = numImages; }
            displayImage(imageIndex);
            rekt = Rectangle.Empty;
        }

        private void previousButton_Click(object sender, EventArgs e)
        {
            imageIndex--;
            if (imageIndex == 0)
            { imageIndex = 1; }
            displayImage(imageIndex);
            rekt = Rectangle.Empty;
        }

        private void goToButton_Click(object sender, EventArgs e)
        {
            // Check if text input is an integer
            bool isTextANumber = int.TryParse(textBox2.Text, out futureIndex);
            if (isTextANumber == true)
            {
                if (futureIndex > numImages)
                { futureIndex = numImages; }
                imageIndex = futureIndex;
                displayImage(imageIndex);
            }
            else
            {
                return;
            }
        }

        // Draw rectangle
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            clickCounter++;
            MouseEventArgs me = (MouseEventArgs)e;
            Point mousePosition = me.Location;
            width = pictureBox1.Width;
            height = pictureBox1.Height;
            if (clickCounter < 2)   // First corner selected
            {
                canDraw = true;
                startX = mousePosition.X;
                startY = mousePosition.Y;
                X1 = (startX - (width / 2)) / (width / 2);
                Y1 = (startY - (height / 2)) / (height / 2);
            }
            else // Second corner selected
            {
                X2 = (mousePosition.X - (width / 2)) / (width / 2);
                Y2 = (mousePosition.Y - (height / 2)) / (height / 2);
                // Input object class
                using (var inputForm = new Form2())
                {
                    var result = inputForm.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        objectClass = inputForm.objectClass;          
                    }
                }
                // Convert to X,Y,W,H
                float X = Math.Min(X1, X2);
                float Y = Math.Min(Y1, Y2);
                float W = Math.Abs(X1 - X2);
                float H = Math.Abs(Y1 - Y2);
                // Write to TXT file
                string boundingBox = $"{X},{Y},{W},{H},{objectClass}";
                System.IO.File.WriteAllText($"{savePath}\\image{imageIndex}.txt", boundingBox);
                clickCounter = 0;
                canDraw = false;
            }
        }

        private void displayImage(int imageIndex)
        {
            // Empties pictureBox before adding new 
            if (pictureBox1.Image != null)
            { pictureBox1.Image.Dispose(); }
            // Add new image to pictureBox
            string filePath = $"{folderPath}\\eye{String.Format("{0:00000}", imageIndex)}.png";
            Image eyeImage = Image.FromFile(filePath);
            pictureBox1.Image = eyeImage;
            textBox1.Text = $"{imageIndex}/{numImages}";
            // Display previously saved bBoxes
            if (File.Exists($"{savePath}\\image{imageIndex}.txt") == true)
            {
                string[] boxes = File.ReadAllLines($"{savePath}\\image{imageIndex}.txt");
                for(int i = 0; i < boxes.Length; i++)
                {
                    // Need to extract more than 1 char, number is multiple chars
                    string bBox = boxes[i];
                    int X = (int)Math.Round(((Convert.ToSingle(bBox[1]) + 1) / 2) * width);
                    int Y = (int)Math.Round(((Convert.ToSingle(bBox[3]) + 1) / 2) * height);
                    int W = (int)Math.Round(((Convert.ToSingle(bBox[5]) + 1) / 2) * width);
                    int H = (int)Math.Round(((Convert.ToSingle(bBox[7]) + 1) / 2) * height);
                    using (Pen pen = new Pen(Color.Blue, 1))
                    {
                        pictureBox1.CreateGraphics().DrawRectangle(pen, X, Y, W, H);
                    }
                }
            }        
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!canDraw) return;
            int x = Math.Min(startX, e.X);
            int y = Math.Min(startY, e.Y);
            int width = Math.Max(startX, e.X) - Math.Min(startX, e.X);
            int height = Math.Max(startY, e.Y) - Math.Min(startY, e.Y);
            rekt = new Rectangle(x, y, width, height);
            Refresh();
        }

        private void writeTextBox3(int numImAnno)
        {
            textBox3.Text = $"{numImAnno} images annotated.";
        }

        private void deleteBtn_Click(object sender, EventArgs e)
        {
            // Delete file and remove rects from picture box
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            using (Pen pen = new Pen(Color.Red, 1))
            {
                e.Graphics.DrawRectangle(pen, rekt);
            }
        }

    }

}
