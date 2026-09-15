using UnityEngine;

namespace Reborn
{
    [RequireComponent(typeof(InspectableObject))]
    [DisallowMultipleComponent]
    public sealed class CarryableObject : MonoBehaviour
    {
        public enum Location { Home, Held, Placed }
        public Location Current { get; private set; }
        public InspectableObject Item => GetComponent<InspectableObject>();
        public ItemPlacementTarget PlacedAt { get; private set; }
        private Transform homeParent;
        private Vector3 homePosition;
        private Quaternion homeRotation;
        private Vector3 homeScale;
        private Collider[] colliders;
        private bool[] colliderStates;

        private void Awake()
        {
            homeParent=transform.parent; homePosition=transform.position;
            homeRotation=transform.rotation; homeScale=transform.localScale;
            colliders=GetComponentsInChildren<Collider>(true);
            colliderStates=new bool[colliders.Length];
        }

        public bool TryHold(Transform anchor)
        {
            if (!isActiveAndEnabled || Current!=Location.Home || !anchor) return false;
            // This first carry implementation supports static props only.
            if (GetComponentInChildren<Rigidbody>()) return false;
            for(int i=0;i<colliders.Length;i++)
            { colliderStates[i]=colliders[i].enabled; colliders[i].enabled=false; }
            Current=Location.Held;
            transform.SetParent(anchor,true);
            transform.localPosition=new Vector3(.22f,-.18f,.55f);
            transform.localRotation=Quaternion.Euler(55,0,0);
            return true;
        }

        internal bool TryPlace(ItemPlacementTarget target, Transform anchor)
        {
            if (Current!=Location.Held || !target || !anchor || !isActiveAndEnabled) return false;
            transform.SetParent(homeParent,true);
            transform.SetPositionAndRotation(anchor.position,anchor.rotation);
            transform.localScale=homeScale;
            Current=Location.Placed; PlacedAt=target; RestoreColliders();
            return true;
        }

        public void Recover()
        {
            if(Current==Location.Home) return;
            if(PlacedAt) PlacedAt.Release(this);
            PlacedAt=null; Current=Location.Home;
            transform.SetParent(homeParent,true);
            transform.SetPositionAndRotation(homePosition,homeRotation);
            transform.localScale=homeScale; RestoreColliders();
        }

        private void RestoreColliders()
        {
            if(colliders==null) return;
            for(int i=0;i<colliders.Length;i++) if(colliders[i]) colliders[i].enabled=colliderStates[i];
        }
        private void OnDisable() { if(Current!=Location.Home) Recover(); }
    }
}
