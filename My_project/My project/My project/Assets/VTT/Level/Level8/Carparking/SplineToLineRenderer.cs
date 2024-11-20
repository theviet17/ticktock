using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class SplineToLineRenderer : MonoBehaviour
{
    //public SplineComputer splineComputer; // Gán Spline Computer vào từ Inspector
    //private LineRenderer lineRenderer;

    //[Range(10, 100)]
    //public int resolution = 50; // Độ phân giải của Line Renderer (số lượng điểm trên spline)

    //private void Start()
    //{
    //    lineRenderer = GetComponent<LineRenderer>();
    //    UpdateLineRenderer();
    //}

    //private void UpdateLineRenderer()
    //{
    //    if (splineComputer == null) return;

    //    // Cập nhật số lượng điểm của Line Renderer theo độ phân giải
    //    lineRenderer.positionCount = resolution;

    //    // Lấy vị trí từng điểm trên spline và gán vào Line Renderer
    //    for (int i = 0; i < resolution; i++)
    //    {
    //        float t = (float)i / (resolution - 1); // Tính tỉ lệ trên spline
    //        Vector3 position = splineComputer.EvaluatePosition(t); // Lấy vị trí tại điểm t
    //        lineRenderer.SetPosition(i, position); // Gán vị trí cho Line Renderer
    //    }
    //}
    public List<Transform> points; // Danh sách các điểm đã định sẵn
    private LineRenderer lineRenderer;
    public GameObject car;
    public CarParkGameManager carParkGameManager;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        // Đảm bảo có ít nhất hai điểm để vẽ đường
        if (points.Count < 2)
        {
            Debug.LogWarning("Cần ít nhất hai điểm để vẽ đường.");
            return;
        }

        // Cập nhật số lượng điểm của Line Renderer
        lineRenderer.positionCount = points.Count;

        // Gán từng điểm vào Line Renderer
        for (int i = 0; i < points.Count; i++)
        {
            lineRenderer.SetPosition(i, points[i].position);
        }
        InstanceCar();
    }

    public void InstanceCar()
    {
        car.transform.position = points[0].transform.position;
        car.transform.up = points[0].transform.up;
        car.GetComponent<BoxCollider2D>().isTrigger = false;
        carParkGameManager.cars.Add(car.GetComponent<Car>());
        car.GetComponent<Car>().GetPaths(points);

        car.name = "Car " + gameObject.name;
    }
}
