using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyManager : MonoBehaviour
{
    public Transform[] EnemySpawn;

    public Transform[] PotionSpawn;

    public Transform[] PotalSpawn; 

    public GameObject enemy;

    public GameObject potion;

    public GameObject Portal;

    public bool[] isSpawn;

    private int EnemyCount = 0;

    private int MaxEnemycount = 5;

    private void Start()
    {
        isSpawn = new bool[EnemySpawn.Length];
        for (int i = 0; i < isSpawn.Length; i++)
        {
            isSpawn[i] = false;
        }

       while(true)
        {
            int x = Random.Range(0, EnemySpawn.Length);
            if (!isSpawn[x])
            {
                Instantiate(enemy, EnemySpawn[x]);
                EnemyCount++;
                isSpawn[x] = true;
            }
            if (EnemyCount == MaxEnemycount)
            {
                break;
            }
        }

        while (true)
        {
            int x = Random.Range(0, EnemySpawn.Length);
            if (!isSpawn[x])
            {
                Instantiate(potion, EnemySpawn[x]);
                isSpawn[x] = true;
                break;
            }
        }

        while (true)
        {
            int x = Random.Range(0, EnemySpawn.Length);
            if (!isSpawn[x])
            {
                Instantiate(Portal, EnemySpawn[x]);
                isSpawn[x] = true;
                break;
            }
        }
    }
}
