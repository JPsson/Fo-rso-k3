namespace ShutTheBoxAdvanced2
{
    public interface IDiceAnimator<in T> where T : IDisplayable
    {
        void Animate(T dice1, T dice2, T ground, T space);
    }
}