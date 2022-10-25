using safe4.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Drawing.Image;


namespace safe4
{
    public partial class Form1 : Form
    {
        private bool[,] addMatrix = new bool[5, 5];         //  matrix creating
        private Button[,] arrButtons;                       // creating buttons array
        private int difficaltyLevel = 3;                    // default field is level hard

        public Form1()
        {
            for (int i = 0; i < 5; i++)   // filling matrix
            {
                for (int j = 0; j < 5; j++)
                {
                    addMatrix[i, j] = true;
                    if (i == j)
                    {
                        addMatrix[i, j] = false;
                    }
                }
            }
            arrButtons = new Button[5, 5];
            int counter = 0;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    arrButtons[i, j] = new Button();
                    counter++;
                    arrButtons[i, j].Click += ButtonAction_OnClick;
                    arrButtons[i, j].Tag = counter;
                }
            }
            InitializeComponent();
            labelOfWin.Visible = false;
            ShakeMatrix(addMatrix);
            ChangeField(arrButtons, addMatrix);    // shows field for start
            buttonEasy.Tag = 1;
            buttonMedium.Tag = 2;
            buttonHard.Tag = 3;
            buttonEasy.Click += BntDifficaltyLevelOnClick;
            buttonMedium.Click += BntDifficaltyLevelOnClick;
            buttonHard.Click += BntDifficaltyLevelOnClick;
        }

        public void BntDifficaltyLevelOnClick(object sender, EventArgs e)  // catches  clicks on difficalty level buttons
        {
            ShakeMatrix(addMatrix);
            difficaltyLevel = Convert.ToInt32((sender as Button).Tag);
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    arrButtons[i, j].Visible = true;
                    if (difficaltyLevel == 2 && (i > 3 || j > 3))
                    {
                        arrButtons[i, j].Visible = false;
                    }
                    else
                    {
                        if (difficaltyLevel == 1 && (i > 2 || j > 2))
                        {
                            arrButtons[i, j].Visible = false;
                        }
                    }
                }
            }
            ChangeField(arrButtons, addMatrix);
        }

    private static void ChangeColumnsAndRows(bool[,] addMatrix, int column, int row)  // change columns in matrix. called in func "changeMatrix"
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (i == column - 1)
                    {
                        if (addMatrix[i, j] is false)
                        {
                            addMatrix[i, j] = true;
                        }
                        else
                        {
                            addMatrix[i, j] = false;
                        }
                    }
                    if (j == row - 1)
                    {
                        if (addMatrix[i, j] is false)
                        {
                            addMatrix[i, j] = true;
                        }
                        else
                        {
                            addMatrix[i, j] = false;
                        }
                    }
                }
            }
            if (addMatrix[column - 1, row - 1] is false)
            {
                addMatrix[column - 1, row - 1] = true;
            }
            else
            {
                addMatrix[column - 1, row - 1] = false;
            }
        }

        private static void ChangeMatrix(int clickedCell, bool[,] addMatrix) //  gets number of cell and changes matrix
        {
            int count = 0, column, row;       // counter for convert number om button to double arrays index (int xy = int[x, y])
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    count++;
                    if (count == clickedCell)
                    {
                        column = i + 1;
                        row = j + 1;
                        ChangeColumnsAndRows(addMatrix, column, row);
                    }
                }
            }
        }

        private static void ChangeField(Button[,] arrButtons, bool[,] addMatrix)   // shows all new changes on the gaming field
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (arrButtons[i, j].Image == Resources.boneSmallHorizontal && arrButtons[i, j].BackColor == Color.Red)
                    {
                        arrButtons[i, j].Image = Resources.boneSmallVertical;
                        arrButtons[i, j].BackColor = Color.Blue;
                    }
                    else
                    {
                        arrButtons[i, j].Image = Resources.boneSmallHorizontal;
                        arrButtons[i, j].BackColor = Color.Red;
                    }
                    arrButtons[i, j].Image = GetPositionInfo(addMatrix[i, j]);
                    arrButtons[i, j].BackColor = GetColorInfo(addMatrix[i, j]);
                }
            }
        }

        public static Bitmap GetPositionInfo(bool cell)  // tells to button about what image it has
        {
            if (cell is true)
            {
                return Resources.boneSmallHorizontal;
            }
            else
            {
                return Resources.boneSmallVertical;
            }
        }

        public static Color GetColorInfo(bool cell)   // tells to button about what color it should be
        {
            if (cell is true)
            {
                return Color.Red;
            }
            else
            {
                return Color.Blue;
            }
        }

        public void ButtonAction_OnClick(object sender, EventArgs e)  // catches  clics on buttons on gaming field
        {
            int index = Convert.ToInt32((sender as Button).Tag);
            ChangeMatrix(index, addMatrix);
            ChangeField(arrButtons, addMatrix);
            WinLabel(addMatrix, difficaltyLevel);   // checks win
            labelOfWin.Visible = WinLabel(addMatrix, difficaltyLevel);  // shows label 
        }

        private static void ShakeMatrix(bool[,] addMatrix)  // shakes matrix
        {
            Random rnd = new Random();
            int[] expect = new int[13] { 1, 2, 3, 6, 7, 8, 11, 12, 13, 19, 20, 24, 25 };  // for shakes only 3x3 field (level easy)
            for (int i = 0; i < rnd.Next(77, 777); i++)
            {
                ChangeMatrix(expect[rnd.Next(1, 13)], addMatrix);
            }
            int countSame = 4;    // for protect against creating autowin field
            while (countSame == 4 || countSame == 5 || countSame == 9 || countSame == 0)
            {
                countSame = 0;
                ChangeMatrix(expect[rnd.Next(1, 8)], addMatrix);
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (addMatrix[i, j] is true)
                        {
                            countSame++;
                        }
                    }
                }
            }
        }

        private void ButtonShake_Click(object sender, EventArgs e)
        {
            ShakeMatrix(addMatrix);
            ChangeField(arrButtons, addMatrix);
            labelOfWin.Visible = false;
        }

        private static bool WinLabel(bool[,] addMatrix, int difficaltyLevel)  // opens finish-game label when win
        {
            int winEasy = 0, winMedium = 0, winHard = 0;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (addMatrix[0, 0] == addMatrix[i, j])
                    {
                        if (difficaltyLevel == 1 && i < 3 && j < 3)
                        {
                            winEasy++;
                            if (winEasy == 9)
                            {
                                return true;
                            }
                        }
                        if (difficaltyLevel == 2 && i < 4 && j < 4)
                        {
                            winMedium++;
                            if (winMedium == 16)
                            {
                                return true;
                            }
                        }
                        if (difficaltyLevel == 3)
                        {
                            winHard++;
                            if (winHard == 25)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}