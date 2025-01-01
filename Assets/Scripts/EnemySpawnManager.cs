using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
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
                enemyObject.SetActive(false); // �lk ba�ta g�r�nmez yap
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
            enemyPools[randomEnemyType.name].Add(enemyObject);
        }

        enemyObject.SetActive(true); // G�r�n�r yap
        enemyObject.transform.position = GetSpawnPosition(); // Rastgele bir konum ayarla
        EnemyController enemyComponent = enemyObject.GetComponent<EnemyController>();
        enemyComponent.enemyStateCurrent = EnemyController.EnemyState.Walk;
        enemyComponent.player = playerTransform;
        enemyComponent.EnemySpawned();
    }

    private GameObject GetPooledEnemy(string enemyTypeName)
    {
        if (enemyPools.ContainsKey(enemyTypeName))
        {
            foreach (GameObject enemy in enemyPools[enemyTypeName])
            {
                if (!enemy.activeInHierarchy&&enemy.GetComponent<EnemyController>().isUsable)
                {
                    return enemy;
                }
            }
        }
        return null;
    }

    private Vector3 GetSpawnPosition()
    {
        // D��man� rastgele bir konuma yerle�tir (�rnek bir spawn mant���)
        float randomX = Random.Range(-10f, 10f);
        float randomY = Random.Range(-10f, 10f);
        return new Vector3(playerTransform.position.x + randomX, playerTransform.position.y + randomY, 0);
    }

    public void DespawnEnemy(GameObject enemyObject)
    {
        enemyObject.SetActive(false); // D��man� havuza geri g�nder
    }

}
