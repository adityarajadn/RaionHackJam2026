using UnityEngine;
using UnityEngine.Serialization;
public class OpenableObject : Interactable
{
    [Header("Sprites")]
    [Tooltip("GameObject atau SpriteRenderer untuk state tertutup")]
    [FormerlySerializedAs("closedSprite")]
    [SerializeField] private GameObject _closedSprite;
    [Tooltip("GameObject atau SpriteRenderer untuk state terbuka")]
    [FormerlySerializedAs("openSprite")]
    [SerializeField] private GameObject _openSprite;
    [FormerlySerializedAs("itemInside")]
    [SerializeField] private GameObject _itemInside;
    private bool _isOpen = false;
    protected override void Awake()
    {
        base.Awake();
        UpdateSpriteState();
        HideItemInside();
    }
    public override void Interact()
    {
        _isOpen = !_isOpen;
        UpdateSpriteState();
        ShowItemInside();
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("door");
        }
    }
    private void UpdateSpriteState()
    {
        if (_closedSprite != null)
        {
            _closedSprite.SetActive(!_isOpen);
        }
        if (_openSprite != null)
        {
            _openSprite.SetActive(_isOpen);
        }
    }
    public void HideItemInside() 
    {
        if (_itemInside != null) 
        {
            _itemInside.SetActive(false);
        }
    }
    public void ShowItemInside() 
    {
        if (_itemInside != null) 
        {
            _itemInside.SetActive(true);
        }
    }
}
