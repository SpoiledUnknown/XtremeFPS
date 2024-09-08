/*Copyright © Spoiled Unknown*/
/*2024*/

using UnityEngine;
using UnityEngine.Animations.Rigging;
using XtremeFPS.FPSController;
using XtremeFPS.WeaponSystem.Pickup;

namespace XtremeFPS.Demo
{
    public class AnimationHandler : MonoBehaviour
    {
        public RigLayer RightRigLayer;
        public RigLayer LeftRigLayer;
        public Animator animator;
        public FirstPersonController personController;

        private void Update()
        {
            if (WeaponPickup.IsWeaponEquipped)
            {
                RightRigLayer.rig.weight = 1f;
                LeftRigLayer.rig.weight = 1f;
            }
            else
            {
                RightRigLayer.rig.weight = 0f;
                LeftRigLayer.rig.weight = 0f;
            }

            if (WeaponPickup.IsWeaponEquipped || personController.CharacterController.velocity.magnitude <= 0)
            {
                animator.SetBool("IsNotHolding", false);
            }
            else if (personController.MovementState == FirstPersonController.PlayerMovementState.Sprinting)
            {
                animator.SetBool("IsNotHolding", true);
                animator.speed = 1.25f;
            }
            else
            {
                animator.SetBool("IsNotHolding", true);
                animator.speed = 0.5f;
            }
        }
    }
}
