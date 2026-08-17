namespace Game2048.Model
{
    public sealed class Tile
    {
        public int Value { get; }
        public int Id { get; }

        public Tile(int value, int id)
        {
            Value = value;
            Id = id;
        }
    }
}
