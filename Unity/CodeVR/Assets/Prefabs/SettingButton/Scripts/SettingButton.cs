using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingButton : MonoBehaviour
{
    [SerializeField] private CodeBlock _settingBlock;

    [SerializeField] private Button _button;

    // Start is called before the first frame update
    void Start()
    {
        this._settingBlock.transform.parent = null;
        this._button.onClick.AddListener(() => this.ToggleShowSetting(!this._settingBlock.gameObject.activeSelf));
        this._settingBlock.gameObject.SetActive(false);
    }

    private void ToggleShowSetting(bool show)
    {
        this._settingBlock.gameObject.SetActive(show);
        var children = this._settingBlock.GetBlockCluster();
        foreach (var child in children)
        {
            child.gameObject.SetActive(show);
        }

        if (show)
            this.RealignSettingBlock();
    }

    private void RealignSettingBlock()
    {
        this._settingBlock.transform.position = this.transform.position - this.transform.forward * 0.15f - this.transform.right * 0.1f;
        this._settingBlock.transform.rotation = this.transform.rotation * Quaternion.Euler(0.0f, -10.0f, 0.0f);
        this._settingBlock.RealignBlockCluster();
    }
}
