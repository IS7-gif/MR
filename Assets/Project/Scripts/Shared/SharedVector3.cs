namespace Project.Scripts.Shared
{
    public readonly struct SharedVector3
    {
        public float X { get; }
        public float Y { get; }
        public float Z { get; }


        public SharedVector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }


        public override string ToString()
        {
            return $"({X:F2}, {Y:F2}, {Z:F2})";
        }
    }
}