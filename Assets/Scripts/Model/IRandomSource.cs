namespace Game2048.Model
{
    public interface IRandomSource
    {
        int Next(int minInclusive, int maxExclusive);
    }
}
