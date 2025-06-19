using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class Player : NetworkBehaviour
{
    private NetworkVariable<Color32> playerColor = new NetworkVariable<Color32>();

    private NetworkVariable<FixedString32Bytes> playername = new NetworkVariable<FixedString32Bytes>();

    private Rigidbody rb;

    [SerializeField] private float speed;
    [SerializeField] private TextMesh playernametext;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public override void OnNetworkSpawn()
    {
        playernametext.text = playername.Value.ToString();
        playername.OnValueChanged += (oldName, newName) =>
        {
            playernametext.text = newName.Value.ToString();
        };

        playerColor.OnValueChanged += (oldColor, newColor) =>
        {
            ApplyColor(newColor);
        };
    }
    public void SetName(string name)
    {
        if (IsOwner)
        {
            SendNametoServerRpc(name);
        }
    }

    public void SetColor(Color color)
    {
        if (IsOwner)
        {
            SendColorToServerRpc(color);
        }
    }

    [Rpc(SendTo.Server)]
    private void SendNametoServerRpc(string name)
    {
        playername.Value = name;
        SendNametoClientRpc(name);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SendNametoClientRpc(string name)
    {
        playernametext.text = name;
    }

    [Rpc(SendTo.Server)]
    private void SendColorToServerRpc(Color color)
    {
        playerColor.Value = (Color32)color;
        
    }
    


    private void ApplyColor(Color32 color)
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = color;
        }
    }


    void Update()
    {

        if(IsOwner)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            Vector2 direccion = new Vector2(h, v);
            direccion.Normalize();
            rb.linearVelocity = new Vector3(direccion.x, 0, direccion.y) * speed + Vector3.up * rb.linearVelocity.y;

        }
    }
}
