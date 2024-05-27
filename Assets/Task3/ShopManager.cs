using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] int _itemCount = 50;
    [SerializeField] int _itemPerRow = 5;
    [SerializeField] Button _itemButton;

    [SerializeField] GameObject _horizontalGroupPrefab;
    [SerializeField] RectTransform _verticalGroup;
    [SerializeField] RectTransform _viewPort;
    [SerializeField] ScrollRect _scrollRect;
    [SerializeField] Gradient _gradient;

    List<GameObject> _horizontalGroups = new List<GameObject>();

    int itemsToAdd;
    float elementHeight;
    Vector2 _oldVelocity = Vector2.zero;
    bool isUpdated = false;

    private void Awake()
    {
        float spacing = _verticalGroup.GetComponent<VerticalLayoutGroup>().spacing;
        elementHeight = _itemButton.GetComponent<RectTransform>().rect.height + spacing;

        //create main group of items
        int itemIndex = 1;
        for (int i = 0; i < _itemCount; i += _itemPerRow)
        {
            GameObject horizontalGroup = Instantiate(_horizontalGroupPrefab, _verticalGroup);
            for (int j = 0; j < _itemPerRow; j++)
            {
                if (i + j < _itemCount)
                {
                    Button item = Instantiate(_itemButton, horizontalGroup.transform);
                    string itemMessage = "Item " + itemIndex + " clicked";
                    item.onClick.AddListener(() => Debug.Log(itemMessage));
                    item.GetComponentInChildren<TextMeshProUGUI>().text = itemIndex.ToString();
                    item.GetComponent<Image>().color = _gradient.Evaluate((float)itemIndex / _itemCount);
                    itemIndex++;
                }
            }
            _horizontalGroups.Add(horizontalGroup);
        }        

        //add copy items to the end to make infinite scrolling
        itemsToAdd = Mathf.CeilToInt(_viewPort.rect.height / elementHeight);
        for (int i = 0; i < itemsToAdd; i++)
        {
            GameObject horizontalGroup = Instantiate(_horizontalGroups[i % _horizontalGroups.Count], _verticalGroup);
            horizontalGroup.transform.SetAsLastSibling();
            for (int j = 0; j < _itemPerRow; j++)
            {
                if (i * _itemPerRow + j < _itemCount)
                {
                    GameObject item = horizontalGroup.transform.GetChild(j).gameObject;
                    string itemMessage = "Item " + item.GetComponentInChildren<TextMeshProUGUI>().text + " clicked";
                    item.GetComponent<Button>().onClick.AddListener(() => Debug.Log(itemMessage));
                }
            }
        }

        //add copy items to the beginning to make infinite scrolling
        for (int i = 0; i < itemsToAdd; i++)
        {
            int index = _horizontalGroups.Count - 1 - i;
            GameObject horizontalGroup = Instantiate(_horizontalGroups[index], _verticalGroup);
            horizontalGroup.transform.SetAsFirstSibling();
            for (int j = 0; j < _itemPerRow; j++)
            {
                if (index * _itemPerRow + j < _itemCount)
                {
                    GameObject item = horizontalGroup.transform.GetChild(j).gameObject;
                    string itemMessage = "Item " + item.GetComponentInChildren<TextMeshProUGUI>().text + " clicked";
                    item.GetComponent<Button>().onClick.AddListener(() => Debug.Log(itemMessage));
                }
            }
        }

        //calculate the size of the vertical group
        _verticalGroup.sizeDelta = new Vector2(_verticalGroup.sizeDelta.x, (_itemCount / _itemPerRow + 2 * itemsToAdd) * elementHeight);

        //set the position of the vertical group
        _verticalGroup.localPosition = new Vector3(_verticalGroup.localPosition.x, elementHeight * itemsToAdd, _verticalGroup.localPosition.z);
    }

    private void Update()
    {
        if (isUpdated)
        {
            _scrollRect.velocity = _oldVelocity;
            isUpdated = false;
        }

        if (_verticalGroup.localPosition.y < elementHeight * itemsToAdd / 2)
        {
            Canvas.ForceUpdateCanvases();
            _oldVelocity = _scrollRect.velocity;
            _verticalGroup.localPosition += new Vector3(0, elementHeight * _itemCount / _itemPerRow, 0);
            isUpdated = true;
        }

        if (_verticalGroup.localPosition.y > elementHeight * (itemsToAdd / 2 + _itemCount / _itemPerRow))
        {
            Canvas.ForceUpdateCanvases();
            _oldVelocity = _scrollRect.velocity;
            _verticalGroup.localPosition -= new Vector3(0, elementHeight * _itemCount / _itemPerRow, 0);
            isUpdated = true;
        }
    }
}
