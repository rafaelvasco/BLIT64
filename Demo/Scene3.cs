using System;
using BLIT64;

namespace Demo
{
    public class Scene3 : Scene
    {
        private const float pi_8 = (float) (Math.PI / 8);
        private const float pi_2 = (float) (Math.PI * 2);
        private float t;
        private const int size = 350;

        public override void Load()
        {
        }

        public override void Update()
        {
            t += 0.5f;
        }

        public override void Draw(Canvas canvas)
        {
            canvas.Clear();

            // Lines
            for (float i = t % 8; i < size; i += 8)
            {
                canvas.SetColor(33);
                canvas.Line((int)i, 0, 0, (int)(size - i));

                canvas.SetColor(9);
                canvas.Line((int)i, size, size, (int)(size - i));
            }

            // Prism

            canvas.SetColor(35);
            for (float i = (t / 64) % pi_8; i < pi_2; i += pi_8)
            {
                var x = (size / 2f) + (size / 4f) * Math.Cos(i);
                var y = (size / 2f) + (size / 4f) * Math.Cos(i);
                canvas.Line(size, 0, (int)x, (int)y);
                canvas.Line(0, size, (int)x, (int)y);
            }

            // Border

            canvas.SetColor(33);
            canvas.Line(0, 0, size, 0);
            canvas.Line(0, 0, 0, size);

            canvas.SetColor(9);
            canvas.Line(size, 0, size, size);
            canvas.Line(0, size, size, size);

        }
    }
}
