/*Copyright © Spoiled Unknown*/
/*2024*/

using System.Collections;
using UnityEngine;
using XtremeFPS.PoolingSystem;
using XtremeFPS.Interfaces;

namespace XtremeFPS.WeaponSystem
{
    public class ParabolicBullet : MonoBehaviour
    {
        #region Variables
        private float speed;
        private float damage;
        private float gravity;
        private Vector3 startPosition;
        private Vector3 startForward;
        private GameObject particlesPrefab;
        private float bulletLiftime;

        private float startTime = -1;
        private Vector3 currentPoint;
        #endregion

        #region Initialization
        public void Initialize(Transform startPoint, float speed, float damage, float gravity, float bulletLifetime, GameObject particlePrefab)
        {
            this.startPosition = startPoint.position;
            this.startForward = startPoint.forward.normalized;
            this.speed = speed;
            this.damage = damage;
            this.gravity = gravity;
            this.particlesPrefab = particlePrefab;
            this.bulletLiftime = bulletLifetime;
        }
        #endregion

        #region MonoBehaviour Callbacks
        void OnEnable()
        {
            StartCoroutine(DestroyBullets());
            startTime = -1f;
        }

        private void FixedUpdate()
        {
            if (startTime < 0) startTime = Time.time;

            float currentTime = Time.time - startTime;
            float prevTime = currentTime - Time.fixedDeltaTime;
            float nextTime = currentTime + Time.fixedDeltaTime;

            RaycastHit hit;
            Vector3 currentPoint = FindPointOnParabola(currentTime);

            if (prevTime > 0)
            {
                Vector3 prevPoint = FindPointOnParabola(prevTime);
                if (CastRayBetweenPoints(prevPoint, currentPoint, out hit)) OnHit(hit);
            }

            Vector3 nextPoint = FindPointOnParabola(nextTime);
            if (CastRayBetweenPoints(currentPoint, nextPoint, out hit)) OnHit(hit);
        }
        private void Update()
        {
            if (startTime < 0) return;

            float currentTime = Time.time - startTime;
            currentPoint = FindPointOnParabola(currentTime);
            transform.position = currentPoint;
        }
        #endregion

        #region Private Methods
        private Vector3 FindPointOnParabola(float time)
        {
            Vector3 point = startPosition + (speed * time * startForward);
            Vector3 gravityVec = gravity * time * time * Vector3.down;
            return point + gravityVec;
        }

        private bool CastRayBetweenPoints(Vector3 startPoint, Vector3 endPoint, out RaycastHit hit)
        {
            return Physics.Raycast(startPoint, endPoint - startPoint, out hit, (endPoint - startPoint).magnitude);
        }

        private void OnHit(RaycastHit hit)
        {
            if (hit.transform.TryGetComponent<IShootableObject>(out IShootableObject shootableObject)) shootableObject.OnHit(hit, damage);

            GameObject HitObject = PoolManager.Instance.SpawnObject(particlesPrefab, hit.point + hit.normal * 0.05f, Quaternion.LookRotation(hit.normal));
            HitObject.transform.parent = hit.transform;

            OnBulletDestroy();
        }

        private IEnumerator DestroyBullets()
        {
            yield return new WaitForSeconds(bulletLiftime);
            OnBulletDestroy();
        }

        private void OnBulletDestroy()
        {
            PoolManager.Instance.DespawnObject(this.gameObject);
        }
        #endregion
    }
}