using UnityEngine;
using Unity.Netcode;

public class Bullet : NetworkBehaviour
{
    private Vector3 direction;
    private Rigidbody rb;
    [SerializeField] private float speed;
    private ulong ownerId;

    private int damage;
    [SerializeField] private MeshRenderer meshRenderer;

    public void InitLocal(Vector3 direction, ulong ownerId, int damage, Color color)
    {
        
        this.direction = direction;
        this.ownerId = ownerId;
        this.damage = damage;

        ApplyColor(color); 
    }


    public void ApplyColorToClients(Color color)
    {
        ApplyColorClientRpc(color); 
    }

    [Rpc(SendTo.Everyone)]
    private void ApplyColorClientRpc(Color color)
    {
        ApplyColor(color);
    }

    private void ApplyColor(Color color)
    {
        
        if (meshRenderer != null)
        {
            meshRenderer.material = new Material(meshRenderer.material);
            meshRenderer.material.color = color;
            Debug.Log($" Material instanciado y color aplicado: {color}");
        }
        else
        {
            Debug.LogWarning("[BULLET] MeshRenderer is null!");
        }
    }

    private void Start()
    {
        if (IsServer)
        {
            rb = GetComponent<Rigidbody>();
        }
    }

    private void Update()
    {
        if (IsServer)
        {
            rb.linearVelocity = direction * speed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsServer && other.CompareTag("Player") && other.GetComponent<Player>().OwnerClientId != ownerId)
        {
            other.GetComponent<Player>().TakeDamage(damage);
            NetworkObject.Despawn(gameObject);
        }
    }
}
