using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class Player : NetworkBehaviour
{
    private NetworkVariable<Color> playerColor = new NetworkVariable<Color>();

    private NetworkVariable<FixedString32Bytes> playername = new NetworkVariable<FixedString32Bytes>();

    public NetworkVariable<int> health = new NetworkVariable<int>(100);

    public NetworkVariable<Color> lightBulletColor = new NetworkVariable<Color>(
    Color.yellow,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Server);

    public NetworkVariable<Color> heavyBulletColor = new NetworkVariable<Color>(
        Color.magenta,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    [SerializeField] private GameObject heavyBulletPrefab;

    private Rigidbody rb;

    [SerializeField] private float speed;
    [SerializeField] private TextMesh playernametext;
    [SerializeField] private GameObject bulletprefab;
    private Vector3 shootDirection;
    public enum BulletType { Light, Heavy }

    private BulletType selectedBulletType = BulletType.Light;

    private void Awake()
    {
        shootDirection = transform.forward;
        rb = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        ApplyColor(playerColor.Value);
        playernametext.text = playername.Value.ToString();

        playername.OnValueChanged += (_, newName) => playernametext.text = newName.Value.ToString();
        playerColor.OnValueChanged += (_, newColor) => ApplyColor(newColor);
    }

    private void ApplyColor(Color color)
    {
        var rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = color;
        }
    }

    public void SetName(string name)
    {
        if (IsOwner) SendNametoServerRpc(name);
    }

    public void SetColor(Color color)
    {
        if (IsOwner) SendColorToServerRpc(color);
    }

    public void SetBulletColors(Color light, Color heavy)
    {
        if (IsOwner) SetBulletColorsServerRpc(light, heavy);
    }

    [Rpc(SendTo.Server)]
    private void SetBulletColorsServerRpc(Color light, Color heavy)
    {
        
        lightBulletColor.Value = light;
        heavyBulletColor.Value = heavy;
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
        playerColor.Value = (Color) color;
    }

    public void TakeDamage(int amount)
    {
        if (IsServer)
        {
            health.Value = Mathf.Max(0, health.Value - amount);
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector2 direccion = new Vector2(h, v).normalized;
        rb.linearVelocity = new Vector3(direccion.x, 0, direccion.y) * speed + Vector3.up * rb.linearVelocity.y;

        if (h != 0 || v != 0)
        {
            shootDirection = new Vector3(v, 0, h);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) selectedBulletType = BulletType.Light;
        if (Input.GetKeyDown(KeyCode.Alpha2)) selectedBulletType = BulletType.Heavy;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShootRpc(shootDirection, selectedBulletType);
        }
    }

    [Rpc(SendTo.Server)]
    private void ShootRpc(Vector3 direction, BulletType type)
    {
        GameObject prefab = type == BulletType.Light ? bulletprefab : heavyBulletPrefab;
        int damage = type == BulletType.Light ? 10 : 25;
        Color color = type == BulletType.Light ? lightBulletColor.Value : heavyBulletColor.Value;

        GameObject obj = Instantiate(prefab, transform.position, Quaternion.identity);
        var bullet = obj.GetComponent<Bullet>();

        bullet.InitLocal(direction, OwnerClientId, damage, color); 

        obj.GetComponent<NetworkObject>().Spawn(); 

        bullet.ApplyColorToClients(color); 
    }
}
