using UnityEngine;
using TMPro;
using XtremeFPS.InputHandling;
using XtremeFPS.PoolingSystem;
using XtremeFPS.WeaponSystem.Bullet;
using System.Collections;

namespace XtremeFPS.WeaponSystem
{
    [RequireComponent(typeof(AudioSource))]
    [AddComponentMenu("Spoiled Unknown/XtremeFPS/Weapon System")]
    public class UniversalWeaponSystem : MonoBehaviour
    {
        #region Variables
        //Reference
        [Header("References")]
        [SerializeField] private Transform shootPoint;
        [SerializeField] private ParticleSystem muzzleFlash;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private TextMeshProUGUI bulletCount;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform ShellPosition;
        [SerializeField] private GameObject Shell;
        [SerializeField] private GameObject particlesPrefab;

        private XtremeFPSInputHandler inputManager;

        //Bullet Physics
        [Header("Bullet Physics")]
        [SerializeField] private float bulletSpeed;
        [SerializeField] private float bulletDamage;
        [SerializeField] private float bulletLifeTime;
        [SerializeField] private float bulletGravitationalForce;

        //Gun stats
        public int BulletsLeft { get; private set; }
        [Header("Gun Stats")]
        [SerializeField] private float transitionSpeed = 10;
        [SerializeField] private bool isGunAuto;
        [SerializeField] private bool isAimHold;
        [SerializeField] private int magazineSize;
        [SerializeField] private int totalBullets;
        [SerializeField] private int bulletsPerTap;
        [SerializeField] private float timeBetweenShooting;
        [SerializeField] private float reloadTime;

        private bool aiming;
        private int bulletsShot;
        private bool readyToShoot;
        private bool shooting;
        private bool reloading;

        //Aiming
        [Header("Aiming Settings")]
        [SerializeField] private bool canAim;
        [SerializeField] private Transform weaponHolder;
        [SerializeField] private Vector3 aimingLocalPosition = new Vector3(0f, -0.12f, 0.2336001f);

        private Vector3 normalLocalPosition;

        //Weapon Recoil 
        [Header("Weapon Recoil Settings")]
        [SerializeField] private bool haveWeaponRecoil = true;

        [SerializeField] private Vector3 recoilKickBackHip = new Vector3(0.015f, 0f, 0.05f);
        [SerializeField] private Vector3 recoilKickBackAds = new Vector3(-0.08f, 0.01f, 0.009f);
        [Space]
        [SerializeField] private Vector3 recoilRotationHip = new Vector3(10f, 5f, 7f);
        [SerializeField] private Vector3 recoilRotationAds = new Vector3(10f, 4f, 6f);

        private Vector3 rotationRecoil;
        private Vector3 positionRecoil;
        private Vector3 rot;

        //Audio Setup
        [Header("Audio Settings")]
        [SerializeField] private AudioClip bulletSoundClip;
        [SerializeField] private AudioClip bulletReloadClip;

        private AudioSource bulletSoundSource;
        #endregion

        #region MonoBehaviour Callbacks
        private void Start()
        {
            inputManager = XtremeFPSInputHandler.Instance;
            bulletSoundSource = GetComponent<AudioSource>();

            BulletsLeft = magazineSize;
            if (canAim) normalLocalPosition = weaponHolder.localPosition;
            bulletCount.text = $"{BulletsLeft / bulletsPerTap} / {totalBullets / bulletsPerTap}";

            readyToShoot = true;
        }

        private void OnEnable()
        {
            bulletCount.text = $"{BulletsLeft / bulletsPerTap} / {totalBullets / bulletsPerTap}";
        }

        private void Update()
        {
            PlayerWeaponsInput();
            DetermineAim();
            HandleWeaponRecoil();
        }
        #endregion

        #region Private Methods
        
        private bool ShouldReload()
        {
            bool reloadTriggered = inputManager.IsTryingToReload || BulletsLeft <= 0;
            bool hasAmmo = totalBullets > 0;
            bool notReloading = !reloading;
            bool magazineNotFull = BulletsLeft < magazineSize;

            return reloadTriggered && hasAmmo && notReloading && magazineNotFull;
        }

