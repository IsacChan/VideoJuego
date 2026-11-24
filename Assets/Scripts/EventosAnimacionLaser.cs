
using UnityEngine;

public class EventosAnimacionLaser : MonoBehaviour
{
    public LaserGiratorio laser;

    public void ActivarDanio()
    {
        if (laser != null)
            laser.ActivarDanio();
    }

    public void DesactivarDanio()
    {
        if (laser != null)
            laser.DesactivarDanio();
    }
}

