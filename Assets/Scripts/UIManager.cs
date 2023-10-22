using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake() 
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void Start()
    {
        //DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadFirstLevel() 
    {
        SceneManager.LoadScene(1);
    }

    public void LoadScendLevel() 
    {
        SceneManager.LoadScene(2);
    }

    public void LoadStartScene()
    {
        SceneManager.LoadScene(0);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1)
        {
            GameObject exitButton = GameObject.FindGameObjectWithTag("ExitButton");
            if (exitButton != null)
            {
                Button quitButtonComponent = exitButton.GetComponent<Button>();
                if (quitButtonComponent != null)
                {
                    quitButtonComponent.onClick.AddListener(LoadStartScene);
                }
            }
        }
    }
}
