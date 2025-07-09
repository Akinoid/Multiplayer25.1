using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI myHealthText;
    [SerializeField] private TextMeshProUGUI enemyHealthText;

    void Update()
    {
        var players = FindObjectsOfType<Player>();

        foreach (var p in players)
        {
            if (p.IsOwner)
                myHealthText.text = "Tu Vida: " + p.health.Value;
            else
                enemyHealthText.text = "Enemigo: " + p.health.Value;
        }
    }
}
