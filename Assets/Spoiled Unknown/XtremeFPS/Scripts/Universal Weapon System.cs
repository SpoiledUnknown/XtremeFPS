/*Copyright © Spoiled Unknown*/
/*2024*/

using UnityEngine;
using TMPro;
using XtremeFPS.InputHandling;
using XtremeFPS.PoolingSystem;
using XtremeFPS.FPSController;

namespace XtremeFPS.WeaponSystem
{
    [RequireComponent(typeof(AudioSource))]
    [AddComponentMenu("Spoiled Unknown/XtremeFPS/Weapon System")]
    public class UniversalWeaponSystem : MonoBehaviour
    {
        #region Variables
        //Reference
        public FirstPersonController fpsController;
        public Transform shootPoint;
        public ParticleSystem muzzleFlash;
        public GameObject bulletPrefab;
        public TextMeshProUGUI bulletCount;
        public Animator animator;
        public GameObject aimUIImage;

        private FPSInputManager inputManager;

        //Bullet Physics
        public float bulletSpeed;
        public float bulletDamage;
        public float bulletLifeTime;
        public float bulletGravitationalForce;

        //Bullet Shell
        public Transform ShellPosition;
        public GameObject Shell;
        public GameObject particlesPrefab;

        //Gun stats
        public int BulletsLeft { get; private set; }
        public bool isGunAuto;
        public bool isAimHold;
        public float timeBetweenEachShots;
        public float timeBetweenShooting;
        public int magazineSize;
        public int totalBullets;
        public int bulletsPerTap;
        public float reloadTime;
        public bool aiming;
        public bool hardMode;

        private int bulletsShot;
        private bool readyToShoot;
        private bool shooting;
        private bool reloading;

        //Aiming
        public bool canAim = true;
        public Transform weaponHolder;
        public Vector3 aimingLocalPosition = new Vector3(0f, -0.12f, 0.2336001f);
        public float aimSmoothing = 6;

        private Vector3 normalLocalPosition;
       
        //Camera Recoil 
        public bool haveCameraRecoil = true;
        public Transform cameraRecoilHolder;
        public float recoilRotationSpeed = 6f;
        public float recoilReturnSpeed = 25f;
        public Vector3 hipFireRecoil = new Vector3(4f, 4f, 4f);
        public Vector3 adsFireRecoil = new Vector3(2f, 2f, 2f);
        public float hRecoil = 0.215f;
        public float vRecoil = 0.221f;

        private Vector3 currentRotation;
        private Vector3 Rot;

        //Weapon Recoil 
        public bool haveWeaponRecoil = true;
        public Transform gunPositionHolder;
        public float gunRecoilPositionSpeed = 8f;
        public float gunPositionReturnSpeed = 10f;
        public Vector3 recoilKickBackHip = new Vector3(0.015f, 0f, 0.05f);
        public Vector3 recoilKickBackAds = new Vector3(-0.08f, 0.01f, 0.009f);
        public float gunRecoilRotationSpeed = 8f;
        public float gunRotationReturnSpeed = 38f;
        public Vector3 recoilRotationHip = new Vector3(10f, 5f, 7f);
        public Vector3 recoilRotationAds = new Vector3(10f, 4f, 6f);

        private Vector3 rotationRecoil;
        private Vector3 positionRecoil;
        private Vector3 rot;

        //Weapon Rotational Sway
        public bool haveRotationalSway = true;
        public float rotaionSwayIntensity = 10f;
        public float rotationSwaySmoothness = 2f;

        private Quaternion originRotation;
        private float mouseX;
        private float mouseY;

        //Jump Sway
        public bool haveJumpSway = true;
        public float jumpIntensity = 5f;
        public float weaponMaxClamp = 20f;
        public float weaponMinClamp = 20f;
        public float jumpSmooth = 15f;
        public float landingIntensity = 5f;
        public float landingSmooth = 15f;
        public float recoverySpeed = 50f;

        private float impactForce = 0;

        //Weapon Move Bobbing
        public bool haveBobbing = true;
        public float magnitude = 0.009f;
        public float idleSpeed = 2f;
        public float walkSpeedMultiplier = 4f;
        public float walkSpeedMax = 6f;
        public float aimReduction = 4f;

        private float sinY = 0f;
        private float sinX = 0f;
        private Vector3 lastPosition;

        //Audio Setup
        public AudioClip bulletSoundClip;
        public AudioClip bulletReloadClip;

        private AudioSource bulletSoundSource;
        #endregion

        #region MonoBehaviour Callbacks
        private void Start()
        {
            inputManager = FPSInputManager.Instance;
            bulletSoundSource = GetComponent<AudioSource>();

            BulletsLeft = magazineSize;

            lastPosition = transform.position;
            if (canAim) normalLocalPosition = weaponHolder.transform.localPosition;
            if (haveRotationalSway) originRotation = transform.localRotation;

            SetBulletCountUI();
            readyToShoot = true;
        }

