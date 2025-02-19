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


        // Her düþman tipi için havuz oluþtur
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
        // Rastgele bir düþman tipi seç
        EnemyCustomPrefab randomEnemyType = enemyCustomPrefab[Random.Range(0, enemyCustomPrefab.Count)];
        GameObject enemyObject = GetPooledEnemy(randomEnemyType.name);

        if (enemyObject == null)
        {
            // Eðer havuzda uygun düþman yoksa yeni bir düþman oluþtur
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
        // Düþmaný rastgele bir konuma yerleþtir (örnek bir spawn mantýðý)
        float randomX = Random.Range(-10f, 10f);
        float randomY = Random.Range(-10f, 10f);
        return new Vector3(enemySpawnArea.position.x + randomX, enemySpawnArea.position.y + randomY, 0);
        //alan ici olarak düzenlencek
    }

    public void DespawnEnemy(GameObject enemyObject)
    {
        enemyObject.SetActive(false); // Düþmaný havuza geri gönder
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(enemySpawnArea.position, new Vector3(enemySpawnArea.position.x+10, enemySpawnArea.position.y+10, enemySpawnArea.position.z+10));
        //alan ici olarak düzenlencek
    }
}
