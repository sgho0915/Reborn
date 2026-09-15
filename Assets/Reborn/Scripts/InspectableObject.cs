using UnityEngine;

namespace Reborn
{
    [DisallowMultipleComponent]
    public sealed class InspectableObject : MonoBehaviour
    {
        [SerializeField] private string stableId;
        [SerializeField] private string displayName;
        [SerializeField, TextArea(3, 12)] private string description;

        public string StableId => stableId;
        public string DisplayName => displayName;
        public string Description => description;

        public void Configure(string id, string title, string text)
        { stableId = id; displayName = title; description = text; }
    }
}
