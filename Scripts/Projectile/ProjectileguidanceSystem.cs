using System.Collections;
using UnityEngine;

public class ProjectileguidanceSystem : MonoBehaviour {
    [SerializeField] private Projectile projectile;
    [SerializeField] private float minBallisticAngle = -50f;
    [SerializeField] private float maxBallisticAngle = 50f;

    private float ballisticAngle;

    private Vector3 targetDirection;

    public IEnumerator HomingCoroutine(GameObject target) {
        ballisticAngle = Random.Range(minBallisticAngle, maxBallisticAngle);
        while (gameObject.activeSelf) {
            if (target.activeSelf) {
                targetDirection = target.transform.position - gameObject.transform.position;

                // var angle = ;
                // transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation =
                    Quaternion.AngleAxis(Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg,
                        Vector3.forward);
                transform.rotation *= Quaternion.Euler(0f, 0f, ballisticAngle);
                projectile.Move();
            }
            else {
                projectile.Move();
            }

            yield return null;
        }
    }
}