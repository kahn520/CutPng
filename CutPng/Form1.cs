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

namespace CutPng
{
    public partial class Form1 : Form
    {
        private Point[] points = new Point[3]
        {
            new Point(720, 405),
            new Point(210, 117),
            new Point(120, 67),
        };

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtInput.Text != "" && Directory.Exists(txtInput.Text))
            {
                DoWork(txtInput.Text);
            }
            else
            {
                FolderBrowserDialog folderDialog = new FolderBrowserDialog();
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    DoWork(folderDialog.SelectedPath);
                }
            }
        }

        void DoWork(string strPath)
        {
            string[] strFiles = Directory.GetFiles(strPath);
            strFiles = strFiles.Where(f => Path.GetExtension(f) == ".png" || Path.GetExtension(f) == ".jpg").ToArray();
            strFiles.ToList().ForEach(f =>
            {
                string strExtension = Path.GetExtension(f);
                string strDirectory = Path.GetDirectoryName(f) + "\\";
                string strName = Path.GetFileNameWithoutExtension(f);
                string strBig = $"{strDirectory}{strName}_1{strExtension}";
                string strMiddle = $"{strDirectory}m_{strName}_1{strExtension}";
                string strSmall = $"{strDirectory}1_{strName}_1{strExtension}";

                Image imgSource = Image.FromFile(f);
                bool bIsTransparent = IsTransparent(imgSource);

                SaveImageBig(imgSource, strBig, bIsTransparent);
                SaveBigMiddle(imgSource, strMiddle, bIsTransparent);
                SaveImageSmall(imgSource, strSmall, bIsTransparent);
                imgSource.Dispose();
            });
            MessageBox.Show(@"完成");
        }

        bool IsTransparent(Image img)
        {
            Bitmap bitmap = new Bitmap(img);
            for (int i = 0; i < bitmap.Width; i++)
            {
                if (bitmap.GetPixel(i, 0).A == 0)
                {
                    return true;
                }
                if (bitmap.GetPixel(i, bitmap.Height - 1).A == 0)
                {
                    return true;
                }
            }
            for (int i = 0; i < bitmap.Height; i++)
            {
                if (bitmap.GetPixel(0, i).A == 0)
                {
                    return true;
                }
                if (bitmap.GetPixel(bitmap.Width - 1, i).A == 0)
                {
                    return true;
                }
            }
            bitmap.Dispose();
            return false;
        }

        void SaveImageBig(Image img, string strPath, bool bIsTransparent)
        {
            SaveImage(img, strPath, points[0].X, points[0].Y, bIsTransparent);
        }

        void SaveBigMiddle(Image img, string strPath, bool bIsTransparent)
        {
            SaveImage(img, strPath, points[1].X, points[1].Y, bIsTransparent);
        }

        void SaveImageSmall(Image img, string strPath, bool bIsTransparent)
        {
            SaveImage(img, strPath, points[2].X, points[2].Y, bIsTransparent);
        }

        void SaveImage(Image img, string strPath, int width, int height, bool bIsTransparent)
        {
            Bitmap bitmap = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.White);
            if (bIsTransparent)
            {
                bitmap.MakeTransparent(Color.White);
            }
            Size sizeDraw = GetDrawSize(width, height, img.Width, img.Height);
            g.DrawImage(img, (int) (width - sizeDraw.Width)/2, (int) (height - sizeDraw.Height)/2, sizeDraw.Width, sizeDraw.Height);
            g.Save();
            g.Dispose();
            bitmap.Save(strPath);
            bitmap.Dispose();
        }

        Size GetDrawSize(float widthPaper, float heightPaper, float widthImg, float heightImg)
        {
            if (widthImg <= widthPaper && heightImg <= heightPaper)
            {
                return new Size((int) widthImg, (int) heightImg);
            }

            int width = (int)widthPaper;
            int height = (int) (heightImg/(widthImg/widthPaper));

            if (height > heightPaper)
            {
                height = (int) heightPaper;
                width = (int) (widthImg/(heightImg/heightPaper));
            }
            return new Size(width, height);
        }
    }
}
