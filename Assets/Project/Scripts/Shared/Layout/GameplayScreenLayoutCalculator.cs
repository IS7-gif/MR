namespace Project.Scripts.Shared.Layout
{
    public static class GameplayScreenLayoutCalculator
    {
        public static GameplayScreenLayout Calculate(
            ScreenLayoutRect screenRect,
            ScreenLayoutRect safeAreaRect,
            bool useSafeArea,
            bool worldExtendsIntoUnsafeBottomArea,
            float safeAreaPadding,
            float gameplayAspect,
            float referenceResolutionWidth,
            float referenceResolutionHeight,
            float topBarHeight,
            float topBarSidePadding,
            float topBarBottomPadding,
            float worldBottomPadding,
            float worldSidePadding)
        {
            var availableRect = useSafeArea ? safeAreaRect : screenRect;
            var pixelScale = CalculatePixelScale(screenRect.Width, screenRect.Height, referenceResolutionWidth, referenceResolutionHeight);
            var paddedAvailableRect = availableRect.Inset(
                safeAreaPadding * pixelScale,
                safeAreaPadding * pixelScale,
                safeAreaPadding * pixelScale,
                safeAreaPadding * pixelScale);
            var gameplayRect = FitAspect(paddedAvailableRect, gameplayAspect);

            var scaledTopBarHeight = topBarHeight * pixelScale;
            var scaledTopBarSidePadding = topBarSidePadding * pixelScale;
            var scaledTopBarBottomPadding = topBarBottomPadding * pixelScale;
            var scaledWorldBottomPadding = worldBottomPadding * pixelScale;
            var scaledWorldSidePadding = worldSidePadding * pixelScale;

            var topBarAreaRect = useSafeArea ? paddedAvailableRect : gameplayRect;
            var topBarRect = ScreenLayoutRect.FromMinMax(
                gameplayRect.XMin + scaledTopBarSidePadding,
                topBarAreaRect.YMax - scaledTopBarHeight,
                gameplayRect.XMax - scaledTopBarSidePadding,
                topBarAreaRect.YMax);

            var worldBottomBase = worldExtendsIntoUnsafeBottomArea
                ? Min(gameplayRect.YMin, screenRect.YMin)
                : gameplayRect.YMin;
            var worldRect = ScreenLayoutRect.FromMinMax(
                gameplayRect.XMin + scaledWorldSidePadding,
                worldBottomBase + scaledWorldBottomPadding,
                gameplayRect.XMax - scaledWorldSidePadding,
                topBarAreaRect.YMax - scaledTopBarHeight - scaledTopBarBottomPadding);

            return new GameplayScreenLayout(safeAreaRect, gameplayRect, topBarRect, worldRect, pixelScale);
        }


        private static float CalculatePixelScale(float screenWidth, float screenHeight, float referenceWidth, float referenceHeight)
        {
            if (referenceWidth <= 0f || referenceHeight <= 0f)
                return 1f;

            return screenWidth / referenceWidth;
        }


        private static ScreenLayoutRect FitAspect(ScreenLayoutRect rect, float aspect)
        {
            if (aspect <= 0f || rect.Width <= 0f || rect.Height <= 0f)
                return rect;

            var rectAspect = rect.Width / rect.Height;
            if (rectAspect > aspect)
            {
                var width = rect.Height * aspect;
                var x = rect.X + (rect.Width - width) * 0.5f;
                return new ScreenLayoutRect(x, rect.Y, width, rect.Height);
            }

            var height = rect.Width / aspect;
            var y = rect.Y + (rect.Height - height) * 0.5f;
            
            return new ScreenLayoutRect(rect.X, y, rect.Width, height);
        }

        private static float Min(float a, float b) => a < b ? a : b;
    }

    public readonly struct GameplayWorldLayout
    {
        public ScreenLayoutRect WorldRect { get; }
        public float FrameWidth { get; }
        public float FrameHeight { get; }
        public float FrameCellSize { get; }
        public float TileCellSize { get; }
        public float GapScale { get; }


        public GameplayWorldLayout(ScreenLayoutRect worldRect, float frameWidth, float frameHeight, float frameCellSize, float tileCellSize, float gapScale)
        {
            WorldRect = worldRect;
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
            FrameCellSize = frameCellSize;
            TileCellSize = tileCellSize;
            GapScale = gapScale;
        }
    }

    public static class GameplayWorldLayoutCalculator
    {
        public static GameplayWorldLayout Calculate(
            ScreenLayoutRect worldRect,
            float maxAspectRatio,
            float framePaddingPercent,
            float tilePaddingPercent,
            int gridWidth,
            int gridHeight,
            float frameExtraHeight,
            float fixedContentHeight,
            float gapCellUnits,
            float minGapScale,
            float minCellSize)
        {
            if (gridWidth <= 0 || gridHeight <= 0)
                return new GameplayWorldLayout(worldRect, 0f, 0f, minCellSize, minCellSize, 1f);

            var safeMinCellSize = Max(0f, minCellSize);
            var safeMinGapScale = Clamp01(minGapScale);
            var safeFramePadding = Clamp01(framePaddingPercent);
            var safeTilePadding = Clamp01(tilePaddingPercent);

            var frameCellSizeByWidth = worldRect.Width * (1f - safeFramePadding) / gridWidth;
            var tileCellSizeByWidth = worldRect.Width * (1f - safeTilePadding) / gridWidth;
            var tileToFrameRatio = frameCellSizeByWidth > 0f
                ? tileCellSizeByWidth / frameCellSizeByWidth
                : 1f;

            var frameCellSize = Max(safeMinCellSize, frameCellSizeByWidth);
            var tileCellSize = Max(safeMinCellSize, tileCellSizeByWidth);
            var frameHeight = gridHeight * frameCellSize + frameExtraHeight;
            var gapScale = CalculateGapScale(
                worldRect.Height,
                frameHeight,
                fixedContentHeight,
                gapCellUnits,
                frameCellSize,
                safeMinGapScale);

            if (gapScale <= safeMinGapScale && StackHeight(frameHeight, fixedContentHeight, gapCellUnits, frameCellSize, gapScale) > worldRect.Height)
            {
                var denominator = gridHeight + gapCellUnits * gapScale;
                var heightFrameCellSize = denominator > 0f
                    ? (worldRect.Height - fixedContentHeight - frameExtraHeight) / denominator
                    : safeMinCellSize;

                frameCellSize = Max(safeMinCellSize, Min(frameCellSizeByWidth, heightFrameCellSize));
                tileCellSize = Max(safeMinCellSize, frameCellSize * tileToFrameRatio);
                frameHeight = gridHeight * frameCellSize + frameExtraHeight;
            }

            var frameWidth = gridWidth * frameCellSize;

            return new GameplayWorldLayout(worldRect, frameWidth, frameHeight, frameCellSize, tileCellSize, gapScale);
        }


        private static float CalculateGapScale(float worldHeight, float frameHeight, float fixedContentHeight,
            float gapCellUnits, float tileCellSize, float minGapScale)
        {
            if (gapCellUnits <= 0f || tileCellSize <= 0f)
                return 1f;

            var desiredGapHeight = gapCellUnits * tileCellSize;
            var availableGapHeight = worldHeight - frameHeight - fixedContentHeight;
            if (availableGapHeight >= desiredGapHeight)
                return 1f;

            return Max(minGapScale, availableGapHeight / desiredGapHeight);
        }

        private static float StackHeight(float frameHeight, float fixedContentHeight, float gapCellUnits,
            float tileCellSize, float gapScale)
        {
            return frameHeight + fixedContentHeight + gapCellUnits * tileCellSize * gapScale;
        }

        private static float Clamp01(float value) => Max(0f, Min(1f, value));
        private static float Min(float a, float b) => a < b ? a : b;
        private static float Max(float a, float b) => a > b ? a : b;
    }
}