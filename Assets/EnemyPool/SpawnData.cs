using UnityEngine;

[System.Serializable]
public class SpawnData
{
    public int Count;
    public bool IsRandomCount;
    public Vector2Int Range;
    public Enemies EnemiesType;

    public SpawnData(Enemies enemyType)
    {
        EnemiesType = enemyType;
    }
}
