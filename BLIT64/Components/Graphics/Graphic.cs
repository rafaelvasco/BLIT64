﻿namespace BLIT64
{
    public abstract class Graphic
    {
        public abstract void Draw(Canvas blitter, int x, int y, int w, int h);

        public abstract void Update();
    }
}
