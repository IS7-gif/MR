namespace Project.Scripts.Shared.Grid
{
    public readonly struct HintCandidate
    {
        public GridPoint FirstTile { get; }
        public GridPoint SecondTile { get; }
        public bool IsValid { get; }


        public static HintCandidate None => default;


        public HintCandidate(GridPoint firstTile, GridPoint secondTile)
        {
            FirstTile = firstTile;
            SecondTile = secondTile;
            IsValid = true;
        }
    }
}