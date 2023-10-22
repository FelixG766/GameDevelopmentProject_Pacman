using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
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
}
