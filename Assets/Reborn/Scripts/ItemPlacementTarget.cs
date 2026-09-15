using UnityEngine;

namespace Reborn
{
    [DisallowMultipleComponent]
    public sealed class ItemPlacementTarget : MonoBehaviour
    {
        [SerializeField] private string acceptedItemId;
        [SerializeField] private string displayName;
        [SerializeField] private Transform placementAnchor;
        [SerializeField] private GameObject completionIndicator;
        public CarryableObject Occupant { get; private set; }
        public bool IsCompleted => Occupant && Occupant.Current==CarryableObject.Location.Placed && Occupant.PlacedAt==this;
        public string DisplayName => displayName;

        public void Configure(string acceptedId,string title,Transform anchor,GameObject indicator)
        { acceptedItemId=acceptedId; displayName=title; placementAnchor=anchor; completionIndicator=indicator; }
        private void Awake() { if(completionIndicator) completionIndicator.SetActive(false); }
        public bool CanAccept(CarryableObject item)
        {
            return isActiveAndEnabled && placementAnchor && !Occupant && item && item.isActiveAndEnabled
                && item.Current==CarryableObject.Location.Held && !string.IsNullOrEmpty(acceptedItemId)
                && item.Item.StableId==acceptedItemId;
        }
        public bool TryAccept(CarryableObject item)
        {
            if(!CanAccept(item) || !item.TryPlace(this,placementAnchor)) return false;
            Occupant=item;
            if(completionIndicator) completionIndicator.SetActive(true);
            return true;
        }
        internal void Release(CarryableObject item)
        {
            if(Occupant!=item) return;
            Occupant=null;
            if(completionIndicator) completionIndicator.SetActive(false);
        }
        private void OnDisable() { if(Occupant) Occupant.Recover(); }
        private void LateUpdate()
        {
            // Also clear the visual if an occupied object was destroyed externally.
            if(!IsCompleted && completionIndicator) completionIndicator.SetActive(false);
        }
    }
}
