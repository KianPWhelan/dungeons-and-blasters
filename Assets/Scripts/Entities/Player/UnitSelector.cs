using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.OfTomorrowInc.DMShooter;
using Fusion;

public class UnitSelector : MonoBehaviour
{
    public List<EnemyGeneric> selectedUnits = new List<EnemyGeneric>();

    public Camera cam;

    public bool demoMode;

    public GameObject holdingShift;

    private Texture2D _whiteTexture;

    private bool isDragging;

    private Vector3 mousePos;

    [HideInInspector]
    public bool render;

    private bool isUp;
    private bool isDown;
    private PlayerInput input;

    public Texture2D WhiteTexture
    {
        get
        {
            if(_whiteTexture == null)
            {
                _whiteTexture = new Texture2D(1, 1);
                _whiteTexture.SetPixel(0, 0, Color.white);
                _whiteTexture.Apply();
            }

            return _whiteTexture;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //HandleInputs();
    }

    private void OnGUI()
    {
        if(isDragging && render)
        {
            Rect rect = GetScreenRect(mousePos, input.mousePosition);
            DrawScreenRect(rect, new Color(0f, 0f, 0f, 0.25f));
            DrawScreenRectBorder(rect, 3, Color.blue);
        }
    }

    public void HandleInputs(PlayerInput input)
    {
        this.input = input;

        if(input.IsDown(PlayerInput.BUTTON_SELECT))
        {
            isUp = false;
            mousePos = input.mousePosition;
            isDragging = true;

            if(!input.IsDown(PlayerInput.BUTTON_MULTI))
            {
                RemoveSelections();
            }
            
            TrySelect();
        }

        if(!input.IsDown(PlayerInput.BUTTON_SELECT) && !isUp)
        {
            isUp = true;

            foreach(GameObject enemy in EnemyManager.enemies)
            {
                if(enemy != null && IsWithinSelectionBounds(enemy.transform))
                {
                    EnemyGeneric e = enemy.GetComponent<EnemyGeneric>();
                    
                    if(e.selectionHighlight != null)
                    {
                        e.selectionHighlight.SetActive(true);
                        selectedUnits.Add(e);
                    }
                }
            }

            isDragging = false;
        }

        //if(demoMode && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        //{
        //    holdingShift.SetActive(true);
        //}

        //else if(demoMode)
        //{
        //    holdingShift.SetActive(false);
        //}

        if(input.IsDown(PlayerInput.BUTTON_SET_DESTINATION) && !isDown)
        {
            isDown = true;
            Vector3 dest = GetMousePoint();
            GameObject follow = TryFollow();
            bool queue = false;

            if (input.IsDown(PlayerInput.BUTTON_MULTI))
            {
                queue = true;
            }

            if(follow != null)
            {
                foreach(EnemyGeneric e in selectedUnits)
                {
                    e.ClearQueue();
                    e.ClearPath();
                    e.CancelFollow();
                    e.FollowEntity(follow);
                    e.canAggro = false;
                }
            }

            else if (dest.x != Mathf.NegativeInfinity)
            {
                foreach (EnemyGeneric e in selectedUnits)
                {
                    if(e == null)
                    {
                        return;
                    }

                    if(queue)
                    {
                        e.AddToQueue(dest);
                        e.canAggro = false;
                    }

                    else
                    {
                        e.ClearQueue();
                        e.ClearPath();
                        e.CancelFollow();
                        e.AddToQueue(dest);
                        e.canAggro = false;
                    }
                }
            }
        }

        else if(input.IsUp(PlayerInput.BUTTON_SET_DESTINATION))
        {
            isDown = false;
        }
    }    

    private Vector3 GetMousePoint()
    {
        Ray ray = cam.ScreenPointToRay(input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray.origin, ray.direction, Mathf.Infinity);
        System.Array.Sort(hits, delegate (RaycastHit hit1, RaycastHit hit2) { return hit1.distance.CompareTo(hit2.distance); });

        foreach (RaycastHit hit in hits)
        {
            if(hit.transform.tag == "Ground")
            {
                return hit.point;
            }
        }

        return Vector3.negativeInfinity;
    }

    private GameObject TryFollow()
    {
        Ray ray = cam.ScreenPointToRay(input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if(hit.transform.gameObject.tag == "Enemy" || hit.transform.gameObject.tag == "Player")
            {
                return hit.transform.gameObject;
            }
        }

        return null;
    }

    private void TrySelect()
    {
        Ray ray = cam.ScreenPointToRay(input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit))
        {
            if(hit.transform.gameObject.TryGetComponent(out EnemyGeneric e) && e.selectionHighlight != null)
            {
                e.selectionHighlight.SetActive(true);
                selectedUnits.Add(e);
            }
        }
    }

    private void RemoveSelections()
    {
        foreach(EnemyGeneric e in selectedUnits)
        {
            if(e != null)
            {
                e.selectionHighlight.SetActive(false);
            }
        }

        selectedUnits.Clear();
    }

    private void DrawScreenRect(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, WhiteTexture);
    }

    private bool IsWithinSelectionBounds(Transform tf)
    {
        if(!isDragging)
        {
            return false;
        }

        Bounds vpBounds = GetVPBounds(cam, mousePos, input.mousePosition);
        return vpBounds.Contains(cam.WorldToViewportPoint(tf.position));
    }

    private void DrawScreenRectBorder(Rect rect, float thickness, Color color)
    {
        // Top Border
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        // Bottom Border
        DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
        // Left Border
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        // Right Border
        DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
    }

    private Rect GetScreenRect(Vector3 screenPos1, Vector3 screenPos2)
    {
        screenPos1.y = Screen.height - screenPos1.y;
        screenPos2.y = Screen.height - screenPos2.y;
        Vector3 bR = Vector3.Max(screenPos1, screenPos2);
        Vector3 tL = Vector3.Min(screenPos1, screenPos2);

        return Rect.MinMaxRect(tL.x, tL.y, bR.x, bR.y);
    }

    private Bounds GetVPBounds(Camera cam, Vector3 screenPos1, Vector3 screenPos2)
    {
        Vector3 pos1 = cam.ScreenToViewportPoint(screenPos1);
        Vector3 pos2 = cam.ScreenToViewportPoint(screenPos2);

        Vector3 min = Vector3.Min(pos1, pos2);
        Vector3 max = Vector3.Max(pos1, pos2);

        min.z = cam.nearClipPlane;
        max.z = cam.farClipPlane;

        Bounds bounds = new Bounds();
        bounds.SetMinMax(min, max);

        return bounds;
    }
}
