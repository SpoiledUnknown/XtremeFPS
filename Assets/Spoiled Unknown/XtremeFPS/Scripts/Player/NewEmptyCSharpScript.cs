using UnityEngine;

public class NewEmptyCSharpScript
{
    ////Interactions
    //public float interactionRange;
    //public int interactionLayerId;


    //private void InteractionHandling()
    //{
    //    if (inputManager.isTryingToInteract)
    //    {
    //        Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRange, interactionLayerMask);

    //        foreach (Collider collider in colliders)
    //        {
    //            if (collider.TryGetComponent(out IWeaponPickup pickup) && !isZoomed)
    //            {
    //                if (pickup.IsEquiped()) continue;
    //                if (WeaponHolder.Instance.GetWeaponCount() < 3) pickup.PickUp();
    //                break;
    //            }
    //        }
    //    }
    //    else if (inputManager.isTryingToInteractAlternate)
    //    {
    //        Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRange, interactionLayerMask);

    //        foreach (Collider collider in colliders)
    //        {
    //            if (collider.TryGetComponent(out IWeaponPickup pickup) && !isZoomed)
    //            {
    //                if (pickup.IsEquiped() && pickup.IsActive()) pickup.Drop();
    //                break;
    //            }
    //        }
    //    }
    //}
}
