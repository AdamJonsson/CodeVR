using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

[RequireComponent(typeof(AudioSource))]
public class BlockMenu : MonoBehaviour
{
    [SerializeField] private List<MenuCategory> _menuCategories;

    private AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
        this._audioSource = this.GetComponent<AudioSource>();

        foreach (var menuCategory in this._menuCategories)
        {
            menuCategory.Button.onClick.AddListener(() => this.OnButtonClicked(menuCategory));
        }

        if (this._menuCategories.Count > 0)
            this.OnButtonClicked(this._menuCategories.First());
    }

    private void OnButtonClicked(MenuCategory menuCategoryToShow)
    {
        foreach (var menuCategory in this._menuCategories)
        {
            this.ToggleMenuCategory(false, menuCategory);
        }
        this.ToggleMenuCategory(true, menuCategoryToShow);
        this._audioSource.Play();
    }

    private void ToggleMenuCategory(bool show, MenuCategory menuCategory)
    {
        var selectedColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        var defaultColor = new Color(0.85f, 0.85f, 0.85f, 1.0f);

        menuCategory.MenuContainer.SetActive(show);
        menuCategory.Button.transition = show ? Selectable.Transition.None : Selectable.Transition.ColorTint;
        menuCategory.Button.GetComponentInChildren<TMP_Text>().fontStyle = show ? (FontStyles.Bold | FontStyles.Underline) : FontStyles.Normal;

        var colors = menuCategory.Button.colors; 
        colors.normalColor = show ? selectedColor : defaultColor;
        menuCategory.Button.colors = colors;

        menuCategory.Button.GetComponent<Image>().color = show ? selectedColor : defaultColor;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]
public struct MenuCategory
{
    public Button Button;
    public GameObject MenuContainer;
}