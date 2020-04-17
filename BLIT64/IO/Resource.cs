﻿namespace BLIT64
{
    public abstract class Resource
    {
        protected bool disposed = false;

        public string Id { get; internal set; }

        protected abstract void Dispose(bool disposing);

        internal void Dispose()
        {
            Dispose(true);
        }
    }
}
