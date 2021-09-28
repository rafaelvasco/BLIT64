namespace BLIT64
{
    public abstract class Scene
    {
        public static Game Game { get; internal set; }
        public static Canvas Canvas { get; internal set; }

        public abstract void Load();

        public abstract void Update();

        public abstract void Draw(Canvas canvas);

    }

    internal class EmptyScene : Scene
    {
        public override void Load()
        {
        }

        public override void Update()
        {
        }

        public override void Draw(Canvas canvas)
        {
        }
    }

    public class LoaderScene : Scene
    {
        public override void Load()
        {
        }

        public override void Update()
        {
        }

        public override void Draw(Canvas canvas)
        {
        }
    }
}
