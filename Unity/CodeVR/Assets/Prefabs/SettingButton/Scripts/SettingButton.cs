using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingButton : MonoBehaviour
{
    [SerializeField] private GameObject _settingObjects;

    [SerializeField] private Button _button;

    // Start is called before the first frame update
    void Start()
    {
        this._settingObjects.transform.parent = null;
        this._button.onClick.AddListener(this.ToggleShowSetting);
        this.ToggleShowSetting();
    }

    private void ToggleShowSetting()
    {
        this._settingObjects.SetActive(!this._settingObjects.activeSelf);
        this._settingObjects.transform.position = this.transform.position - this.transform.forward * 0.15f - this.transform.right * 0.1f;
        this._settingObjects.transform.rotation = this.transform.rotation * Quaternion.Euler(0.0f, -10.0f, 0.0f);
    }
}
