using System;
using UnityEngine;

public class Parabola : MonoBehaviour
{
    [Header("Parabola")] 
    public Transform StartPoint;
    public Transform EndPoint;
    public Vector3 Size;
    [Range(0, 1)] public float DistanceFactor;

    public void SetStartPosition(Vector3 position) => StartPoint.position = position;
    public void SetEndPosition(Vector3 position) => EndPoint.position = position;

    public Vector3 Evaluate(float time) => Evaluate(StartPoint.position, EndPoint.position, Size, DistanceFactor, time);
    public Vector3 EvaluateBezier(float time) => EvaluateBezier(StartPoint.position, EndPoint.position, Size, DistanceFactor, time);

    private static Vector3 GetSizePoint(Vector3 size, float factor)
    {
        if(size.y > 0f)
        {
            size.y -= factor;
            if(size.y < 0f) size.y = 0f;
        }
        else
        {
            size.y += factor;
            if(size.y > 0f) size.y = 0f;
        }
        return size;
    }
    public static Vector3 EvaluateBezier(Vector3 start, Vector3 end, Vector3 size, float distanceFactor, float time)
    {
        if(time > 1f) time = 1f;
        else if(time < 0f) time = 0f;
        if(distanceFactor > 1) distanceFactor = distanceFactor = 1f;
        else if(distanceFactor < 0) distanceFactor = 0f;
        var factorY = Vector3.Distance(start, end) * distanceFactor / 2f;
        var b = start + (end - start) * 0.5f + GetSizePoint(size, factorY);
        var ab = start + (b - start) * time;
        var bc = b + (end - b) * time;
        return ab + (bc - ab) * time;
    }
    public static Vector3 Evaluate(Vector3 start, Vector3 end, Vector3 size, float distanceFactor, float time)
    {
        if(time > 1f) time = 1f;
        else if(time < 0f) time = 0f;
        if(distanceFactor > 1) distanceFactor = distanceFactor = 1f;
        else if(distanceFactor < 0) distanceFactor = 0f;
        var factorY = Vector3.Distance(start, end) * distanceFactor / 2f;
        var parabolaSin = Mathf.Sin(time * Mathf.PI);
        var directionPosition = (end - start) * time;
        start.x += directionPosition.x + parabolaSin * size.x;
        start.y += directionPosition.y + parabolaSin * GetSizePoint(size, factorY).y;
        start.z += directionPosition.z + parabolaSin * size.z;
        return start;
    }

#if UNITY_EDITOR
    [Header("Gizmos")] 
    [SerializeField] private Color StartColor = new Color(0f, 1f, 0f, 0f);
    [SerializeField] private Color EndColor = Color.red;
    [SerializeField, Min(0)] private float SizePoints = 5f;
    
    [Space]
    [SerializeField] private bool Bezier;
    [SerializeField, Min(2)] private int Resolution = 16;

    private void Draw()
    {
        var points = new Vector3[Resolution];
        var start = StartPoint.position;
        var end = EndPoint.position;
        for (var i = 0; i < Resolution; i++)
        {
            var time = (float)i / Resolution;
            if(Bezier) points[i] = EvaluateBezier(start, end, Size, DistanceFactor, time);
            else points[i] = Evaluate(start, end, Size, DistanceFactor, time);
            if(i == 0) continue;
            Gizmos.color = Color.Lerp(StartColor, EndColor, time);
            Gizmos.DrawLine(points[i - 1], points[i]);
        }
        Gizmos.color = EndColor;
        Gizmos.DrawLine(points[Resolution - 1], end);
    }
    private void OnDrawGizmos()
    {
        if(!StartPoint || !EndPoint) return;
        var end = EndPoint.position;
        var tempColor = Gizmos.color;
        Draw();

        var cubeColor = EndColor;
        cubeColor.a = 1;
        Gizmos.color = cubeColor;        
        Gizmos.DrawLine(end + Vector3.back * SizePoints, end + Vector3.forward * SizePoints);
        Gizmos.DrawLine(end + Vector3.left * SizePoints, end + Vector3.right * SizePoints);
        Gizmos.DrawLine(end + Vector3.down * SizePoints, end + Vector3.up * SizePoints);
        Gizmos.DrawWireCube(end, Vector3.one * SizePoints);
        Gizmos.color = tempColor;
    } 
#endif
}