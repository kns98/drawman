using System;
using SkiaSharp;

public class HumanBodyDrawing
{
    private float headSize = 50;
    private float bodyHeight = 200;
    private float bodyWidth = 80;
    private float armLength = 100;
    private float legLength = 120;
    private float eyeSize = 10;

    public void Run(string outputFilePath = "human_body.png")
    {
        // Set canvas dimensions
        int canvasWidth = 800;
        int canvasHeight = 600;

        // Create an image surface
        using (var surface = SKSurface.Create(new SKImageInfo(canvasWidth, canvasHeight)))
        {
            var canvas = surface.Canvas;

            // Clear the canvas
            canvas.Clear(SKColors.White);

            // Draw the human figure
            float centerX = canvasWidth / 2;
            float topY = 50;

            DrawHead(canvas, centerX, topY);
            DrawEyes(canvas, centerX, topY);
            DrawBody(canvas, centerX, topY);
            DrawArms(canvas, centerX, topY);
            DrawLegs(canvas, centerX, topY);

            // Save the output to a file
            using (var image = surface.Snapshot())
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            {
                using (var stream = System.IO.File.OpenWrite(outputFilePath))
                {
                    data.SaveTo(stream);
                }
            }
        }

        Console.WriteLine($"Human body drawing saved to {outputFilePath}");
    }

    private void DrawHead(SKCanvas canvas, float centerX, float topY)
    {
        float headX = centerX - headSize / 2;
        float headY = topY;
        canvas.DrawOval(headX + headSize / 2, headY + headSize / 2, headSize / 2, headSize / 2, new SKPaint { Color = SKColors.LightBlue });
    }

    private void DrawEyes(SKCanvas canvas, float centerX, float topY)
    {
        float headX = centerX - headSize / 2;
        float headY = topY;

        float leftEyeX = headX + headSize / 4;
        float leftEyeY = headY + headSize / 3;
        float rightEyeX = headX + 3 * headSize / 4;
        float rightEyeY = leftEyeY;

        var eyePaint = new SKPaint { Color = SKColors.White };
        canvas.DrawCircle(leftEyeX, leftEyeY, eyeSize, eyePaint);
        canvas.DrawCircle(rightEyeX, rightEyeY, eyeSize, eyePaint);
    }

    private void DrawBody(SKCanvas canvas, float centerX, float topY)
    {
        float bodyX = centerX - bodyWidth / 2;
        float bodyY = topY + headSize;
        var bodyPaint = new SKPaint { Color = SKColors.LightGreen };
        canvas.DrawRect(bodyX, bodyY, bodyWidth, bodyHeight, bodyPaint);
    }

    private void DrawArms(SKCanvas canvas, float centerX, float topY)
    {
        float bodyX = centerX - bodyWidth / 2;
        float bodyY = topY + headSize;

        float leftArmStartX = bodyX;
        float leftArmStartY = bodyY + bodyHeight / 4;
        float leftArmEndX = leftArmStartX - armLength;
        float leftArmEndY = leftArmStartY;

        float rightArmStartX = bodyX + bodyWidth;
        float rightArmStartY = leftArmStartY;
        float rightArmEndX = rightArmStartX + armLength;
        float rightArmEndY = rightArmStartY;

        var armPaint = new SKPaint { Color = SKColors.Black, StrokeWidth = 4 };
        canvas.DrawLine(leftArmStartX, leftArmStartY, leftArmEndX, leftArmEndY, armPaint);
        canvas.DrawLine(rightArmStartX, rightArmStartY, rightArmEndX, rightArmEndY, armPaint);
    }

    private void DrawLegs(SKCanvas canvas, float centerX, float topY)
    {
        float bodyY = topY + headSize;
        float leftLegStartX = centerX - bodyWidth / 4;
        float leftLegStartY = bodyY + bodyHeight;
        float leftLegEndX = leftLegStartX - legLength / 4;
        float leftLegEndY = leftLegStartY + legLength;

        float rightLegStartX = centerX + bodyWidth / 4;
        float rightLegStartY = leftLegStartY;
        float rightLegEndX = rightLegStartX + legLength / 4;
        float rightLegEndY = rightLegStartY + legLength;

        var legPaint = new SKPaint { Color = SKColors.Black, StrokeWidth = 4 };
        canvas.DrawLine(leftLegStartX, leftLegStartY, leftLegEndX, leftLegEndY, legPaint);
        canvas.DrawLine(rightLegStartX, rightLegStartY, rightLegEndX, rightLegEndY, legPaint);
    }

    public static void Main(string[] args)
    {
        var drawing = new HumanBodyDrawing();
        drawing.Run();
    }
}
