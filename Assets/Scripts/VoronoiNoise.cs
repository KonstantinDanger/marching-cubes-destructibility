using UnityEngine;

[CreateAssetMenu(menuName = "Noise/Voronoi", fileName = "Noise_Voronoi")]
public class VoronoiNoise : Noise
{
    public override float Evaluate(float x, float y)
    {
        int intX = Mathf.FloorToInt(x);
        int intY = Mathf.FloorToInt(y);

        float minDist = 1.0f;

        for (int yDir = -1; yDir <= 1; yDir++)
        {
            for (int xDir = -1; xDir <= 1; xDir++)
            {
                int cellX = intX + xDir;
                int cellY = intY + yDir;

                Vector2 point = new Vector2(
                    cellX + GetRandom(cellX, cellY, 0),
                    cellY + GetRandom(cellX, cellY, 1)
                );

                Vector2 currentPos = new Vector2(x, y);
                float dist = Vector2.Distance(currentPos, point);

                if (dist < minDist)
                {
                    minDist = dist;
                }
            }
        }

        return Mathf.Clamp01(minDist);
    }

    private static float GetRandom(int x, int y, int seed)
    {
        float sine = Mathf.Sin(x * 12.9898f + y * 78.233f + seed * 43.123f);
        return Mathf.Abs((sine * 43758.5453f) % 1.0f);
    }
}
