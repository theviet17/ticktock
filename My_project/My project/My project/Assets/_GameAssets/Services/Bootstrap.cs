
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Bootstrap
{

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void GenerateServices()=> Object.DontDestroyOnLoad(Object.Instantiate(Resources.Load("Services")));


    public static void LoadSceneWhenStart()
    {
        SceneManager.LoadScene(0);
    }

}
