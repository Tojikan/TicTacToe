using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static bool inputEnabled = false;                    //static bool to set if the player input is enabled

    void Update()
    {
        if (inputEnabled)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 100f);
                if (hit)
                {
                    if (hit.transform.tag == "EmptyTile")
                    {
                        Vector2Int value = hit.transform.GetComponent<EmptyTile>().TileValue;
                        hit.transform.gameObject.SetActive(false);
                        BoardState.AddPosition(value, 1);
                    }
                }
            }
        }
    }
}
