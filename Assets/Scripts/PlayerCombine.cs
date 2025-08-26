using UnityEngine;
using System.Collections.Generic;

public class PlayerCombine : MonoBehaviour
{
    private List<Transform> combinedObjects = new List<Transform>();
    private PlayerInputActions actions;
    private float separateCooldown = 0.5f; // くっつき禁止時間（秒）
    private float lastSeparateTime = -1f;

    private void Awake()
    {
        actions = new PlayerInputActions();
        actions.Player.Enable();

        // プレイヤー自身のRigidbodyの回転を固定（Rigidbodyがある場合のみ）
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Time.time - lastSeparateTime < separateCooldown) return;

        if (collision.gameObject.CompareTag("Objects"))
        {
            collision.transform.SetParent(transform);
            combinedObjects.Add(collision.transform);
        }
    }

    void Update()
    {
        // Shiftキー（Separateアクション）が押されたら切り離し
        if (actions.Player.Separate.WasPressedThisFrame())
        {
            if (combinedObjects.Count > 0)
            {
                foreach (var obj in combinedObjects)
                {
                    obj.SetParent(null);
                }
                combinedObjects.Clear();
                lastSeparateTime = Time.time;
            }
        }
    }
}
