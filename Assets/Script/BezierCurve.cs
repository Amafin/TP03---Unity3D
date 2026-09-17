using UnityEngine;

public enum BezierType
{
    Quadratic,
    Cubic
}

[ExecuteAlways] // Permet d'actualiser et voir la courbe dans l'éditeur hors Play Mode
[RequireComponent(typeof(LineRenderer))]
public class BezierCurve : MonoBehaviour
{
    [Header("Type de Courbe")]
    public BezierType curveType = BezierType.Cubic;

    [Range(10, 150)]
    public int resolution = 50; // Nombre de segments pour lisser la courbe

    [Header("Points de Contrôle")]
    public Transform point0;
    public Transform point1;
    public Transform point2;
    public Transform point3; // Utilisé uniquement en mode Cubique

    [Header("Visibilité")]
    [Tooltip("Si vrai, le LineRenderer est caché de la GameView (masqué pour la Main Camera)")]
    public bool hideInGameView = true;

    private LineRenderer lineRenderer;
    private Camera mainCam;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SetupLineRenderer();
    }

    void OnEnable()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SetupLineRenderer();
        Camera.onPreCull += OnCameraPreCull;
        Camera.onPostRender += OnCameraPostRender;
    }

    void OnDisable()
    {
        Camera.onPreCull -= OnCameraPreCull;
        Camera.onPostRender -= OnCameraPostRender;
    }

    void Update()
    {
        DrawCurve();
    }

    private void SetupLineRenderer()
    {
        if (lineRenderer == null) return;

        lineRenderer.useWorldSpace = true;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;

        // Assigne un matériau de base si aucun n'est configuré
        if (lineRenderer.sharedMaterial == null)
        {
            lineRenderer.sharedMaterial = new Material(Shader.Find("Sprites/Default"));
        }
    }

    // --- ALGORITHMES DE BÉZIER ---

    // Bézier Quadratique : B(t) = (1-t)²*P0 + 2(1-t)t*P1 + t²*P2
    public static Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1f - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 point = uu * p0;
        point += 2f * u * t * p1;
        point += tt * p2;
        return point;
    }

    // Bézier Cubique : B(t) = (1-t)³*P0 + 3(1-t)²t*P1 + 3(1-t)t²*P2 + t³*P3
    public static Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1f - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 point = uuu * p0;
        point += 3f * uu * t * p1;
        point += 3f * u * tt * p2;
        point += ttt * p3;
        return point;
    }

    // --- CALCUL ET AFFICHAGE EN TEMPS RÉEL ---
    private void DrawCurve()
    {
        if (lineRenderer == null) return;
        if (!HasValidPoints()) return;

        lineRenderer.positionCount = resolution + 1;

        Vector3 p0 = point0.position;
        Vector3 p1 = point1.position;
        Vector3 p2 = point2.position;
        Vector3 p3 = point3 != null ? point3.position : Vector3.zero;

        for (int i = 0; i <= resolution; i++)
        {
            float t = (float)i / resolution;
            Vector3 pointOnCurve = Vector3.zero;

            if (curveType == BezierType.Quadratic)
            {
                pointOnCurve = CalculateQuadraticBezierPoint(t, p0, p1, p2);
            }
            else if (curveType == BezierType.Cubic)
            {
                pointOnCurve = CalculateCubicBezierPoint(t, p0, p1, p2, p3);
            }

            lineRenderer.SetPosition(i, pointOnCurve);
        }
    }

    private bool HasValidPoints()
    {
        if (point0 == null || point1 == null || point2 == null) return false;
        if (curveType == BezierType.Cubic && point3 == null) return false;
        return true;
    }

    // --- MASQUAGE POUR LA CAMÉRA DE JEU (VISIBLE SCENE SEULEMENT) ---
    private void OnCameraPreCull(Camera cam)
    {
        if (!hideInGameView || lineRenderer == null) return;

        // Si la caméra qui rend la vue est la caméra du jeu (pas la vue Scene)
        if (cam.cameraType == CameraType.Game)
        {
            lineRenderer.enabled = false;
        }
        else
        {
            lineRenderer.enabled = true;
        }
    }

    private void OnCameraPostRender(Camera cam)
    {
        if (!hideInGameView || lineRenderer == null) return;

        // Réactive le LineRenderer pour les autres caméras (notamment la Scene View)
        lineRenderer.enabled = true;
    }

    // --- GIZMOS D'AIDE DANS LA VUE SCENE ---
    private void OnDrawGizmos()
    {
        if (!HasValidPoints()) return;

        // Dessine les lignes directrices (tangentes)
        Gizmos.color = Color.gray;
        Gizmos.DrawLine(point0.position, point1.position);

        if (curveType == BezierType.Quadratic)
        {
            Gizmos.DrawLine(point1.position, point2.position);
        }
        else
        {
            Gizmos.DrawLine(point1.position, point2.position);
            Gizmos.DrawLine(point2.position, point3.position);
        }

        // Sphères sur les points de contrôle
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(point0.position, 0.15f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(point1.position, 0.12f);
        Gizmos.DrawSphere(point2.position, 0.12f);

        if (curveType == BezierType.Cubic && point3 != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(point3.position, 0.15f);
        }
    }
}