using ImageProcessor;

using System;

using System.Collections.Generic;

using System.Drawing;

using System.Linq;

using System.Windows.Forms;

using static ImageProcessor.Processor;



namespace ImageProcessorApp

{

    public partial class Form1 : Form

    {

        private PictureBox pictureBox;

        private Bitmap originalImage;

        private Bitmap resultImage;



        public Form1()

        {

            this.Text = "Поиск пути";

            this.Size = new Size(1000, 700);



            pictureBox = new PictureBox

            {

                Location = new Point(10, 10),

                Size = new Size(960, 540),

                BorderStyle = BorderStyle.FixedSingle,

                SizeMode = PictureBoxSizeMode.Zoom

            };



            Button btnOpen = new Button { Text = "Открыть", Location = new Point(10, 560), Size = new Size(100, 40) };

            Button btnFind = new Button { Text = "Найти путь", Location = new Point(120, 560), Size = new Size(100, 40) };

            Button btnSave = new Button { Text = "Сохранить", Location = new Point(230, 560), Size = new Size(100, 40) };



            btnOpen.Click += (s, e) =>

            {

                OpenFileDialog dlg = new OpenFileDialog();

                dlg.Filter = "Images|*.bmp;*.jpg;*.png;*.jpeg";

                if (dlg.ShowDialog() == DialogResult.OK)

                {

                    originalImage = new Bitmap(dlg.FileName);

                    pictureBox.Image = originalImage;

                }

            };



            btnFind.Click += (s, e) =>

            {

                if (originalImage == null) return;



                var objects = Processor.ProcessImage(originalImage);

                FoundObject start = null;

                foreach (var obj in objects)

                {

                    if (obj.IsStart)

                    {

                        start = obj;

                        break;

                    }

                }



                if (start == null)

                {

                    MessageBox.Show("Красная стрелка не найдена!");

                    return;

                }



                resultImage = new Bitmap(originalImage);

                List<FoundObject> path = new List<FoundObject>();

                path.Add(start);



                FoundObject current = start;

                HashSet<FoundObject> used = new HashSet<FoundObject>();

                used.Add(start);



                using (Graphics g = Graphics.FromImage(resultImage))

                {

                    Pen whitePen = new Pen(Color.White, 3);

                    for (int step = 0; step < 20; step++)

                    {

                        FoundObject next = null;

                        double minDist = 999999;



                        foreach (var obj in objects)

                        {

                            if (used.Contains(obj)) continue;



                            double dx = obj.Center.X - current.Center.X;

                            double dy = obj.Center.Y - current.Center.Y;

                            double dist = Math.Sqrt(dx * dx + dy * dy);



                            if (dist < 30 || dist > 300) continue;

                            double angleToObj = Math.Atan2(dy, dx);

                            double angleDiff = Math.Abs(angleToObj - current.Angle);

                            if (angleDiff > Math.PI) angleDiff = 2 * Math.PI - angleDiff;

                            if (angleDiff < 0.8)

                            {

                                if (dist < minDist)

                                {

                                    minDist = dist;

                                    next = obj;

                                }

                            }

                        }



                        if (next == null) break;

                        g.DrawLine(whitePen, current.Center, next.Center);



                        path.Add(next);

                        used.Add(next);

                        current = next;

                    }

                    {

                        FoundObject last = path[path.Count - 1];

                        g.DrawRectangle(new Pen(Color.Blue, 3),

                            last.Center.X - 30,

                            last.Center.Y - 30,

                            60, 60);

                    }

                    g.DrawEllipse(new Pen(Color.Red, 2),

                        start.Center.X - 15,

                        start.Center.Y - 15,

                        30, 30);

                }



                pictureBox.Image = resultImage;



            };



            btnSave.Click += (s, e) =>

            {

                if (resultImage == null) return;



                SaveFileDialog dlg = new SaveFileDialog();

                dlg.Filter = "JPEG|*.jpg|PNG|*.png|BMP|*.bmp";

                if (dlg.ShowDialog() == DialogResult.OK)

                {

                    resultImage.Save(dlg.FileName);

                }

            };



            this.Controls.Add(pictureBox);

            this.Controls.Add(btnOpen);

            this.Controls.Add(btnFind);

            this.Controls.Add(btnSave);

        }

    }

}