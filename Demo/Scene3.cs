using System;
using BLIT64;

namespace Demo
{
    public class Scene3 : Scene
    {
        private float pi_8 = (float)(Math.PI / 8);
        private float pi_2 = (float)(Math.PI * 2);
        private float t;
        private int size = 350;

        public override void Update()
        {
            t += 0.5f;
        }

        public override void Draw(Blitter blitter)
        {
            blitter.Clear();

            // Lines
            for (float i = t % 8; i < size; i += 8)
            {
                blitter.Line((int)i, 0, 0, (int)(size - i), 1, 33);
                blitter.Line((int)i, size, size, (int)(size - i), 1, 9);
            }

            // Prism

            for (float i = (t / 64) % pi_8; i < pi_2; i += pi_8)
            {
                var x = (size / 2f) + (size / 4f) * Math.Cos(i);
                var y = (size / 2f) + (size / 4f) * Math.Cos(i);
                blitter.Line(size, 0, (int)x, (int)y, 1, 35);
                blitter.Line(0, size, (int)x, (int)y, 1, 35);

            }


            // Border

            blitter.Line(0, 0, size, 0, 1, 33);
            blitter.Line(0, 0, 0, size, 1, 33);
            blitter.Line(size, 0, size, size, 1, 9);
            blitter.Line(0, size, size, size, 1, 9);

        }
    }
}
