using Login;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TicTacToe
{
    public partial class Form_TicTacToe : Form
    {
        private readonly int x = 0;
        private readonly int y = 0;
        private readonly Size btnSize = new Size(40, 40);
        private readonly Char[] alphaLabels = new Char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S' };

        private Button[,] btnGrid = new Button[14, 18];

        public Form_TicTacToe()
        {
            InitializeComponent();

            ControlMove.ControlEventSubscription(this);

            for (int row = 0; row < btnGrid.GetLength(0); row++)
            {
                for (int column = 0; column < btnGrid.GetLength(1); column++)
                {
                    Button btn = new Button
                    {
                        Size = btnSize,
                        Location = new Point(x, y),
                        Name = String.Format("button_{0}{1}", row + 1, alphaLabels[column]),
                        Enabled = false,
                        TabStop = false,
                        TabIndex = 0
                    };

                    btn.Click += FillXO_Click;
                    btn.Font = new Font(btn.Font.FontFamily, 24f, FontStyle.Bold);

                    btnGrid[row, column] = btn;
                    panel1.Controls.Add(btn);

                    x += btnSize.Width;
                }
                x = 0;
                y += btnSize.Height;
            }
        }

        enum Player { X, O }

        private Player _player = Player.X;
        
        private int scoreX = 0;
        private int scoreO = 0;

        #region BUTTONS

        private void button_NewGame_Click(object sender, EventArgs e)
        {
            foreach (Button item in panel1.Controls)
            {
                item.Enabled = true;
                item.Text = string.Empty;
            }

            Player p = (_player == Player.X) ? Player.O : Player.X;
            PlayerLabelChange(label_PlayerX, label_PlayerO, p);

            button_NewGame.Enabled = false;
            button_ResetScore.Enabled = false;
            button_Restart.Enabled = true;

            label_GameTime.Visible = true;
            label_Mins.Visible = true;
            label_Secs.Visible = true;
            label_MilSecs.Visible = true;
            label_Separator1.Visible = true;
            mins = 0; secs = 0; mils = 0;

            timer1.Start();
        }

        private void button_ResetScore_Click(object sender, EventArgs e)
        {
            scoreO = 0;
            scoreX = 0;
            label_ScorePO.Text = scoreO.ToString();
            label_ScorePX.Text = scoreX.ToString();
        }

        private void button_Restart_Click(object sender, EventArgs e)
        {
            timer1.Stop();

            mins = 0; secs = 0; mils = 0;
            timer1.Start();

            foreach (Button item in btnGrid)
                item.Text = string.Empty;

            NextPlayerTurn(_player);

            Player p = (_player == Player.X) ? Player.O : Player.X;
            PlayerLabelChange(label_PlayerX, label_PlayerO, p);
        }

        #region CLOSE BUTTON

        private void label_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label_Close_MouseHover(object sender, EventArgs e)
        {
            label_Close.ForeColor = Color.WhiteSmoke;
            label_Close.BackColor = Color.Red;
        }

        private void label_Close_MouseLeave(object sender, EventArgs e)
        {
            label_Close.ForeColor = Color.Black;
            label_Close.BackColor = Color.WhiteSmoke;
        }

        #endregion

        #endregion

        #region WIN CONDITIONS // 5 IN A ROW

        private bool WinConditionCheck(Player player)
        {
            if (RowCheck(player) || VerticalCheck(player) || DiagonalCheck(player) || ReversedDiagonalCheck(player))
                return true;

            return false;
        }

        private bool RowCheck(Player player)
        {
            for (int row = 0; row < btnGrid.GetLength(0); row++)
                for (int column = 0; column < btnGrid.GetLength(1); column++)
                    if (CellTextCheck(btnGrid[row, column], player))
                        return true;

            return false;
        }

        private bool VerticalCheck(Player player)
        {
            for (int column = 0; column < btnGrid.GetLength(1); column++)
                for (int row = 0; row < btnGrid.GetLength(0); row++)
                    if(CellTextCheck(btnGrid[row, column], player))
                        return true;

            return false;
        }

        private bool DiagonalCheck(Player player)
        {
            for (int row = 0; row <= btnGrid.GetLength(0) - 5; row++)
                for (int column = 0; column <= btnGrid.GetLength(1) - 5; column++)
                    for (int i = 0; i < 5; i++)
                        if (CellTextCheck(btnGrid[row + i, column + i], player))
                            return true;

            return false;
        }

        private bool ReversedDiagonalCheck(Player player)
        {
            for (int row = 0; row <= btnGrid.GetLength(0) - 5; row++)
                for (int column = btnGrid.GetLength(1) - 1; column >= 4; column--)
                    for (int i = 0; i < 5; i++)
                        if (CellTextCheck(btnGrid[row + i, column - i], player))
                            return true;

            return false;
        }

        int FiveInARowCount = 0;

        private bool CellTextCheck(Button cell, Player player)
        {
            switch (player)
            {
                case Player.X:
                    if (cell.Text == "X")
                    {
                        FiveInARowCount++;
                        if (FiveInARowCount == 5)
                            return true;
                    }
                    else
                        FiveInARowCount = 0;

                    break;
                case Player.O:
                    if (cell.Text == "O")
                    {
                        FiveInARowCount++;
                        if (FiveInARowCount == 5)
                            return true;
                    }
                    else
                        FiveInARowCount = 0;

                    break;
            }

            return false;            
        }

        #endregion

        private Button selectedBtn;

        private void FillXO_Click(object sender, EventArgs e)
        {
            selectedBtn = sender as Button;

            if (selectedBtn.Text == string.Empty)
            {
                label_GameTime.Focus();

                switch (_player)
                {
                    case Player.X:
                        PlayerLabelChange(label_PlayerX, label_PlayerO, Player.X);
                        selectedBtn.ForeColor = Color.DarkGreen;
                        selectedBtn.Text = "X";
                        break;
                    case Player.O:
                        PlayerLabelChange(label_PlayerX, label_PlayerO, Player.O);
                        selectedBtn.ForeColor = Color.Red;
                        selectedBtn.Text = "O";
                        break;
                }

                if (WinConditionCheck(_player))
                {
                    timer1.Stop();

                    label_PlayerX.ForeColor = Color.Black;
                    label_PlayerX.Font = FontOnTurn;

                    label_PlayerO.ForeColor = Color.Black;
                    label_PlayerO.Font = FontOnTurn;

                    switch (_player)
                    {
                        case Player.X:
                            MessageBox.Show("X Won!");
                            scoreX++;
                            label_ScorePX.Text = scoreX.ToString();
                            break;
                        case Player.O:
                            MessageBox.Show("O Won!");
                            scoreO++;
                            label_ScorePO.Text = scoreO.ToString();
                            break;
                    }

                    foreach (Button item in panel1.Controls)
                        item.Enabled = false;

                    button_NewGame.Enabled = true;
                    button_ResetScore.Enabled = true;
                    button_Restart.Enabled = false;
                }
                else
                {
                    NextPlayerTurn(_player);
                }
            }
        }

        private void NextPlayerTurn(Player p)
        {
            switch (p)
            {
                case Player.O:
                    _player = Player.X;
                    break;
                case Player.X:
                    _player = Player.O;
                    break;
            }
        }

        private readonly Font FontOnTurn = new Font("Comic Sans MS", 36f, FontStyle.Bold);
        private readonly Font FontNotTurn = new Font("Comic Sans MS", 24f, FontStyle.Bold);

        private void PlayerLabelChange(Label labelPlayerX, Label labelPlayerO, Player player)
        {
            switch (player)
            {
                case Player.X:
                    labelPlayerO.ForeColor = Color.Red;
                    labelPlayerO.Font = FontOnTurn;

                    labelPlayerX.ForeColor = Color.Black;
                    labelPlayerX.Font = FontNotTurn;
                    break;

                case Player.O:
                    labelPlayerX.ForeColor = Color.DarkGreen;
                    labelPlayerX.Font = FontOnTurn;

                    labelPlayerO.ForeColor = Color.Black;
                    labelPlayerO.Font = FontNotTurn;
                    break;
            }
        }

        private int mins, secs, mils;

        private void timer1_Tick(object sender, EventArgs e)
        {
            mils++;
            if (mils >= 10)
            {
                mils = 0;
                secs++;

                if (secs >= 60)
                {
                    secs = 0;
                    mins++;
                }
            }

            label_Mins.Text = String.Format("{0:00}",mins);
            label_Secs.Text = String.Format("{0:00}",secs);
            label_MilSecs.Text = String.Format("{0:00}",mils);
        }
    }
}
