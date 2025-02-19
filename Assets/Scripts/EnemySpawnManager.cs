using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    public Transform enemySpawnArea;
    Transform playerTransform;

    [System.Serializable]
    public class EnemyCustomPrefab
    {
        public string name;
        public GameObject prefab;
        public int initialPoolSize;
    }
    public List<EnemyCustomPrefab> enemyCustomPrefab;
    private Dictionary<string, List<GameObject>> enemyPools = new Dictionary<string, List<GameObject>>();



    public void SpawnStart()
    {
        playerTransform = GameManagerCustom.GetPlayer().transform;


        // Her d��man tipi i�in havuz olu�tur
        foreach (EnemyCustomPrefab enemys in enemyCustomPrefab)
        {
            List<GameObject> pool = new List<GameObject>();

            for (int i = 0; i < enemys.initialPoolSize; i++)
            {
                GameObject enemyObject = Instantiate(enemys.prefab);
                EnemyPublicController enemyController = enemyObject.GetComponent<EnemyPublicController>();
                enemyObject.GetComponent<IBaseEnemy>().Create(enemyController.enemyStatsScriptableObject);
                pool.Add(enemyObject);
            }

            enemyPools.Add(enemys.name, pool);
        }

        InvokeRepeating("SpawnEnemy", 0, 4);
    }


    private void SpawnEnemy()
    {
        // Rastgele bir d��man tipi se�
        EnemyCustomPrefab randomEnemyType = enemyCustomPrefab[Random.Range(0, enemyCustomPrefab.Count)];
        GameObject enemyObject = GetPooledEnemy(randomEnemyType.name);

        if (enemyObject == null)
        {
            // E�er havuzda uygun d��man yoksa yeni bir d��man olu�tur
            enemyObject = Instantiate(randomEnemyType.prefab);
            EnemyPublicController enemyController = enemyObject.GetComponent<EnemyPublicController>();
            enemyController.Create(enemyController.enemyStatsScriptableObject);
            enemyPools[randomEnemyType.name].Add(enemyObject);
        }
        enemyObject.GetComponent<IBaseEnemy>().Spawn(GetSpawnPosition());
    }

    private GameObject GetPooledEnemy(string enemyTypeName)
    {
        if (enemyPools.ContainsKey(enemyTypeName))
        {
            foreach (GameObject enemy in enemyPools[enemyTypeName])
            {
            }
        }
        return null;
    }

    private Vector3 GetSpawnPosition()
    {
        // D��man� rastgele bir konuma yerle�tir (�rnek bir spawn mant���)
        float randomX = Random.Range(-10f, 10f);
        float randomY = Random.Range(-10f, 10f);
        return new Vector3(enemySpawnArea.position.x + randomX, enemySpawnArea.position.y + randomY, 0);
        //alan ici olarak d�zenlencek
    }

    public void DespawnEnemy(GameObject enemyObject)
    {
        enemyObject.SetActive(false); // D��man� havuza geri g�nder
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(enemySpawnArea.position, new Vector3(enemySpawnArea.position.x+10, enemySpawnArea.position.y+10, enemySpawnArea.position.z+10));
        //alan ici olarak d�zenlencek
    }
}
