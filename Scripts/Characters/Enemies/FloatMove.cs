using UnityEngine;

public class FloatMove : MonoBehaviour {
    public float speed = 1f;
    public float amplitude = 0.5f;
    private Vector3 startPos;

    private void Start() {
        startPos = transform.position;
    }

    private void Update() {
        var x = Time.time * speed;
        var y = Mathf.Sin(2 * x) * amplitude;
        transform.position = startPos + new Vector3(-x, y, 0);
    }
}