        private void PlayerWeaponsInput()
        { 
            shooting = isGunAuto ? inputManager.IsShootHold : inputManager.IsShootTap;
            aiming = isAimHold ? inputManager.IsAimHold : inputManager.IsAimTap;

            if (ShouldReload()) StartCoroutine(Reload());

            //Shoot
            if (!readyToShoot || !shooting || reloading || BulletsLeft <= 0) return;
            bulletsShot = bulletsPerTap;
            Shoot();
            bulletSoundSource.PlayOneShot(bulletSoundClip);
        }

        private void Shoot()
        {
            readyToShoot = false;

            GameObject bulletObject = PoolManager.Instance.SpawnObject(bulletPrefab, shootPoint.position, Quaternion.identity);
            ParabolicBullet parabolicBullet = bulletObject.GetComponent<ParabolicBullet>();
            parabolicBullet.Initialize(shootPoint, bulletSpeed, bulletDamage, bulletGravitationalForce, bulletLifeTime, particlesPrefab);

            //Graphics
            muzzleFlash.Play();

            PoolManager.Instance.SpawnObject(Shell, ShellPosition.position, ShellPosition.rotation);

            if (aiming)
            {
                rotationRecoil += new Vector3(-recoilRotationAds.x, Random.Range(-recoilRotationAds.y, recoilRotationAds.y), Random.Range(-recoilRotationAds.z, recoilRotationAds.z));
                positionRecoil += new Vector3(Random.Range(-recoilKickBackAds.x, recoilKickBackAds.y), Random.Range(-recoilKickBackAds.y, recoilKickBackAds.y), recoilKickBackAds.z);
            }
            else
            {
                rotationRecoil += new Vector3(-recoilRotationHip.x, Random.Range(-recoilRotationHip.y, recoilRotationHip.y), Random.Range(-recoilRotationHip.z, recoilRotationHip.z));
                positionRecoil += new Vector3(Random.Range(-recoilKickBackHip.x, recoilKickBackHip.y), Random.Range(-recoilKickBackHip.y, recoilKickBackHip.y), recoilKickBackHip.z);
            }

            BulletsLeft--;
            bulletsShot--;

            bulletCount.text = $"{BulletsLeft / bulletsPerTap} / {totalBullets / bulletsPerTap}";

            Invoke(nameof(ResetShot), timeBetweenShooting);
            if (bulletsShot > 0 && BulletsLeft > 0) Invoke(nameof(Shoot), 0.01f);
        }

        private void ResetShot()
        {
            readyToShoot = true;
        }

        IEnumerator Reload()
        {
            reloading = true;
            animator.SetBool("IsReloading", true);
            bulletSoundSource.PlayOneShot(bulletReloadClip);

            yield return new WaitForSeconds(reloadTime);

            reloading = false;
            animator.SetBool("IsReloading", false);


            switch (totalBullets.CompareTo(magazineSize))
            {
                case 1:  // totalBullets > magazineSize
                    BulletsLeft = magazineSize;
                    totalBullets -= magazineSize;
                    break;
                case 0:  // totalBullets == magazineSize
                    BulletsLeft = magazineSize;
                    totalBullets -= magazineSize;
                    break;
                case -1: // totalBullets < magazineSize
                    BulletsLeft = totalBullets;
                    totalBullets = 0;
                    break;
            }
            bulletCount.text = $"{BulletsLeft / bulletsPerTap} / {totalBullets / bulletsPerTap}";
        }

        private void DetermineAim()
        {
            if (!canAim) return;

            Vector3 target = normalLocalPosition;
            if (aiming) target = aimingLocalPosition;

            Vector3 desiredPosition = Vector3.Lerp(weaponHolder.transform.localPosition, target, Time.deltaTime * transitionSpeed);
            weaponHolder.transform.localPosition = desiredPosition;
        }

        private void HandleWeaponRecoil()
        {
            if(!haveWeaponRecoil) return;
            rotationRecoil = Vector3.Lerp(rotationRecoil, Vector3.zero, transitionSpeed * Time.deltaTime);
            positionRecoil = Vector3.Lerp(positionRecoil, Vector3.zero, transitionSpeed * Time.deltaTime);

            transform.localPosition = Vector3.Slerp(transform.localPosition, positionRecoil, transitionSpeed * Time.deltaTime);
            rot = Vector3.Slerp(rot, rotationRecoil, transitionSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.Euler(rot);
        }
        #endregion
    }
}