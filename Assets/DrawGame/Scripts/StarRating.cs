using UnityEngine;

public static class StarRating
{
    public static int Calculate(int linesUsed, float timeTaken, int idealLines, float idealTime)
    {
        int stars = 1;

        bool linesGood = linesUsed <= idealLines;
        bool timeGood = timeTaken <= idealTime;

        if (linesGood && timeGood)
            stars = 3;
        else if (linesGood || timeGood)
            stars = 2;
        else
            stars = 1;

        return stars;
    }
}
