using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadMainScene : MonoBehaviour
{
    [SerializeField] private Button _button;

    // Start is called before the first frame update
    void Start()
    {
        this._button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        SceneManager.LoadScene("MainScene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
