using UnityEngine;
using System.Collections.Generic;

public class PlayerCombine : MonoBehaviour
{
    private PlayerMove playerMove;
    private List<Transform> combinedObjects = new List<Transform>();
    private List<Material> originalMaterials = new List<Material>();
    private PlayerInputActions actions;
    private float separateCooldown = 0.5f;
    private float lastSeparateTime = -1f;

    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material combineMaterial;

    private Renderer playerRenderer;

    private void Awake()
    {
        playerRenderer = GetComponentInChildren<Renderer>();
        playerMove = GetComponent<PlayerMove>();

        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        actions = new PlayerInputActions();
        actions.Player.Enable();
    }

    private void Update()
    {
        // プレイヤーの状態に応じて色を変更
        if (playerRenderer != null)
        {
            if (playerMove.currentState == PlayerState.Combine)
            {
                playerRenderer.material = combineMaterial;
            }
            else
            {
                playerRenderer.material = normalMaterial;
            }
        }

        if (actions.Player.Separate.WasPressedThisFrame())
        {
            if (combinedObjects.Count > 0)
            {
                for (int i = 0; i < combinedObjects.Count; i++)
                {
                    var objRenderer = combinedObjects[i].GetComponent<Renderer>();
                    if (objRenderer != null && i < originalMaterials.Count)
                    {
                        objRenderer.material = originalMaterials[i]; // 元のマテリアルに戻す
                    }
                    combinedObjects[i].SetParent(null);
                }
                combinedObjects.Clear();
                originalMaterials.Clear();
                lastSeparateTime = Time.time;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (playerMove.currentState != PlayerState.Combine) return;
        if (Time.time - lastSeparateTime < separateCooldown) return;

        if (collision.gameObject.CompareTag("Objects"))
        {
            var renderer = collision.gameObject.GetComponent<Renderer>();
            if (renderer != null && combineMaterial != null)
            {
                originalMaterials.Add(renderer.material); // 元のマテリアルを保存
                renderer.material = combineMaterial;
            }

            collision.transform.SetParent(transform);
            combinedObjects.Add(collision.transform);
        }
    }
}
