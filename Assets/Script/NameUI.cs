using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NameUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button button;
    [SerializeField] private TMP_Dropdown colorDropdown;
    Color selectedColor = Color.red;

    void Start()
    {
        colorDropdown.onValueChanged.AddListener(OnColorSelected);
        button.onClick.AddListener(SetName);
        
    }

    private void SetName()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players)
        {
            if(player.GetComponent<Player>().IsOwner)
            {
                player.GetComponent<Player>().SetName(nameInputField.text);
                player.GetComponent<Player>().SetColor(selectedColor);
            }
        }
        
        Destroy(gameObject);
    }
    

    private void OnColorSelected(int index)
    {
        
        string selectedName = colorDropdown.options[index].text;

        switch (selectedName.ToLower())
        {
            case "red":
                selectedColor = Color.red;
                break;
            case "blue":
                selectedColor = Color.blue;
                break;
            case "black":
                selectedColor = Color.black;
                break;
        }        

    }
}
