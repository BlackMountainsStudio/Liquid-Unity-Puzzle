using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineContainer))]
public class RopeController : MonoBehaviour
{
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;
    [Min(10)][SerializeField] private int ropePointCount = 100;
    [SerializeField] float deformSpeed = 10;
    [SerializeField] float maxYDifference = 0.5f;
    private SplineContainer splineContainer;
    private MeshCollider meshCollider;
    private BezierKnot[] knotArray;
    private float3[] initialPointPositions;
    int midPointIndex;
    private float currentMinY;
    void Awake()
    {
        meshCollider = GetComponent<MeshCollider>();
        splineContainer = GetComponent<SplineContainer>();
        midPointIndex = ropePointCount / 2;
        splineContainer.Spline.Knots = new BezierKnot[ropePointCount];
        initialPointPositions = new float3[ropePointCount];
        splineContainer.Spline.SetKnot(0, new BezierKnot(startPoint.position));
        initialPointPositions[0] = startPoint.position;
        splineContainer.Spline.SetKnot(ropePointCount - 1, new BezierKnot(endPoint.position));
        initialPointPositions[ropePointCount - 1] = endPoint.position;
        Vector3 segmentVector = (endPoint.position- startPoint.position) / (ropePointCount - 1);
        for (int i = 1; i < ropePointCount-1; i++)
        {   
            var position = startPoint.position + i * segmentVector;
            splineContainer.Spline.SetKnot(i, new BezierKnot(position));
            initialPointPositions[i] = position;
        }
        knotArray = splineContainer.Spline.Knots.ToArray();

    }

    private void Update()
    {
        return;
        for (int i = 1; i < ropePointCount-1; i++)
        {
            var position = initialPointPositions[i];
            position.y += Mathf.Lerp(-maxYDifference,maxYDifference, Mathf.Sin(Time.time + i));
            splineContainer.Spline.SetKnot(i, new BezierKnot(position));
        }   
    }

    private void OnCollisionStay(Collision other)
    {
        if(other.contacts.Length == 0) return;
        var contactPosition = other.contacts[0];
        var closestPointIndex = GetClosestPointIndex(contactPosition.point.x);
        var minIndex = math.max(0, closestPointIndex - (int)(ropePointCount*.5f));
        var maxIndex = math.min(ropePointCount-1, closestPointIndex + (int)(ropePointCount*.5f));
        for (int i = minIndex; i < maxIndex; i++)
        {
            var distance =  Mathf.Abs(closestPointIndex - i);
            if(closestPointIndex <= distance) continue;
            var speed = deformSpeed * (closestPointIndex - distance) / closestPointIndex;
            MoveRopeSegment(i,speed);
        }
    }

    private void MoveRopeSegment(int index, float moveValue)
    {
        var distanceToMidPoint = math.abs(midPointIndex - index);
        var yDifference = maxYDifference * (midPointIndex-distanceToMidPoint)/midPointIndex;
        var minPositionY = initialPointPositions[index].y-yDifference;
        var position = knotArray[index].Position;
        position.y -= moveValue * Time.fixedDeltaTime;
        position.y = math.max(position.y, minPositionY);
        splineContainer.Spline.SetKnot(index, new BezierKnot(position));
        knotArray[index].Position = position;
        if (currentMinY > position.y)
        {
            currentMinY = position.y;
            Debug.Log(currentMinY);
        }
    }

    //Performans ve kolay hesaplanması için sadece x ekseninde mesafeleri hesapladım.    
    int GetClosestPointIndex(float positionX)
    {
        float x = Mathf.InverseLerp(startPoint.position.x, endPoint.position.x, positionX);
        return (int)(ropePointCount* x);
    }
}