        private void Update()
        {
            PlayerWeaponsInput();

            DetermineAim();

            HandleWeaponRecoil();
            HandleCameraRecoil();

            WeaponRotationSway();
            WeaponBobbing();
            JumpSwayEffect();
        }
        #endregion

        #region Private Methods
        private void PlayerWeaponsInput()
        {
             if (isGunAuto) shooting = inputManager.isFiringHold;
             else shooting = inputManager.isFiringTapped;

            //handle mouse inputs
            mouseX = inputManager.mouseDirection.x;
            mouseY = inputManager.mouseDirection.y;

            if (isAimHold) aiming = inputManager.isAimingHold;
            else aiming = inputManager.isAimingTapped;

            if ((inputManager.isReloading || BulletsLeft == 0)
                && BulletsLeft < magazineSize
                && totalBullets > 0
                && !reloading) Reload();

            //Shoot
            if (readyToShoot
                && shooting
                && !reloading
                && BulletsLeft > 0)
            {
                bulletsShot = bulletsPerTap;
                Shoot();
                bulletSoundSource.PlayOneShot(bulletSoundClip);
            }
            else fpsController.AddRecoil(0f, 0f);
        }

        #region Shooting && Reloading
        private void Shoot()
        {
            readyToShoot = false;

            GameObject bulletObject = PoolManager.Instance.SpawnObject(bulletPrefab, shootPoint.position, Quaternion.identity);
            ParabolicBullet parabolicBullet = bulletObject.GetComponent<ParabolicBullet>();
            parabolicBullet.Initialize(shootPoint, bulletSpeed, bulletDamage, bulletGravitationalForce, bulletLifeTime, particlesPrefab);

            //Graphics
            muzzleFlash.Play();

            PoolManager.Instance.SpawnObject(Shell, ShellPosition.position, ShellPosition.rotation);
            float hRecoil = Random.Range(-this.hRecoil, this.hRecoil);

            if (aiming)
            {
                currentRotation += new Vector3(-adsFireRecoil.x, Random.Range(-adsFireRecoil.y, adsFireRecoil.y), Random.Range(-adsFireRecoil.z, adsFireRecoil.z));
                rotationRecoil += new Vector3(-recoilRotationAds.x, Random.Range(-recoilRotationAds.y, recoilRotationAds.y), Random.Range(-recoilRotationAds.z, recoilRotationAds.z));
                positionRecoil += new Vector3(Random.Range(-recoilKickBackAds.x, recoilKickBackAds.y), Random.Range(-recoilKickBackAds.y, recoilKickBackAds.y), recoilKickBackAds.z);

                fpsController.AddRecoil(hRecoil * 0.5f, vRecoil * 0.5f);
            }
            else
            {
                currentRotation += new Vector3(-hipFireRecoil.x, Random.Range(-hipFireRecoil.y, hipFireRecoil.y), Random.Range(-hipFireRecoil.z, hipFireRecoil.z));
                rotationRecoil += new Vector3(-recoilRotationHip.x, Random.Range(-recoilRotationHip.y, recoilRotationHip.y), Random.Range(-recoilRotationHip.z, recoilRotationHip.z));
                positionRecoil += new Vector3(Random.Range(-recoilKickBackHip.x, recoilKickBackHip.y), Random.Range(-recoilKickBackHip.y, recoilKickBackHip.y), recoilKickBackHip.z);

                fpsController.AddRecoil(hRecoil, vRecoil);
            }

            BulletsLeft--;
            bulletsShot--;

            SetBulletCountUI();

            Invoke(nameof(ResetShot), timeBetweenShooting);
            if (bulletsShot > 0 && BulletsLeft > 0) Invoke(nameof(Shoot), timeBetweenEachShots);
        }
        private void ResetShot()
        {
            readyToShoot = true;
        }
        private void Reload()
        {
            reloading = true;
            HandleReloadAnimation();
            bulletSoundSource.PlayOneShot(bulletReloadClip);
            Invoke(nameof(ReloadFinished), reloadTime);
        }
        private void HandleReloadAnimation()
        {
            animator.SetBool("IsReloading", reloading);
        }
        private void ReloadFinished()
        {
            reloading = false;
            HandleReloadAnimation();
            if (hardMode)
            {
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
                    default:
                        // Handle the case when totalBullets and magazineSize cannot be compared directly
                        // User can call functions here, like display text saying out of ammo...
                        break;
                }
            }
            else //if hardMode is false
            {
                if ((BulletsLeft + totalBullets) >= magazineSize)
                {
                    int bulletsNeededForReload = magazineSize - BulletsLeft;
                    BulletsLeft += bulletsNeededForReload;
                    totalBullets -= bulletsNeededForReload;
                }
                else
                {
                    int bulletsNeededForReload = magazineSize - BulletsLeft;
                    BulletsLeft += Mathf.Min(bulletsNeededForReload, totalBullets);
                    totalBullets -= bulletsNeededForReload;
                    totalBullets = Mathf.Max(0, totalBullets);
                }
            }
            SetBulletCountUI();
        }
        private void SetBulletCountUI()
        {
            if (bulletCount == null) return;
            bulletCount.SetText(BulletsLeft + " / " + totalBullets);
        }
        #endregion
        #region Recoil
        private void HandleWeaponRecoil()
        {
            if(!haveWeaponRecoil) return;
            rotationRecoil = Vector3.Lerp(rotationRecoil, Vector3.zero, gunRotationReturnSpeed * Time.deltaTime);
            positionRecoil = Vector3.Lerp(positionRecoil, Vector3.zero, gunPositionReturnSpeed * Time.deltaTime);

            gunPositionHolder.localPosition = Vector3.Slerp(gunPositionHolder.localPosition, positionRecoil, gunRecoilPositionSpeed * Time.deltaTime);
            rot = Vector3.Slerp(rot, rotationRecoil, gunRecoilRotationSpeed* Time.deltaTime);
            gunPositionHolder.localRotation = Quaternion.Euler(rot);
        }
        private void HandleCameraRecoil()
        {
            if (!haveCameraRecoil) return;

            currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, recoilReturnSpeed * Time.deltaTime);
            Rot = Vector3.Slerp(Rot, currentRotation, recoilRotationSpeed * Time.deltaTime);
            cameraRecoilHolder.transform.localRotation = Quaternion.Euler(Rot);
        }
        #endregion
        private void DetermineAim()
        {
            if (!canAim) return;

            Vector3 target = normalLocalPosition;
            if (aiming) target = aimingLocalPosition;

            Vector3 desiredPosition = Vector3.Lerp(weaponHolder.transform.localPosition, target, Time.deltaTime * aimSmoothing);
            weaponHolder.transform.localPosition = desiredPosition;
            if (aimUIImage != null)
            {
                aimUIImage.SetActive(aiming);
                animator.gameObject.SetActive(!aiming);
                fpsController.enableZoom = aiming;
            }

        }
        #region Effects
            private void WeaponRotationSway()
        {
            if(!haveRotationalSway) return;

            Quaternion newAdjustedRotationX = Quaternion.AngleAxis(rotaionSwayIntensity * mouseX * -1f, Vector3.up);
            Quaternion targetRotation = originRotation * newAdjustedRotationX;
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, rotationSwaySmoothness * Time.deltaTime);
        }
        private void WeaponBobbing()
        {
            if(!haveBobbing || fpsController.MovementState == FirstPersonController.PlayerMovementState.Sliding) return;

            if (!fpsController.IsGrounded)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime);
                return;
            }

            // Calculate delta time based on the player's movement speed.
            float delta = Time.deltaTime * idleSpeed;
            float velocity = (lastPosition - transform.position).magnitude * walkSpeedMultiplier;
            delta += Mathf.Clamp(velocity, 0, walkSpeedMax);

            // Update the sinX and sinY values to create a bobbing effect.
            sinX += delta / 2;
            sinY += delta;
            sinX %= Mathf.PI * 2;
            sinY %= Mathf.PI * 2;

            // Adjust the weapon's local position to create the bobbing effect.
            float magnitude = aiming ? this.magnitude / aimReduction : this.magnitude;
            transform.localPosition = Vector3.zero + magnitude * Mathf.Sin(sinY) * Vector3.up;
            transform.localPosition += magnitude * Mathf.Sin(sinX) * Vector3.right;

            lastPosition = transform.position;
        }
        private void JumpSwayEffect()
        {
            if(!haveJumpSway || aiming) return;

            switch (fpsController.IsGrounded)
            {
                case false:
                    // Adjust the weapon's rotation based on the player's jump velocity.
                    float yVelocity = fpsController.jumpVelocity.y;
                    yVelocity = Mathf.Clamp(yVelocity, -weaponMinClamp, weaponMaxClamp);
                    impactForce = -yVelocity * landingIntensity;

                    if (aiming)
                    {
                        yVelocity = Mathf.Max(yVelocity, 0);
                    }

                    // Update the weapon's local rotation to simulate the jump sway effect.
                    this.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, Quaternion.Euler(0f, 0f, yVelocity * jumpIntensity), Time.deltaTime * jumpSmooth);
                    break;
                case true when impactForce >= 0:
                    // If the player is grounded and has impact force, adjust the weapon's rotation accordingly.
                    this.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, Quaternion.Euler(0, 0, impactForce), Time.deltaTime * landingSmooth);
                    impactForce -= recoverySpeed * Time.deltaTime;
                    break;
                case true:
                    // If the player is grounded and there's no impact force, reset the weapon's rotation.
                    this.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, Quaternion.identity, Time.deltaTime * landingSmooth);
                    break;
            }
        }
        #endregion
        #endregion
    }
}