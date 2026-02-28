using System.Drawing.Imaging;
using System.Timers;

public class FadingPictureBox : Control
{
    private Image image;
    private float alpha = 1f; // 0 = transparent, 1 = opaque
    private bool fadingIn = true;
    private float step = 0.02f; // amount alpha changes per tick
    private System.Timers.Timer fadeTimer;

    public FadingPictureBox()
    {
        this.DoubleBuffered = true;
        fadeTimer = new System.Timers.Timer(30); // 30ms per frame
        fadeTimer.Elapsed += FadeTimer_Elapsed;
    }

    // Set the image to display
    public void SetImage(Image img)
    {
        this.image = img;
        this.Invalidate();
    }

    // Start fade in or out over time
    public void Fade(bool fadeIn, int durationMs)
    {
        fadingIn = fadeIn;
        step = 20f / durationMs * (fadeIn ? 1 : -1);
        alpha = fadeIn ? 0f : 1f;
        fadeTimer.Start();
    }

    private void FadeTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        alpha += step;

        if (alpha >= 1f)
        {
            alpha = 1f;
            fadeTimer.Stop();
        }
        else if (alpha <= 0f)
        {
            alpha = 0f;
            fadeTimer.Stop();
        }

        this.Invalidate(); // trigger repaint
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        if (image == null) return;

        // Draw image with current alpha
        ColorMatrix matrix = new ColorMatrix();
        matrix.Matrix33 = alpha;
        ImageAttributes attributes = new ImageAttributes();
        attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

        e.Graphics.DrawImage(
            image,
            new Rectangle(0, 0, this.Width, this.Height),
            0, 0, image.Width, image.Height,
            GraphicsUnit.Pixel,
            attributes
        );
    }
}

