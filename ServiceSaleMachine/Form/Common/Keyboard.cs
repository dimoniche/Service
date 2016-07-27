using System;
using System.Drawing;
using System.Windows.Forms;

namespace ServiceSaleMachine
{
    public partial class Keyboard : UserControl
    {
        public TableLayoutPanel Table { get { return table; } set { table = value; } }

        public delegate void KeyboardEventHandler(object sender, KeyBoardEventArgs e);

        // событие обновления данных
        public event KeyboardEventHandler KeyboardEvent;

        public int CountRow { get { return table.RowCount; }  set { table.RowCount = value; } }
        public int CountCol { get { return table.ColumnCount; } set { table.ColumnCount = value; } }

        public Keyboard()
        {
            InitializeComponent();
        }

        public Control GetAnyControlAt(int column, int row)
        {
            foreach (Control control in Table.Controls)
            {
                var cellPosition = Table.GetCellPosition(control);
                if (cellPosition.Column == column && cellPosition.Row == row)
                    return control;
            }
            return null;
        }

        public void LoadPicture(string[,] str)
        {
            table.SuspendLayout();

            table.ColumnStyles.Clear();
            table.RowStyles.Clear();

            for (int i = 0; i < table.RowCount; i++)
            {
                table.RowStyles.Add(new RowStyle(SizeType.Percent, ((float)100.0) / table.RowCount));

                for (int j = 0; j < table.ColumnCount; j++)
                {
                    table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, ((float)100.0) / table.ColumnCount));

                    // добавим картинки в ячейки
                    PictureBox pbx = new PictureBox();
                    pbx.Dock = DockStyle.Fill;
                    pbx.SizeMode = PictureBoxSizeMode.StretchImage;
                    if (str[i, j] != null)
                    {
                        pbx.Load(str[i, j]);
                    }

                    pbx.Click += table_Click;
                    pbx.Tag = new Point(j,i);

                    table.Controls.Add(pbx, j, i);
                }
            }

            table.ResumeLayout();
        }

        private void table_Click(object sender, EventArgs e)
        {
            Point cellPos = (Point)((PictureBox)sender).Tag;

            KeyboardEvent(this, new KeyBoardEventArgs(cellPos));
        }
    }

    public class KeyBoardEventArgs : EventArgs
    {
        public Point Message { get; private set; }

        public KeyBoardEventArgs(Point message)
        {
            Message = message;
        }
    }
}
