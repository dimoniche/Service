using System;
using System.Drawing;
using System.Windows.Forms;

namespace ServiceSaleMachine
{
    public class ScalableLabel : Control
    {
        public ScalableLabel()
        {
            base.AutoSize = false;
        }

        public override bool AutoSize
        {
            get { return false; }
            set { }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            StringFormat string_Format = new StringFormat();
            string_Format.Alignment = StringAlignment.Center;
            string_Format.LineAlignment = StringAlignment.Center;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            var size = e.Graphics.MeasureString(Text, Font);
            var kx = 1f * ClientSize.Width / size.Width;
            var ky = 1f * ClientSize.Height / size.Height;
            var k = Math.Min(1, Math.Min(kx, ky));
            e.Graphics.ScaleTransform(k, k);
            using (var brush = new SolidBrush(ForeColor))
                e.Graphics.DrawString(Text, Font, brush, size.Width / 2, size.Height / 2, string_Format);
        }

        protected override void OnResize(EventArgs e)
        {
            Refresh();
        }

        protected override void OnTextChanged(EventArgs e)
        {
            Refresh();
        }
    }
}
