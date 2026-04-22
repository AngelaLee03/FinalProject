using UnityEngine;

public class DrawMesh : MonoBehaviour
{
    private Mesh mesh;
    private bool isDragging = false;
    private Vector3 lastTouchPosition;
    private Plane drawPlane;
    public float heightOffset = 0.01f;
    public float lineThickness = 0.05f;
    private void Awake()
    {
        drawPlane = new Plane(Vector3.up, Vector3.zero);
    }
    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            //Vector3 screenPoint = touch.position;
            //screenPoint.z = transform.position.z - Camera.main.transform.position.z;
            //Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPoint);
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            float enter;
            Vector3 worldPosition = Vector3.zero;

            if (drawPlane.Raycast(ray, out enter))
            {
                worldPosition = ray.GetPoint(enter);
            }
            else
            {
                return;
            }
            worldPosition += drawPlane.normal * heightOffset;

            if (touch.phase == TouchPhase.Began)
            {
                mesh = new Mesh();
                //mesh.RecalculateNormals();

                Vector3[] vertices = new Vector3[4];
                Vector2[] uv = new Vector2[4];
                int[] triangles = new int[6];

                vertices[0] = worldPosition;
                vertices[1] = worldPosition;
                vertices[2] = worldPosition;
                vertices[3] = worldPosition;

                uv[0] = Vector2.zero;
                uv[1] = Vector2.zero;
                uv[2] = Vector2.zero;
                uv[3] = Vector2.zero;

                triangles[0] = 0;
                triangles[1] = 3;
                triangles[2] = 1;

                triangles[3] = 1;
                triangles[4] = 3;
                triangles[5] = 2;

                mesh.vertices = vertices;
                mesh.uv = uv;
                mesh.triangles = triangles;
                mesh.MarkDynamic();

                GetComponent<MeshFilter>().mesh = mesh;
                isDragging = true;
                lastTouchPosition = worldPosition;
            }

            if (isDragging && touch.phase == TouchPhase.Moved)
            {
                float minDistance = .1f;
                if (Vector3.Distance(worldPosition, lastTouchPosition) > minDistance)
                {
                    Vector3[] vertices = new Vector3[mesh.vertices.Length + 2];
                    Vector2[] uv = new Vector2[mesh.uv.Length + 2];
                    int[] triangles = new int[mesh.triangles.Length + 6];

                    mesh.vertices.CopyTo(vertices, 0);
                    mesh.uv.CopyTo(uv, 0);
                    mesh.triangles.CopyTo(triangles, 0);

                    int vIndex = vertices.Length - 4;
                    int vIndex0 = vIndex + 0;
                    int vIndex1 = vIndex0 + 1;
                    int vIndex2 = vIndex0 + 2;
                    int vIndex3 = vIndex0 + 3;

                    Vector3 vec = (worldPosition - lastTouchPosition).normalized;
                    Vector3 normal = drawPlane.normal;
                    //Vector3 normal = new Vector3(0, 0, -1f);
                    Vector3 newVertexUp = worldPosition + Vector3.Cross(vec, normal) * lineThickness;
                    Vector3 newVertexDown = worldPosition + Vector3.Cross(vec, normal * -1f) * lineThickness;

                    vertices[vIndex2] = newVertexUp;
                    vertices[vIndex3] = newVertexDown;

                    uv[vIndex2] = Vector2.zero;
                    uv[vIndex3] = Vector2.zero;

                    int tIndex = triangles.Length - 6;

                    triangles[tIndex + 0] = vIndex0;
                    triangles[tIndex + 1] = vIndex2;
                    triangles[tIndex + 2] = vIndex1;

                    triangles[tIndex + 3] = vIndex1;
                    triangles[tIndex + 4] = vIndex2;
                    triangles[tIndex + 5] = vIndex3;

                    mesh.vertices = vertices;
                    mesh.uv = uv;
                    mesh.triangles = triangles;

                    lastTouchPosition = worldPosition;
                }
            }
            if (touch.phase == TouchPhase.Ended)
            {
                isDragging = false;
            }
        }
    }
}
