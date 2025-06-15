namespace ECC.GUI.Models
{
    public struct DelayItem
    {
        public int Value { get; set; }

        public DelayItem()
        {
            Value = 0;
        }

        public DelayItem(int value)
        {
            Value = value;
        }

        public override readonly string ToString()
        {
            return $"{Value} ms";
        }
    }
}
