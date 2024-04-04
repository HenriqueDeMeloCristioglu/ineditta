namespace Ineditta.BuildingBlocks.Core.Parallels
{
    public class AtomicCounter
    {
        public int Value { get; set; }
        public AtomicCounter(int initialValue)
        {
            Value = initialValue;
        }
    }
}
