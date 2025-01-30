using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointManager : MonoBehaviour
{
    private List<Transform> _wayPoints = new List<Transform>();
    [SerializeField] private Transform _trackMesh;
    [SerializeField] float _interval = 25;
    [SerializeField] private LayerMask _groundLayer;
    private const float RAYCAST_HEIGHT = 10;
    public void GenerateWayPoints()
    {
        if (_trackMesh == null)
        {
            Debug.LogError($"ÉRÅ[ÉXMeshÇ™ê›íËÇ≥ÇÍÇƒÇ¢Ç‹ÇπÇÒ");
            return;
        }
        _wayPoints.Clear();
        Mesh mesh = _trackMesh.GetComponent<MeshFilter>().sharedMesh;
        if (mesh == null)
        {
            Debug.LogError($"MeshÇ™å©Ç¬Ç©ÇËÇ‹ÇπÇÒ");
            return;
        }
        Vector3[] vertices = mesh.vertices;
        int verticesCount = vertices.Length;
        for (int i = 0; i < verticesCount; i += Mathf.FloorToInt(_interval))
        {
            Vector3 worldPos = _trackMesh.TransformPoint(vertices[i]);
            AdjustHeight(worldPos);
            GameObject newPoint = new GameObject($"WayPoint : {_wayPoints.Count}");
            newPoint.transform.position = worldPos;
            newPoint.transform.parent = this.transform;
            _wayPoints.Add(newPoint.transform);
        }
    }
    public List<Transform> GetWayPoint()
    {
        GenerateWayPoints();
        return _wayPoints;
    }
    private Vector3 AdjustHeight(Vector3 position)
    {
        RaycastHit hit;
        if (Physics.Raycast(position + Vector3.up * RAYCAST_HEIGHT, Vector3.down, out hit, RAYCAST_HEIGHT * 2, _groundLayer))
        {
            position.y = hit.point.y;
        }
        return position;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < _wayPoints.Count; i++)
        {
            Gizmos.DrawSphere(_wayPoints[i].position, 0.5f);
            if (i < _wayPoints.Count - 1)
            {
                Gizmos.DrawLine(_wayPoints[i].position, _wayPoints[i + 1].position);
            }
        }
        if (_wayPoints.Count > 1)
        {
            Gizmos.DrawLine(_wayPoints[_wayPoints.Count - 1].position, _wayPoints[0].position);
        }
    }
}
