using UnityEngine;

public class BezierCameraFollower : MonoBehaviour
{
    public BezierCurve bezierCurve;
    public float speed = 5f;
    public bool loop = true;

    [Header("Cible au centre")]
    public Transform lookTarget;
    public float rotationSmoothSpeed = 5f;

    private const int SAMPLE_COUNT = 100;
    private float[] arcLengths;
    private float totalCurveLength = 0f;
    private float currentDistanceTraveled = 0f;

    void Start()
    {
        InitializeCurveLengths();
    }

    void OnValidate()
    {
        if (Application.isPlaying && bezierCurve != null)
        {
            InitializeCurveLengths();
        }
    }

    public void InitializeCurveLengths()
    {
        if (bezierCurve == null) return;

        arcLengths = new float[SAMPLE_COUNT + 1];
        arcLengths[0] = 0f;
        totalCurveLength = 0f;

        Vector3 prevPoint = GetRawBezierPoint(0f);

        for (int i = 1; i <= SAMPLE_COUNT; i++)
        {
            float t = (float)i / SAMPLE_COUNT;
            Vector3 currentPoint = GetRawBezierPoint(t);
            totalCurveLength += Vector3.Distance(prevPoint, currentPoint);
            arcLengths[i] = totalCurveLength;
            prevPoint = currentPoint;
        }
    }

    void Update()
    {
        if (bezierCurve == null || totalCurveLength <= 0.01f) return;

        currentDistanceTraveled += speed * Time.deltaTime;

        if (currentDistanceTraveled > totalCurveLength)
        {
            if (loop)
            {
                currentDistanceTraveled %= totalCurveLength;
            }
            else
            {
                currentDistanceTraveled = totalCurveLength;
                return;
            }
        }

        float currentT = GetTFromDistance(currentDistanceTraveled);
        Vector3 currentPos = GetRawBezierPoint(currentT);
        transform.position = currentPos;

        Vector3 targetPoint = lookTarget != null ? lookTarget.position : GetCurveCenter();
        Vector3 directionToTarget = (targetPoint - currentPos).normalized;

        if (directionToTarget.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothSpeed * Time.deltaTime);
        }
    }

    private Vector3 GetCurveCenter()
    {
        if (bezierCurve.curveType == BezierType.Quadratic)
        {
            return (bezierCurve.point0.position + bezierCurve.point1.position + bezierCurve.point2.position) / 3f;
        }

        Vector3 p3 = bezierCurve.point3 != null ? bezierCurve.point3.position : bezierCurve.point2.position;
        return (bezierCurve.point0.position + bezierCurve.point1.position + bezierCurve.point2.position + p3) / 4f;
    }

    private float GetTFromDistance(float targetDistance)
    {
        if (targetDistance <= 0f) return 0f;
        if (targetDistance >= totalCurveLength) return 1f;

        for (int i = 0; i < SAMPLE_COUNT; i++)
        {
            if (targetDistance <= arcLengths[i + 1])
            {
                float segmentStartDist = arcLengths[i];
                float segmentEndDist = arcLengths[i + 1];
                float segmentLength = segmentEndDist - segmentStartDist;

                float fraction = (targetDistance - segmentStartDist) / segmentLength;
                float tStart = (float)i / SAMPLE_COUNT;
                float tEnd = (float)(i + 1) / SAMPLE_COUNT;

                return Mathf.Lerp(tStart, tEnd, fraction);
            }
        }

        return 1f;
    }

    private Vector3 GetRawBezierPoint(float t)
    {
        Vector3 p0 = bezierCurve.point0.position;
        Vector3 p1 = bezierCurve.point1.position;
        Vector3 p2 = bezierCurve.point2.position;

        if (bezierCurve.curveType == BezierType.Quadratic)
        {
            return BezierCurve.CalculateQuadraticBezierPoint(t, p0, p1, p2);
        }
        else
        {
            Vector3 p3 = bezierCurve.point3 != null ? bezierCurve.point3.position : p2;
            return BezierCurve.CalculateCubicBezierPoint(t, p0, p1, p2, p3);
        }
    }
}