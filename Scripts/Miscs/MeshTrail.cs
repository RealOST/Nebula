using System.Collections;
using UnityEngine;

public class MeshTrail : MonoBehaviour {
    [SerializeField] private GameObject meshTrailPrefab;
    [SerializeField] private GameObject model;
    [SerializeField] private float fadeDuration = 0.5f;

    public void Spawn() {
        StartCoroutine(SpawnAndFade());
    }

    private IEnumerator SpawnAndFade() {
        var mt = PoolManager.Release(meshTrailPrefab);

        mt.transform.position = model.transform.position;
        mt.transform.rotation = model.transform.rotation;
        mt.transform.localScale = model.transform.localScale;

        var mtRenderer = mt.GetComponent<MeshRenderer>();

        mtRenderer.material = new Material(meshTrailPrefab.GetComponent<MeshRenderer>().material);

        var originalColor = mtRenderer.material.color;

        var elapsed = 0f;
        while (elapsed < fadeDuration) {
            var alpha = Mathf.Lerp(originalColor.a, 0f, elapsed / fadeDuration);
            var c = originalColor;
            c.a = alpha;
            mtRenderer.material.color = c;

            elapsed += Time.deltaTime;
            yield return null;
        }

        mt.SetActive(false); // 放回池中
    }
}