using System;
using System.Drawing;
using System.IO;
using ImageMagick;

/// <summary>
/// Represents a 3D vector with double precision components.
/// </summary>
public struct Vector3f
{
    public double X { get; }
    public double Y { get; }
    public double Z { get; }

    public Vector3f(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public double this[int index]
    {
        get
        {
            double v = index switch
            {
                0 => X,
                1 => Y,
                2 => Z,
                _ => throw new IndexOutOfRangeException(),
            };
            return v;
        }
    }

    public static Vector3f Zero => new Vector3f(0, 0, 0);
    public static Vector3f OneX => new Vector3f(1, 0, 0);
    public static Vector3f OneY => new Vector3f(0, 1, 0);
    public static Vector3f OneZ => new Vector3f(0, 0, 1);

    public static Vector3f operator -(Vector3f a) => new Vector3f(-a.X, -a.Y, -a.Z);
    public static Vector3f operator -(Vector3f a, Vector3f b) => new Vector3f(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    public static Vector3f operator +(Vector3f a, Vector3f b) => new Vector3f(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public static Vector3f operator *(Vector3f a, double scalar) => new Vector3f(a.X * scalar, a.Y * scalar, a.Z * scalar);
    public static Vector3f operator /(Vector3f a, double scalar) => new Vector3f(a.X / scalar, a.Y / scalar, a.Z / scalar);

    public static double Dot(Vector3f a, Vector3f b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

    public static Vector3f Cross(Vector3f a, Vector3f b) => new Vector3f(
        a.Y * b.Z - a.Z * b.Y,
        a.Z * b.X - a.X * b.Z,
        a.X * b.Y - a.Y * b.X);

    public static Vector3f Unitize(Vector3f vector)
    {
        double length = Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
        return vector / length;
    }

    public double Length() => Math.Sqrt(X * X + Y * Y + Z * Z);

    public static Vector3f Reflect(Vector3f incident, Vector3f normal)
    {
        return incident - normal * 2 * Dot(incident, normal);
    }

    internal bool IsZero()
    {
        return X == 0 && Y == 0 && Z == 0;
    }
}

class Program
{
    static void Main(string[] args)
    {
        int frameCount = 60;
        int width = 800;
        int height = 600;
        string outputFolder = "frames";
        string outputFilePath = "animated_human_video.mp4";

        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }

        for (int frame = 0; frame < frameCount; frame++)
        {
            using (var frameImage = GenerateFrame(frame, width, height))
            {
                string frameFilePath = Path.Combine(outputFolder, $"frame_{frame:D3}.png");
                frameImage.Save(frameFilePath, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        Console.WriteLine("Frames saved. Creating video...");

        using (var collection = new MagickImageCollection())
        {
            for (int frame = 0; frame < frameCount; frame++)
            {
                string frameFilePath = Path.Combine(outputFolder, $"frame_{frame:D3}.png");
                var magickImage = new MagickImage(frameFilePath);
                magickImage.AnimationDelay = 10;
                collection.Add(magickImage);
            }

            collection.Write(outputFilePath);
        }

        Console.WriteLine($"Video saved to {outputFilePath}");
    }

    static Bitmap GenerateFrame(int frame, int width, int height)
    {
        Bitmap bitmap = new Bitmap(width, height);
        using (Graphics g = Graphics.FromImage(bitmap))
        {
            g.Clear(Color.White);

            double headTilt = Math.Sin(frame * 0.1) * 15;
            double armAngle = Math.Sin(frame * 1);

            int centerX = width / 2;
            int centerY = height / 2;

            DrawNeck(g, new Vector3f(centerX, centerY - 100, 0));
            DrawHead(g, new Vector3f(centerX, centerY - 150, 0), headTilt);
            DrawBody(g, new Vector3f(centerX, centerY, 0));
            DrawArm(g, new Vector3f(centerX - 50, centerY - 50, 0), Vector3f.OneX * armAngle);
            DrawArm(g, new Vector3f(centerX + 50, centerY - 50, 0), Vector3f.OneX * -armAngle);
            DrawLeg(g, new Vector3f(centerX - 20, centerY + 100, 0), Vector3f.OneY * armAngle);
            DrawLeg(g, new Vector3f(centerX + 20, centerY + 100, 0), Vector3f.OneY * -armAngle);
        }
        return bitmap;
    }

    static void DrawNeck(Graphics g, Vector3f position)
    {
        g.FillRectangle(Brushes.LightBlue, (float)position.X - 10, (float)position.Y - 20, 20, 20);
    }

    static void DrawHead(Graphics g, Vector3f position, double tilt)
    {
        g.TranslateTransform((float)position.X, (float)position.Y);
        g.RotateTransform((float)tilt);
        g.FillEllipse(Brushes.LightBlue, -50, -50, 100, 100);

        g.FillEllipse(Brushes.Black, -30, -20, 20, 20);
        g.FillEllipse(Brushes.Black, 10, -20, 20, 20);

        g.DrawArc(new Pen(Color.Black, 2), -20, 10, 40, 20, 0, -180);
        g.ResetTransform();
    }

    static void DrawBody(Graphics g, Vector3f position)
    {
        g.FillRectangle(Brushes.LightGreen, (float)position.X - 30, (float)position.Y - 100, 60, 200);
    }

    static void DrawArm(Graphics g, Vector3f position, Vector3f offset)
    {
        Vector3f endPoint = position + offset;
        g.DrawLine(new Pen(Color.Black, 4), (float)position.X, (float)position.Y, (float)endPoint.X, (float)endPoint.Y);
    }

    static void DrawLeg(Graphics g, Vector3f position, Vector3f offset)
    {
        Vector3f endPoint = position + offset;
        g.DrawLine(new Pen(Color.Black, 4), (float)position.X, (float)position.Y, (float)endPoint.X, (float)endPoint.Y);
    }
}
