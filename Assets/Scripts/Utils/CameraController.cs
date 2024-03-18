using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Utils
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController instance;
        private Transform followerTr;
        private Vector3 dampVel;
        private float[] xLimit = new float[2];
        private float[] yLimit = new float[2];
        private Camera cameraComp;
        
        [Header("Camera Follow")]
        [SerializeField] private float dampingTime;
        [SerializeField] private Vector2 offset;
        
        [Header("Shake Effect")] 
        [SerializeField] private float shakeDuration = 0.01f;
        [SerializeField] private float shakeMagnitude = 0.03f;
        public float shakeTime = 0.8f;
        
        private bool borderFlag = false;
        
        private void Awake()
        {
            instance = this;
            cameraComp = transform.GetChild(0).GetComponent<Camera>();
        }

        public static void SetFollow(GameObject obj)
        {
            instance.followerTr = obj.transform;
        }

        public static void SetBorder(float xMin, float xMax, float yMin, float yMax)
        {
            instance.xLimit[0] = xMin;
            instance.xLimit[1] = xMax;

            instance.yLimit[0] = yMin;
            instance.yLimit[1] = yMax;

            instance.borderFlag = true;
        }

        public static void DeleteBorder()
        {
            instance.borderFlag = false;
        }

        public static void Shake(float duration = .0f, float magnitude = .0f)
        {
            if (duration <= 0) duration = instance.shakeDuration;
            if (magnitude <= 0) magnitude = instance.shakeMagnitude;
            instance.StartCoroutine(instance.StartShake(duration, magnitude));
            // Debug.Log("Shake");
        }
        
        private IEnumerator StartShake(float duration, float magnitude)
        {
            Vector3 origin = Vector3.zero;
            float passed = .0f;
            while (duration > passed)
            {
                passed += Time.deltaTime;
                cameraComp.transform.localPosition = origin + Vector3.down * (Mathf.Sin(passed) * magnitude / passed);
                yield return null;
            }

            cameraComp.transform.localPosition = origin;
        }

        private IEnumerator StartShake(float frequency, float ksai, float r, float initialPoint, float duration)
        {
            float k1 = ksai / Mathf.PI / frequency;
            float k2 = 1 / Mathf.Pow(2 * Mathf.PI * frequency,2);
            float k3 = r * ksai / (2 * Mathf.PI * frequency);

            float x = initialPoint;
            float y = initialPoint;
            float dy = .0f;

            float passedTime = .0f;
            while (passedTime < duration)
            {
                passedTime += Time.deltaTime;
                
                var target = transform;
                var targetLocal = target.localPosition;
                
                float dx = (targetLocal.y - x) / Time.deltaTime;
                x = targetLocal.y;

                y += Time.deltaTime * dy;
                dy = dy * Time.deltaTime * (x + k3 * dx - y - k1 * dy) / k2;

                targetLocal.y = y;
                target.localPosition = targetLocal;
                yield return null;
            }
        }

        void Update()
        {
            if (!ReferenceEquals(followerTr, null))
            {
                var position = transform.position;
                var target = Vector3.SmoothDamp(position, followerTr.position + new Vector3(offset.x, offset.y, position.z),
                    ref dampVel, dampingTime);
                
                if (borderFlag)
                {
                    target.x = Mathf.Clamp(target.x, xLimit[0], xLimit[1]);
                    target.y = Mathf.Clamp(target.y, yLimit[0], yLimit[1]);
                }

                transform.position = target;
            }
        }
    }
}