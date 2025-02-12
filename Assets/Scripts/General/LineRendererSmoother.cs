using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineRendererSmoother : MonoBehaviour
{
    public LineRenderer Line;
    public Vector3[] InitialState = new Vector3[1];
    public int SmoothingSections = 10;
}