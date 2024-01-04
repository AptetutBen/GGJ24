using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    public Transform cursor;
    public SpriteRenderer cursorSprite;
    public Color playerColour;

    private Camera mainCamera;
    private NetworkManager networkManager;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetPlayer();

        if (!IsOwner)
        {
            return;
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void SetPlayer()
    {
        cursorSprite.color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        MoveCursor(GetMousePosition());
    }

    public void MoveCursor(Vector3 pos)
    {
        cursor.transform.position = pos;
    }

    private Vector3 GetMousePosition()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector3(worldPos.x, worldPos.y, 0);
    }
}
