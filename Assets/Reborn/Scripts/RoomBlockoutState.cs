using UnityEngine;

namespace Reborn
{
    // Layout review only. Story progression and interactions are added separately.
    public sealed class RoomBlockoutState : MonoBehaviour
    {
        public enum Layout { Reality, Opening, Exit }
        [SerializeField] private Layout layout = Layout.Reality;
        [SerializeField] private GameObject smallCabinet;
        [SerializeField] private Transform largeCabinet;
        [SerializeField] private GameObject memoryDoor;
        [SerializeField] private GameObject entranceDoor;
        [SerializeField] private GameObject drawers;

        public Layout Current => layout;

        public void Configure(GameObject small, Transform large, GameObject memory, GameObject entrance, GameObject fronts)
        {
            smallCabinet = small; largeCabinet = large; memoryDoor = memory;
            entranceDoor = entrance; drawers = fronts;
            Apply(Layout.Reality);
        }

        public void Apply(Layout value)
        {
            layout = value;
            if (smallCabinet == null || largeCabinet == null) return;
            smallCabinet.SetActive(value == Layout.Reality);
            largeCabinet.gameObject.SetActive(value != Layout.Reality);
            largeCabinet.localPosition = new Vector3(value == Layout.Exit ? -0.98f : 0.45f, 0, 0.45f);
            memoryDoor.SetActive(value != Layout.Reality);
            entranceDoor.SetActive(value != Layout.Exit);
            drawers.SetActive(value != Layout.Exit);
        }

        private void OnValidate() => Apply(layout);
    }
}
