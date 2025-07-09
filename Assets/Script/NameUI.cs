using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NameUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button button;
    [SerializeField] private TMP_Dropdown colorDropdown;
    [SerializeField] private TMP_Dropdown lightBulletColorDropdown;
    [SerializeField] private TMP_Dropdown heavyBulletColorDropdown;

    Color selectedColor = Color.red;
    private Color selectedLightBulletColor = Color.yellow;
    private Color selectedHeavyBulletColor = Color.magenta;

    void Start()
    {
        colorDropdown.onValueChanged.AddListener(OnColorSelected);
        lightBulletColorDropdown.onValueChanged.AddListener(OnLightBulletColorSelected);
        heavyBulletColorDropdown.onValueChanged.AddListener(OnHeavyBulletColorSelected);
        button.onClick.AddListener(SetData);
        
    }

    private void SetData()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players)
        {
            if(player.GetComponent<Player>().IsOwner)
            {                
                player.GetComponent<Player>().SetName(nameInputField.text);
                player.GetComponent<Player>().SetColor(selectedColor);
                player.GetComponent<Player>().SetBulletColors(selectedLightBulletColor, selectedHeavyBulletColor);                
            }
        }
        
        Destroy(gameObject);
    }
    

    private void OnColorSelected(int index)
    {
        selectedColor = GetColorFromIndex(index);
    }
    private void OnLightBulletColorSelected(int index)
    {
        selectedLightBulletColor = GetColorFromIndex(index);
    }

    private void OnHeavyBulletColorSelected(int index)
    {
        selectedHeavyBulletColor = GetColorFromIndex(index);
    }

    private Color GetColorFromIndex(int index)
    {
        string selectedName = colorDropdown.options[index].text.ToLower();

        switch (selectedName)
        {
            case "red":
                return Color.red;

            case "blue":
                return Color.blue;

            case "yellow":
                return Color.yellow;

            case "green":
                return Color.green;

            case "black":
                return Color.black;

            case "white":
                return Color.white;

            case "magenta":
                return Color.magenta;

            default:
                return Color.gray;
        }
    }
}
