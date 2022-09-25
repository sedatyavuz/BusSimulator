using System.Collections;
using UnityEngine;

public class Shake : MonoBehaviour
{
    [SerializeField] private float duration, magnitude;
    CameraFollower cameraFollower;
    [SerializeField] private Transform target;

    private void Start()
    {
        cameraFollower = GetComponent<CameraFollower>();
    }
    public IEnumerator ShakeAnimation(float duration, float magnitude)
    {
        cameraFollower.isCameraFollow = false;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.position = new Vector3(target.position.x+x, target.position.y + y, target.position.z);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        cameraFollower.isCameraFollow = true;
    }

    public void StartShake(float _duration, float _magnitude)
    {
        StartCoroutine(ShakeAnimation(_duration, _magnitude));
    }

    public void StartShake()
    {
        StartCoroutine(ShakeAnimation(duration, magnitude));
    }
}