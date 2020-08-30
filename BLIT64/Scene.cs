namespace BLIT64
{
    public abstract class Scene
    {
        public Game Game { get; internal set; }
        public Blitter Blitter { get; internal set; }

        public virtual void Load() { }

        public abstract void Update();

        public abstract void Draw(Blitter blitter);

    }

    internal class EmptyScene : Scene
    {
        public override void Update()
        {
        }

        public override void Draw(Blitter blitter)
        {
        }
    }

    public class LoaderScene : Scene
    {
        public override void Update()
        {
            throw new System.NotImplementedException();
        }

        public override void Draw(Blitter blitter)
        {
            throw new System.NotImplementedException();
        }
    }
}
