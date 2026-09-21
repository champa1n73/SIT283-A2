using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetUI : MonoBehaviour
{
    [SerializeField] private XROrigin xr;
    [SerializeField] private Transform spawnPoint;

    private void Start()
    {
        xr.transform.position = spawnPoint.position;
        xr.transform.rotation = spawnPoint.rotation;
        Camera.main.transform.position = spawnPoint.position;
        Camera.main.transform.rotation = spawnPoint.rotation;
    }

    public void ResetGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
