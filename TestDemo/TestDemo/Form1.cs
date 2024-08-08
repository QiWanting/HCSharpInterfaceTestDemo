using HalconDotNet;
using HCSharpInterface.matching;
using HCSharpInterface.Model;
using System;
using System.Windows.Forms;

namespace TestDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MouseWheel += HSmartWindowControl1_MouseWheel;
        }

        private void HSmartWindowControl1_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                MouseEventArgs newe = new MouseEventArgs(e.Button, e.Clicks, e.X - Location.X, e.Y - Location.Y, e.Delta);
                if (hSmartWindowControl1.RectangleToScreen(hSmartWindowControl1.ClientRectangle).Contains(MousePosition))
                {
                    hSmartWindowControl1.HSmartWindowControl_MouseWheel(sender, newe);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message + "/n" + ex.StackTrace); }
        }

        private void ButtonSelectFile_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openfile = new OpenFileDialog();
                HObject hObject = new HObject();

                if (openfile.ShowDialog() == DialogResult.OK && (openfile.FileName != ""))
                {
                    HOperatorSet.ReadImage(out hObject, openfile.FileName);
                    hSmartWindowControl1.HalconWindow.DispObj(hObject);
                }
                openfile.Dispose();
                hObject.Dispose();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message + "/n" + ex.StackTrace); }
        }

        /// <summary>
        /// 测试代码部分
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonTest_Click(object sender, EventArgs e)
        {
            try
            {
                string testPath = "D:\\own\\2024\\4G\\Code\\新建文件夹\\DTE_ImageProcessing\\src\\HCSharpInterfaceTests\\matching\\Test";
                HObject hObject = new HObject();
                HOperatorSet.ReadImage(out hObject, testPath + $"/nike.bmp");
                HImage hImage = new HImage(hObject);
                RectangleData rectangleData = new RectangleData(
                    (float)138.962, (float)77.9856, (float)459.056, (float)606.83);
                Matching matching = new Matching(hImage, 0, 360, 0.35, rectangleData, 5);
                matching.CreateShapeModel();

                HObject imageSearch = new HObject();
                HOperatorSet.ReadImage(out imageSearch, testPath + $"/1.bmp");
                HImage image = new HImage(imageSearch);
                (ReturnCode returnCode, double[,] transMatrixList) returnData = matching.FindShapeModel(image);

                //matching.PreviewModelPoint();
                //(ReturnCode returnCode, HObject previewModelImage) result = class1.PreviewModelPoint(matching.SourceImage, 138.962, 77.9856, 459.056, 606.83, matching.ModelID);
                //HObject resultContours = class1.contours;
                HObject imagePrint = new HObject();
                HOperatorSet.ReadImage(out imagePrint, testPath + $"/1_Green.png");
                hImage = new HImage(imagePrint);

                (ReturnCode returnCode, HImage composeImage, RectangleData imageRect) resultMatchingData = matching.GenMatchingImage(returnData.transMatrixList, hImage);

                //获取图像及显示窗口长宽
                HOperatorSet.GetImageSize(resultMatchingData.composeImage, out HTuple imgWidth, out HTuple imgHeight);
                int wndWidth = hSmartWindowControl1.ClientRectangle.Width;
                int wndHeight = hSmartWindowControl1.ClientRectangle.Height;

                //计算比例
                double scale = Math.Max(1.0 * imgWidth.I / wndWidth, 1.0 * imgHeight / wndHeight);
                double w = wndWidth * scale;
                double h = wndHeight * scale;
                //居中时，Part的区域
                hSmartWindowControl1.HalconWindow.SetPart(-(h - imgHeight) / 2, -(w - imgWidth) / 2, imgHeight + (h - imgHeight.D) / 2, imgWidth + (w - imgWidth) / 2);

                //背景色
                hSmartWindowControl1.HalconWindow.SetWindowParam("background_color", "black");
                hSmartWindowControl1.HalconWindow.ClearWindow();

                hSmartWindowControl1.HalconWindow.DispObj(image);
                //hSmartWindowControl1.HalconWindow.DispObj(resultMatchingData.composeImage);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "/n" + ex.StackTrace);
            }
        }
    }
}
