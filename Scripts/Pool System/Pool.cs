using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pool {
    public GameObject Prefab => prefab;
    public int Size => size;
    public int RuntimeSize => queue.Count;

    [SerializeField] private GameObject prefab;
    [SerializeField] private int size = 1;

    private Queue<GameObject> queue;

    private Transform parent;

    public void Initialize(Transform parent) {
        queue = new Queue<GameObject>();
        this.parent = parent;

        for (var i = 0; i < size; i++) queue.Enqueue(Copy());
    }

    private GameObject Copy() {
        var copy = Object.Instantiate(prefab, parent);

        copy.SetActive(false);

        return copy;
    }

    private GameObject AvailableObject() {
        GameObject availableObject = null;

        if (queue.Count > 0 && !queue.Peek().activeSelf)
            availableObject = queue.Dequeue();
        else
            availableObject = Copy();

        queue.Enqueue(availableObject);

        return availableObject;
    }

    public GameObject PreparedObject() {
        var preparedGameObject = AvailableObject();

        preparedGameObject.SetActive(true);

        return preparedGameObject;
    }

    public GameObject PreparedObject(Vector3 position) {
        var preparedGameObject = AvailableObject();

        preparedGameObject.SetActive(true);
        preparedGameObject.transform.position = position;

        return preparedGameObject;
    }

    public GameObject PreparedObject(Vector3 position, Quaternion rotation) {
        var preparedGameObject = AvailableObject();

        preparedGameObject.SetActive(true);
        preparedGameObject.transform.position = position;
        preparedGameObject.transform.rotation = rotation;

        return preparedGameObject;
    }

    public GameObject PreparedObject(Vector3 position, Quaternion rotation, Vector3 localScale) {
        var preparedGameObject = AvailableObject();

        preparedGameObject.SetActive(true);
        preparedGameObject.transform.position = position;
        preparedGameObject.transform.rotation = rotation;
        preparedGameObject.transform.localScale = localScale;

        return preparedGameObject;
    }
}