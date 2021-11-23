using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class FPSCamera : SimulationBehaviour, IBeforeUpdate
{
    public PlayerMovement target;
    public Vector3 offset;
    public bool lockCursor;
    private Angle yawDelta;
    private Angle pitchDelta;

    //public void Start()
    //{
    //    if(lockCursor)
    //    {
    //        Cursor.visible = false;
    //        Cursor.lockState = CursorLockMode.Confined;
    //    }
    //}

    public override void Render()
    {
        if (target == null)
        {
            return;
        }

        float pitch = (float)(target.pitch + pitchDelta);
        float yaw = (float)(target.yaw + pitchDelta);

        transform.position = target.transform.position;
        transform.rotation = Quaternion.Euler(pitch, yaw, 0);

        target.transform.rotation = Quaternion.Euler(0, yaw, 0);
    }

    public void BeforeUpdate()
    {
        AccumulateYawPitch();
    }

    public (Angle, Angle) ConsumeYawPitch()
    {
        var yp = (yawDelta, pitchDelta);
        yawDelta = 0;
        pitchDelta = 0;
        return yp;
    }

    public void SetTarget(PlayerMovement target)
    {
        this.target = target;

        if (lockCursor)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    private void AccumulateYawPitch()
    {
        yawDelta += Input.GetAxisRaw("Mouse X");
        pitchDelta -= Input.GetAxisRaw("Mouse Y");
    }
}
