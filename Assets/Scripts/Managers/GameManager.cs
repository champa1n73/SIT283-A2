using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] List<GameObject> objectsToDisableOnStart;
    private void Awake()
    {
        foreach (GameObject gameObj in objectsToDisableOnStart)
        {
            gameObj.SetActive(false);
        }
    }

    public void StartGame()
    {
        foreach (GameObject gameObj in objectsToDisableOnStart)
        {
            gameObj.SetActive(true);
        }
    }

}
