using UnityEngine;
using Cinemachine;

public class SwitchBoundsConfiner : MonoBehaviour
{

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += SwitchBoundsShape;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= SwitchBoundsShape;
    }

    /// <summary>
    /// Switch the collider that cinemachine uses to define edges of the screen
    /// </summary>
    private void SwitchBoundsShape()
    {
        var polygonCollider = GameObject.FindGameObjectWithTag(Tags.BoundsConfiner).GetComponent<PolygonCollider2D>();

        var cinemachineConfiner = GetComponent<CinemachineConfiner>();
        cinemachineConfiner.m_BoundingShape2D = polygonCollider;
        cinemachineConfiner.InvalidatePathCache();
    }
}
